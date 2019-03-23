using MessengerSY.Core.Domain;
using MessengerSY.Core.RefreshTokenOptions;
using MessengerSY.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.UserProfileService
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IRefreshTokenInfo _refreshTokenInfo;
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileService(IRepository<UserProfile> userProfileRepository, IUnitOfWork unitOfWork, IRefreshTokenInfo refreshTokenInfo, IRepository<RefreshToken> refreshTokenRepository)
        {
            _userProfileRepository = userProfileRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenInfo = refreshTokenInfo;
            _unitOfWork = unitOfWork;
        }

        #region UserProfile

        public async Task<UserProfile> GetUserProfileById(int userProfileId)
        {
            if (userProfileId <= 0)
                return null;

            return await _userProfileRepository.GetById(userProfileId);
        }

        public async Task<UserProfile> GetUserProfileByPhone(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return await _userProfileRepository.GetOne(userProfile =>
                string.Equals(userProfile.PhoneNumber, phoneNumber));
        }

        public async Task<bool> IsUserProfileExists(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            return await _userProfileRepository.Any(userProfile => string.Equals(userProfile.PhoneNumber, phoneNumber));
        }

        public async Task<(UserProfile userProfile, RefreshToken refreshToken)> CreateUserProfile(string phoneNumber, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            var userProfile = new UserProfile()
            {
                PhoneNumber = phoneNumber,
                RegistrationDate = DateTime.Now
            };
            var refreshTokenModel = BuildRefreshToken(refreshToken);
            userProfile.RefreshTokens.Add(refreshTokenModel);

            await CreateUserProfile(userProfile);

            return (userProfile, refreshTokenModel);
        }

        public async Task CreateUserProfile(UserProfile userProfile)
        {
            if (userProfile == null)
                throw new ArgumentNullException(nameof(userProfile));

            await _userProfileRepository.Add(userProfile);
            await _unitOfWork.Commit();
        }

        public async Task UpdateUserProfile(UserProfile userProfile)
        {
            if (userProfile == null)
                throw new ArgumentNullException(nameof(userProfile));

            _userProfileRepository.Update(userProfile);
            await _unitOfWork.Commit();
        }

        #endregion

        #region UserProfileToken

        private RefreshToken BuildRefreshToken(string token, int userProfileId = 0)
        {
            return new RefreshToken()
            {
                Token = token,
                Expires = DateTime.Now.AddDays(_refreshTokenInfo.RefreshTokenLifeTimeDays),
                UserProfileId = userProfileId
            };
        }

        public async Task<RefreshToken> GetRefreshTokenById(int refreshTokenId)
        {
            if (refreshTokenId <= 0)
                return null;

            return await _refreshTokenRepository.GetById(refreshTokenId);
        }

        public IEnumerable<RefreshToken> GetUserProfileRefreshTokensNoTracking(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            return _refreshTokenRepository.TableNoTracking.Where(refreshToken =>
                refreshToken.UserProfileId == userProfileId);
        }

        public IEnumerable<RefreshToken> GetUserProfileRefreshTokens(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            return _refreshTokenRepository.Table.Where(refreshToken =>
                refreshToken.UserProfileId == userProfileId);
        }

        public async Task<RefreshToken> CreateRefreshToken(string token, int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            var refreshToken = BuildRefreshToken(token, userProfileId);
            await CreateRefreshToken(refreshToken);

            return refreshToken;
        }

        public async Task CreateRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            await _refreshTokenRepository.Add(refreshToken);
            await _unitOfWork.Commit();
        }

        public bool CheckRefreshTokenExpirationDate(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            return refreshToken.IsActive;
        }

        public async Task UpdateRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.Commit();
        }

        public async Task ExtendRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            var offsetDate = refreshToken.Expires - DateTime.Now;
            if (offsetDate.Days <= 1)
            {
                refreshToken.Expires.AddHours(_refreshTokenInfo.ExtendTimeHours);
                await UpdateRefreshToken(refreshToken);
            }
        }

        public async Task DeleteRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            _refreshTokenRepository.Remove(refreshToken);
            await _unitOfWork.Commit();
        }

        public async Task DeleteRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
        {
            if (refreshTokens == null)
                throw new ArgumentNullException(nameof(refreshTokens));

            _refreshTokenRepository.RemoveRange(refreshTokens);
            await _unitOfWork.Commit();
        }

        #endregion
    }
}
