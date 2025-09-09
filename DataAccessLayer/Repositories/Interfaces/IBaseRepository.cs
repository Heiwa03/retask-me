using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        /// <summary>
        /// Adds an entity of type <typeparamref name="TEntity"/> to the data context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to add.</typeparam>
        /// <param name="entity">The entity to be added.</param>
        /// <remarks>
        /// This method adds the specified <paramref name="entity"/> to the data context using the DbSet of <typeparamref name="TEntity"/>.
        /// </remarks>
        void Add<TEntity>(TEntity entity) where TEntity : BaseId;

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to delete.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        /// <remarks>
        /// This method removes the specified entity from the database.
        /// </remarks>
        void Delete<TEntity>(TEntity entity) where TEntity : BaseId;

        /// <summary>
        /// Gets an entity by its ID from the database asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        Task<TEntity?> GetByIdAsync<TEntity>(long id) where TEntity : BaseId;

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID, or null if not found.</returns>
        TEntity? GetById<TEntity>(long id) where TEntity : BaseId;

        /// <summary>
        /// Retrieves an entity by its UUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="uuid">The UUID of the entity to retrieve.</param>
        /// <returns>
        /// The entity with the specified UUID, or null if not found.
        /// </returns>
        Task<TEntity?> GetByUuidAsync<TEntity>(Guid uuid) where TEntity : BaseId;

        /// <summary>
        /// Retrieves the entity of type <paramref name="TEntity"/> from the database by the specified UUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <param name="uuid">The UUID of the entity.</param>
        /// <returns>The entity with the specified UUID, or null if not found.</returns>
        TEntity? GetByUuid<TEntity>(Guid uuid) where TEntity : BaseId;

        /// <summary>
        /// Updates the specified entity in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
        void Update<TEntity>(TEntity entity) where TEntity : BaseId;

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="IDbContextTransaction"/> that represents the transaction.</returns>
        /// <remarks>
        /// This method starts a new transaction on the database context.
        /// </remarks>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Saves the changes made in the data context asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the number of objects written to the underlying database.
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}
