using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("api/inventory/items")]
    public class ItemsController(
        IRepository<InventoryItem> inventoryItemsRepository,
        IRepository<CatalogItem> catalogItemsRepository,
        IdentityClient identityClient) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await identityClient.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound($"User with id {userId} not found.");
            }

            var inventoryItemsEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);

            var itemIds = inventoryItemsEntities.Select(item => item.CatalogItemId);

            var catalogItemsEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemsDtos = inventoryItemsEntities.Select(i =>
            {
                var catalogItem = catalogItemsEntities.Single(c => c.Id == i.CatalogItemId);
                return i.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemsDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandtItemsDto grandtItemsDto)
        {
            var user = await identityClient.GetUserAsync(grandtItemsDto.UserId);
            if (user == null)
            {
                return NotFound($"User with id {grandtItemsDto.UserId} not found.");
            }
            var catalogItem = await catalogItemsRepository.GetAsync(grandtItemsDto.CatalogItemId);
            if (catalogItem == null)
            {
                return NotFound($"Catalog item with id {grandtItemsDto.CatalogItemId} not found.");
            }

            var amount = catalogItem.Price * grandtItemsDto.Quantity;
            string idempotencyKey = Guid.NewGuid().ToString();
            var debitRequest = new DebitRequestDto(amount, idempotencyKey);

            var debitResult = await identityClient.DebitAsync(user.Id, debitRequest);
            if (!debitResult)
            {
                return BadRequest($"User with id {grandtItemsDto.UserId} has insufficient balance.");
            }

            try
            {
                var inventoryItem = await inventoryItemsRepository.GetAsync
                 (
                    item => item.UserId == grandtItemsDto.UserId &&
                            item.CatalogItemId == grandtItemsDto.CatalogItemId
                 );

                if (inventoryItem == null)
                {
                    inventoryItem = grandtItemsDto.AsInventoryItem();
                    await inventoryItemsRepository.CreateAsync(inventoryItem);
                }
                else
                {
                    inventoryItem.Quantity += grandtItemsDto.Quantity;
                    inventoryItem.AcquiredDate = DateTimeOffset.UtcNow;
                    await inventoryItemsRepository.UpdateAsync(inventoryItem);
                }
                return Ok();
            }
            catch (Exception)
            {
                await identityClient.CreditAsync(user.Id, debitRequest);
                throw;
            }
        }
    }
}
