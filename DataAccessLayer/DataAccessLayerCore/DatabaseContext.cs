using DataAccessLayerCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayerCore
{
    public partial class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<DailyTask> DailyTasks {get; set;}
        public virtual DbSet<Board> Board {get; set; }
    }
}
