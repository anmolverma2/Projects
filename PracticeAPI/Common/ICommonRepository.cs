using PracticeAPI.Model;
using System.Linq.Expressions;

namespace PracticeAPI.Common
{
    public interface ICommonRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByAnyFilter(Expression<Func<T,bool>> filter,bool isTracking = false);
        Task<List<T>> GetAllByFilterAsync(Expression<Func<T,bool>> filter,bool isTracking = false);
        Task<bool> DeleteAsync(T record);
        Task<T> UpdateAsync(T record);
        Task<T> Create(T record);
    }
}
