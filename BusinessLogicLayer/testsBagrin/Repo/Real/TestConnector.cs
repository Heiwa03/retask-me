using Microsoft.EntityFrameworkCore;

using BusinessLogicLayer.testsBagrin.Entity;

namespace BusinessLogicLayer.testsBagrin.Repo.Real{
    public class TestConnector : DbContext{
        public DbSet<TestUser> Users {get; set;}

        public TestConnector(DbContextOptions<TestConnector> options) : base(options){
            
        }
    }
}