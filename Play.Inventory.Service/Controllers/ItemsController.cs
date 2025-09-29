using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController(
        IRepository<InventoryItem> inventoryItemsRepository,
        IRepository<CatalogItem> catalogItemsRepository) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
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
    }
}
