namespace Play.Inventory.Service.Dtos
{
    public record GrandtItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);
}
