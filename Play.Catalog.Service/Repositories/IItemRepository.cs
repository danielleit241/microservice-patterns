using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IItemRepository
    {
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(Guid id);
        Task CreateAsync(Item item);
        Task UpdateAsync(Item item);
        Task RemoveAsync(Guid id);
    }
}
