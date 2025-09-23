using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayerCore.Repositories
{
    public class UserSessionRepository (DatabaseContext databaseContext): BaseRepository (databaseContext), IUserSessionRepository
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        public async Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken)
        {
            return await _databaseContext.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.isActive);
        }

        public async Task<UserSession?> GetSessionByUserIdAsync(long userId)
        {
            return await _databaseContext.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.isActive);
        }

        public async Task<UserSession?> GetSessionByJwtIdAsync(string jwtId)
        {
            return await _databaseContext.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.JwtId == jwtId && s.isActive);
        }

        public async Task RedeemRefreshTokenAsync(string refreshToken, string jwtId)
        {
            var session = await _databaseContext.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

            if (session != null)
            {
                session.Redeemed = true;
                session.isActive = false;
                session.JwtId = jwtId;
                _databaseContext.Set<UserSession>().Update(session);
            }
        }

        public async Task RemoveSessionByUserIdAsync(long userId)
        {
            var sessions = await _databaseContext.Set<UserSession>()
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (sessions.Any())
            {
                _databaseContext.Set<UserSession>().RemoveRange(sessions);
            }
        }
    }
}