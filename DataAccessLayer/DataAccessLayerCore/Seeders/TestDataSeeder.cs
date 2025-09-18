<<<<<<< HEAD
﻿using DataAccessLayerCore.Entities;
using System;
=======
﻿using System;
>>>>>>> data-access-implementation
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
using DataAccessLayerCore;
=======
>>>>>>> data-access-implementation
using System.Security.Cryptography;
using System.Runtime.InteropServices.Swift;
using DataAccessLayerCore;
using DataAccessLayerCore.Entities;

namespace DataAccessLayerCore.Seeders
{
    public class TestDataSeeder
    {
        private readonly DatabaseContext _databaseContext;

        public TestDataSeeder (DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task Initialize()
        {
            await using var transaction = await _databaseContext.Database.BeginTransactionAsync();
            try
            {
                await Seed();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Seed()
        {
            if (_databaseContext.Users.Any())
            {
                Console.WriteLine("DB already has data. No seeding.");
                return;
            }

            var usersToAdd = new List<User>
            {
                new User 
                { 
                    Uuid = Guid.NewGuid(),
                    Username = "Ivan",
                    NormalizedUsername = "IVAN",
                    Password = "12341234"
                },
                new User
                {
                    Uuid = Guid.NewGuid(),
                    Username = "Dan",
                    NormalizedUsername = "DAN",
                    Password = "123321123"
                },
                new User
                {
                    Uuid = Guid.NewGuid(),
                    Username = "Vlad",
                    NormalizedUsername = "VLAD",
                    Password = "12344321"
                }
            };

            _databaseContext.Users.AddRange(usersToAdd);

            await _databaseContext.SaveChangesAsync();
        }
    }
}
