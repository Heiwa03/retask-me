using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayerCore.Repositories
{
    public class UserSessionRepository(DatabaseContext databaseContext) : BaseRepository(databaseContext), IUserSessionRepository
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

        public async Task<bool> RedeemRefreshTokenAsync(string refreshToken, string jwtId)
        {
            var session = await _databaseContext.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

            if (session != null)
            {
                session.Redeemed = true;
                //session.isActive = false;
                session.JwtId = jwtId;
                _databaseContext.Set<UserSession>().Update(session);
                return true;
            }
            return false;
        }

        /*public async Task RemoveSessionByUserIdAsync(long userId)
        {
            await _databaseContext.Set<UserSession>()
                .Where(s => s.UserId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    session => session.isActive,
                    false));
        }*/

        public async Task RemoveSessionByUserIdAsync(long userId)
        {
            // 1. Retrieve all matching entities (Db trip #1)
            var sessions = await _databaseContext.Set<UserSession>()
                .Where(s => s.UserId == userId)
                .ToListAsync();

            // 2. Modify the property of each entity in memory
            // (This automatically marks them as 'Modified' by the Change Tracker)
            foreach (var session in sessions)
            {
                session.isActive = false;
                // Note: You do NOT explicitly call .Update() here 
                // because the entities are already tracked.
            }

            // 3. Save all changes (Db trip #2, sends N UPDATE commands)
            await _databaseContext.SaveChangesAsync();
        }
    }
}