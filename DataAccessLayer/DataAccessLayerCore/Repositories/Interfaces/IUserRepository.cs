using DataAccessLayerCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerCore.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        /// <summary>
        /// Checks if an email is occupied.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns><c>true</c> if the email is occupied, <c>false</c> otherwise.</returns>
        bool IsUsernameOccupied(string email);

        /// <summary>
        /// Retrieves a user from the database given an email.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>
        /// The user object if found in the database, otherwise null.
        /// </returns>
        Task<User?> GetUserByUsername(string username);
    }
}
