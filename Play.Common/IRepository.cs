using System.Linq.Expressions;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdAsync(Expression<Func<T, bool>> filter);
        Task CreateAsync(T item);
        Task UpdateAsync(T item);
        Task RemoveAsync(Guid id);
    }
}
