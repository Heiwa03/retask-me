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


namespace DataAccessLayerCore.Repositories
{
    public class UserRepository (DatabaseContext databaseContext) : BaseRepository(databaseContext), IUserRepository
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        /// <summary>
        /// Checks if an email is occupied.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns><c>true</c> if the email is occupied, <c>false</c> otherwise.</returns>
        public bool IsUsernameOccupied(string email)
        {
            return _databaseContext.Users.Any(x => x.NormalizedUsername == email.ToUpper());
        }

        /// <summary>
        /// Retrieves a user from the database given an email.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>
        /// The user object if found in the database, otherwise null.
        /// </returns>
        public async Task<User?> GetUserByUsername(string email)
        {
            var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.NormalizedUsername == email.ToUpper());

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
