using DataAccessLayerCore.Entities;
using System.Threading.Tasks;

namespace DataAccessLayerCore.Repositories.Interfaces
{
    public interface IUserSessionRepository : IBaseRepository
    {
        /// <summary>
        /// Retrieves a user session by its refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token of the session to retrieve.</param>
        /// <returns>The UserSession entity if found, otherwise null.</returns>
        Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Retrieves a user session by its user ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user whose session is to be retrieved.</param>
        /// <returns>The UserSession entity if found, otherwise null.</returns>
        Task<UserSession?> GetSessionByUserIdAsync(long userId);

        /// <summary>
        /// Retrieves a user session by its JWT ID asynchronously.
        /// </summary>
        /// <param name="jwtId">The unique ID of the JWT associated with the session.</param>
        /// <returns>The UserSession entity if found, otherwise null.</returns>
        Task<UserSession?> GetSessionByJwtIdAsync(string jwtId);

        /// <summary>
        /// Marks a refresh token as redeemed and updates the session with a new JWT ID.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be redeemed.</param>
        /// <param name="jwtId">The new JWT ID to associate with the session.</param>
        /// <returns>Boolean True if Session was found, otherwise false.</returns>
        Task<bool> RedeemRefreshTokenAsync(string refreshToken, string jwtId);

        /// <summary>
        /// Removes all user sessions for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose sessions are to be removed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task RemoveSessionByUserIdAsync(long userId);
    }
}