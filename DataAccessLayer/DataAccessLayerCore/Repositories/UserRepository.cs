// Microsoft
using Microsoft.EntityFrameworkCore;

// DAL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserRepository (DatabaseContext databaseContext) : BaseRepository(databaseContext), IUserRepository
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        /// <summary>
        /// Checks if a username is occupied.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username is occupied, <c>false</c> otherwise.</returns>
        public bool IsUserNameOccupied(string username)
        {
            return _databaseContext.Users.Any(x => x.NormalizedUsername == username.ToUpper());
        }

        /// <summary>
        /// Retrieves a user from the database given a username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>
        /// The user object if found in the database, otherwise null.
        /// </returns>
        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.NormalizedUsername == username.ToUpper());

            return user;
        }

        /// <summary>
        /// Retrieves a user session by the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token used to identify the user session.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result is a nullable <see cref="UserSession"/> object if found, otherwise <c>null</c>.</returns>
        public async Task<UserSession?> GetSessionByRefreshToken(string refreshToken)
        {
            var session = await _databaseContext.UserSessions.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

            return session;
        }
    }
}
