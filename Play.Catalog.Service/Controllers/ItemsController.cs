using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("api/items")]
    public class ItemsController(IRepository<Item> repo) : ControllerBase
    {
        private static int requestCounter = 0;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            requestCounter++;
            Console.WriteLine($"Request {requestCounter}: starting...");

            if (requestCounter <= 2)
            {
                Console.WriteLine($"Request {requestCounter}: Delaying...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounter <= 4)
            {
                Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error)...");
                return StatusCode(500);
            }

            var items = await repo.GetAllAsync();

            Console.WriteLine($"Request {requestCounter}: 200 (OK)...");
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
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var item = repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            repo.RemoveAsync(id);
            return NoContent();
        }
    }
}
