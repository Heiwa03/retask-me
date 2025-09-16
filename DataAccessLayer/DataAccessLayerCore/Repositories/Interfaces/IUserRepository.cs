using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        /// <summary>
        /// Checks if a username is occupied.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username is occupied, <c>false</c> otherwise.</returns>
        bool IsUserNameOccupied(string username);

        /// <summary>
        /// Retrieves a user from the database given a username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>
        /// The user object if found in the database, otherwise null.
        /// </returns>
        Task<User?> GetUserByUsername(string username);

        /// <summary>
        /// Retrieves a user session by the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token used to identify the user session.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result is a nullable <see cref="UserSession"/> object if found, otherwise <c>null</c>.</returns>
        Task<UserSession?> GetSessionByRefreshToken(string refreshToken);
    }
}
