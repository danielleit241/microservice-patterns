using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepository : IItemRepository
    {
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> dbCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public ItemsRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            var items = await dbCollection.Find(filterBuilder.Empty).ToListAsync();
            return items;
        }

        public async Task<Item?> GetByIdAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            var item = await dbCollection.FindAsync(filter);
            return item.FirstOrDefault();
        }

        public async Task CreateAsync(Item item)
        {
            ArgumentNullException.ThrowIfNull(item);
            await dbCollection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(Item item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await dbCollection.ReplaceOneAsync(filter, item);
        }

        public async Task RemoveAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
