using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayerCore;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;

namespace DataAccessLayerCore.Repositories
{
    public class UserRepository (DatabaseContext databaseContext) : BaseRepository(databaseContext), IUserRepository
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        /// <summary>
        /// Checks if an username is occupied.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username is occupied, <c>false</c> otherwise.</returns>
        public bool IsUsernameOccupied(string username)
        {
            return _databaseContext.Users.Any(x => x.NormalizedUsername == username.ToUpper());
        }

        /// <summary>
        /// Retrieves a user from the database given an username.
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
    }
}
