using MessengerSY.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.UserProfileService
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfileById(int userProfileId);
        Task<UserProfile> GetUserProfileByPhone(string phoneNumber);
        Task<bool> IsUserProfileExists(string phoneNumber);
        Task<(UserProfile userProfile, RefreshToken refreshToken)> CreateUserProfile(string phoneNumber, string refreshToken);
        Task CreateUserProfile(UserProfile userProfile);
        
        Task<RefreshToken> GetRefreshTokenById(int refreshTokenId);
        IEnumerable<RefreshToken> GetUserProfileRefreshTokensNoTracking(int userProfileId);
        IEnumerable<RefreshToken> GetUserProfileRefreshTokens(int userProfileId);
        bool CheckRefreshTokenExpirationDate(RefreshToken refreshToken);
        Task UpdateRefreshToken(RefreshToken refreshToken);
        Task ExtendRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> CreateRefreshToken(string token, int userProfileId);
        Task CreateRefreshToken(RefreshToken refreshToken);
        Task DeleteRefreshToken(RefreshToken refreshToken);
        Task DeleteRefreshTokens(IEnumerable<RefreshToken> refreshTokens);
    }
}
