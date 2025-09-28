using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemAsync();

            var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);

            var invetoryItemsDto = from i in inventoryItemEntities
                                   join c in catalogItems
                                   on i.CatalogItemId equals c.Id
                                   select i.AsDto(c.Name, c.Description);

            return Ok(invetoryItemsDto);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandtItemsDto grandtItemsDto)
        {
            var inventoryItem = await itemsRepository.GetAsync
             (
                item => item.UserId == grandtItemsDto.UserId &&
                        item.CatalogItemId == grandtItemsDto.CatalogItemId
             );

            if (inventoryItem == null)
            {
                inventoryItem = grandtItemsDto.AsInventoryItem();
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grandtItemsDto.Quantity;
                inventoryItem.AcquiredDate = DateTimeOffset.UtcNow;
                await itemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}
