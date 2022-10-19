using Clay.SmartDoor.Core.Entities;
using System.Linq.Expressions;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Begins tracking the entity, and any other reachable
        /// entity that are not already being tracked in the entity added state
        /// such that they will be inserted into the database when <seealso cref="IUnitOfWork.SaveAsync"/>
        /// is called.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Add task that represents the asynchronous Add operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> expression, List<string> includes = null!);
        
        IQueryable<T> GetAll(
            Expression<Func<T, bool>> expression = null!, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!, 
            List<string> includes = null!);
        void Update(T entity);
    }
}
