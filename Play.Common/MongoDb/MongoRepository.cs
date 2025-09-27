using MongoDB.Driver;
using System.Linq.Expressions;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            var items = await dbCollection.Find(filterBuilder.Empty).ToListAsync();
            return items;
        }
        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            var items = await dbCollection.Find(filter).ToListAsync();
            return items;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            var item = await dbCollection.FindAsync(filter);
            return item.FirstOrDefault();
        }

        public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> filter)
        {
            var item = await dbCollection.FindAsync(filter);
            return item.FirstOrDefault();
        }

        public async Task CreateAsync(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            await dbCollection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(T item)
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
