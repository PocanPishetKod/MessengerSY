using MessengerSY.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.UserProfileService
{
    public interface IUserProfileService
    {
        IEnumerable<UserProfile> GetExistsUserProfiles(IEnumerable<string> phoneNumbers);
        Task<UserProfile> GetUserProfileById(int userProfileId);
        Task<UserProfile> GetUserProfileByPhone(string phoneNumber);
        Task<bool> IsUserProfileExists(string phoneNumber);
        Task<bool> IsUserProfileExists(int userProfileId);
        Task<(UserProfile userProfile, RefreshToken refreshToken)> CreateUserProfile(string phoneNumber, string refreshToken);
        Task AddUserProfile(UserProfile userProfile);
        
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

        Task<Contact> GetContact(int ownerUserProfileId, int linkedUserProfileId);
        IEnumerable<Contact> GetNotLinkedUserProfileContacts(string linkedPhoneNumber, bool isTracking);
        IEnumerable<Contact> GetLinkedUserProfileContacts(int ownerUserProfileId, bool isTracking);
        IEnumerable<Contact> GetUserProfileContects(int userProfileId, bool IsTracking);
        Task AddContact(Contact contact);
        Task AddContacts(IEnumerable<Contact> contacts);
        Task UpdateContacts(IEnumerable<Contact> contacts);
        Task<bool> IsContactExists(int userProfileOwnerId, int linkedUserProfileId);
        Task<bool> IsContactExists(int ownerUserProfileId, string phoneNumber);
    }
}
