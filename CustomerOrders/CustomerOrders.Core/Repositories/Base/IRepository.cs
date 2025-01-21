using CustomerOrders.Core.Entities.Base;

namespace CustomerOrders.Core.Repositories.Base
{
    public interface IRepository
    {
        public interface IRepository<T> where T : Entity
        {
            Task<IReadOnlyList<T>> GetAllAsync();
            Task<T> GetByIdAsync(int id);
            Task<T> AddAsync(T entity);
            Task UpdateAsync(T entity);
            Task DeleteAsync(T entity);
        }
    }
}
