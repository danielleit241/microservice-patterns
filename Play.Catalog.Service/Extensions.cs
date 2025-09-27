using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static Item AsItem(this ItemDto itemDto)
            => new()
            {
                Id = itemDto.Id,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = itemDto.CreatedDate
            };

        public static Item AsItem(this CreateItemDto createItemDto)
            => new()
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

        public static ItemDto AsDto(this Item item)
            => new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }
}
