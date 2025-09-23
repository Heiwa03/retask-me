using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayerCore.Repositories
{
    public class UserSessionRepository (DatabaseContext databaseContext): BaseRepository (databaseContext), IUserSessionRepository
    {
        public UserSessionRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.Set<UserSession>()
                                   .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.isActive);
        }

        public async Task<UserSession?> GetByUserIdAsync(long userId)
        {
            return await _dbContext.Set<UserSession>()
                                   .FirstOrDefaultAsync(s => s.UserId == userId && s.isActive);
        }

        public async Task<UserSession?> GetByJwtIdAsync(string jwtId)
        {
            return await _dbContext.Set<UserSession>()
                                   .FirstOrDefaultAsync(s => s.JwtId == jwtId && s.isActive);
        }

        public async Task AddAsync(UserSession session)
        {
            await _dbContext.Set<UserSession>().AddAsync(session);
        }

        public async Task RedeemTokenAsync(string refreshToken, string jwtId)
        {
            var session = await _dbContext.Set<UserSession>()
                                            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

            if (session != null)
            {
                session.Redeemed = true;
                session.isActive = false;
                session.JwtId = jwtId;
                _dbContext.Set<UserSession>().Update(session);
            }
        }

        public async Task RemoveByUserIdAsync(long userId)
        {
            var sessions = await _dbContext.Set<UserSession>()
                                           .Where(s => s.UserId == userId)
                                           .ToListAsync();

            if (sessions.Any())
            {
                _dbContext.Set<UserSession>().RemoveRange(sessions);
            }
        }

        public async Task UpdateSessionAsync(UserSession session)
        {
            _dbContext.Set<UserSession>().Update(session);
        }
    }
}