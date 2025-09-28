using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("api/items")]
    public class ItemsController(IRepository<Item> repo, IPublishEndpoint publishEndpoint) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var items = await repo.GetAllAsync();
            return Ok(items.Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid id)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
        {
            var item = createItemDto.AsItem();
            await repo.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;
            await repo.UpdateAsync(item);

            await publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await repo.RemoveAsync(id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}
