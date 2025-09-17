// System
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

// DAL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;

namespace DataAccessLayerCore.Repositories
{
    public class BaseRepository(DatabaseContext databaseContext) : IBaseRepository
    {
        /// <summary>
        /// Adds an entity of type <typeparamref name="TEntity"/> to the data context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to add.</typeparam>
        /// <param name="entity">The entity to be added.</param>
        /// <remarks>
        /// This method adds the specified <paramref name="entity"/> to the data context using the DbSet of <typeparamref name="TEntity"/>.
        /// </remarks>
        public void Add<TEntity>(TEntity entity) where TEntity : BaseId
        {
            databaseContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to delete.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        /// <remarks>
        /// This method removes the specified entity from the database.
        /// </remarks>
        public void Delete<TEntity>(TEntity entity) where TEntity : BaseId
        {
            databaseContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Gets an entity by its ID from the database asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<TEntity?> GetByIdAsync<TEntity>(long id) where TEntity : BaseId
        {
            var entity = await databaseContext.Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID, or null if not found.</returns>
        public TEntity? GetById<TEntity>(long id) where TEntity : BaseId
        {
            var entity = databaseContext.Set<TEntity>()
                .FirstOrDefault(x => x.Id == id);

            return entity;
        }

        /// <summary>
        /// Retrieves an entity by its UUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="uuid">The UUID of the entity to retrieve.</param>
        /// <returns>
        /// The entity with the specified UUID, or null if not found.
        /// </returns>
        public async Task<TEntity?> GetByUuidAsync<TEntity>(Guid uuid) where TEntity : BaseId
        {
            var entity = await databaseContext.Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            return entity;
        }

        /// <summary>
        /// Retrieves the entity of type <paramref name="TEntity"/> from the database by the specified UUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <param name="uuid">The UUID of the entity.</param>
        /// <returns>The entity with the specified UUID, or null if not found.</returns>
        public TEntity? GetByUuid<TEntity>(Guid uuid) where TEntity : BaseId
        {
            var entity = databaseContext.Set<TEntity>()
                .FirstOrDefault(x => x.Uuid == uuid);

            return entity;
        }


        /// <summary>
        /// Updates the specified entity in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
        public void Update<TEntity>(TEntity entity) where TEntity : BaseId
        {
            databaseContext.Set<TEntity>().Update(entity);
        }


        /// <summary>
        /// Saves the changes made in the data context asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the number of objects written to the underlying database.
        /// </returns>
        public async Task<int> SaveChangesAsync()
        {
            return await databaseContext.SaveChangesAsync();
        }

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="IDbContextTransaction"/> that represents the transaction.</returns>
        /// <remarks>
        /// This method starts a new transaction on the database context.
        /// </remarks>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await databaseContext.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Skips the specified number of items based on the page number and size.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="size">The number of items per page.</param>
        /// <returns>The number of items to skip.</returns>
        protected int SkipSize(int page, int size)
        {
            return (page - 1) * size;
        }
    }
}
