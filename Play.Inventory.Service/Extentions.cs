using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public static class Extentions
    {
        public static InventoryItemDto AsDto(this InventoryItem item)
        {
            return new InventoryItemDto(
                item.CatalogItemId,
                item.Quantity,
                item.AcquiredDate
            );
        }

        public static InventoryItem AsInventoryItem(this Dtos.GrandtItemsDto item)
        {
            return new InventoryItem
            {
                UserId = item.UserId,
                CatalogItemId = item.CatalogItemId,
                Quantity = item.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };
        }
    }
}
