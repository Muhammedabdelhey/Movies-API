using System.Linq.Expressions;

namespace Movies_Core_Layer.Interfaces
{
    /// <summary>
    /// Interface for a generic repository providing basic CRUD operations on entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <returns>An IEnumerable of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database with specified navigation properties included.
        /// </summary>
        /// <param name="includes">Array of navigation properties to include.</param>
        /// <returns>An IEnumerable of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync(string[] includes);

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> based on a specified predicate.
        /// </summary>
        /// <param name="expression">The predicate to match the entity this should filter using  <typeparamref name="T"/> Id.</param>
        /// <returns>The first entity that matches the specified predicate.</returns>
        /// 
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> based on a specified predicate, with specified navigation properties included.
        /// </summary>
        /// <param name="expression">The predicate to match the entity this should filter using  <typeparamref name="T"/> Id.</param>
        /// <param name="includes">Array of navigation properties to include.</param>
        /// <returns>The first entity that matches the specified predicate.</returns>
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> expression, string[] includes);

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its identifier,with specified navigation properties included.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        ///<param name="includes">Array of navigation properties to include.</param>
        /// <returns>The entity with the specified identifier.</returns>
        Task<T> GetByIdAsync(int id, string[] includes);

        /// <summary>
        /// Adds a new entity of type <typeparamref name="T"/> to the DataBase.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type <typeparamref name="T"/> in the DataBase.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an existing entity of type <typeparamref name="T"/> from the DataBase.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The deleted entity.</returns>
        Task<T> DeleteAsync(T entity);
    }
}
