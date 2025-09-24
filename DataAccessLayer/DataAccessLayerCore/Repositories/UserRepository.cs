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
        /// <param name="name">The email to check.</param>
        /// <returns><c>true</c> if the email is occupied, <c>false</c> otherwise.</returns>
        public bool IsUsernameOccupied (string email)
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
    }
}
