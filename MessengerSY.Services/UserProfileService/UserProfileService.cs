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
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRefreshTokenInfo _refreshTokenInfo;
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileService(IRepository<UserProfile> userProfileRepository,
            IUnitOfWork unitOfWork, IRefreshTokenInfo refreshTokenInfo,
            IRepository<RefreshToken> refreshTokenRepository, IRepository<Contact> contactRepository)
        {
            _userProfileRepository = userProfileRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _contactRepository = contactRepository;
            _refreshTokenInfo = refreshTokenInfo;
            _unitOfWork = unitOfWork;
        }

        #region UserProfile

        public IEnumerable<UserProfile> GetExistsUserProfiles(IEnumerable<string> phoneNumbers)
        {
            if (phoneNumbers == null)
                throw new ArgumentNullException(nameof(phoneNumbers));

            return _userProfileRepository.TableNoTracking.Where(userProfile =>
                phoneNumbers.Contains(userProfile.PhoneNumber));
        }

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

        public async Task<bool> IsUserProfileExists(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            return await _userProfileRepository.Any(userProfile => userProfile.Id == userProfileId);
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

            await AddUserProfile(userProfile);

            return (userProfile, refreshTokenModel);
        }

        public async Task AddUserProfile(UserProfile userProfile)
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
                refreshToken.Expires = refreshToken.Expires.AddHours(_refreshTokenInfo.ExtendTimeHours);
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

        #region Contact

        public async Task<Contact> GetContact(int ownerUserProfileId, int linkedUserProfileId)
        {
            if (ownerUserProfileId <= 0)
                throw new ArgumentException(nameof(ownerUserProfileId));

            if (linkedUserProfileId <= 0)
                throw new ArgumentException(nameof(linkedUserProfileId));

            return await _contactRepository.GetOne(contact =>
                contact.ContactOwnerId == ownerUserProfileId && contact.LinkedUserProfileId == linkedUserProfileId);
        }

        public IEnumerable<Contact> GetUserProfileContects(int userProfileId, bool IsTracking)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            IQueryable<Contact> table;
            if (IsTracking)
            {
                table = _contactRepository.Table;
            }
            else
            {
                table = _contactRepository.TableNoTracking;
            }

            return table.Where(contact => contact.ContactOwnerId == userProfileId);
        }

        public IEnumerable<Contact> GetLinkedUserProfileContacts(string linkedPhoneNumber, bool isTracking)
        {
            if (string.IsNullOrWhiteSpace(linkedPhoneNumber))
                throw new ArgumentNullException(nameof(linkedPhoneNumber));

            IQueryable<Contact> table;
            if (isTracking)
            {
                table = _contactRepository.Table;
            }
            else
            {
                table = _contactRepository.TableNoTracking;
            }

            return table.Where(contact =>
                contact.LinkedUserProfileId == null && contact.PhoneNumber == linkedPhoneNumber);
        }

        public IEnumerable<Contact> GetLinkedUserProfileContacts(int ownerUserProfileId, bool isTracking)
        {
            if (ownerUserProfileId <= 0)
                throw new ArgumentException(nameof(ownerUserProfileId));

            IQueryable<Contact> table;
            if (isTracking)
            {
                table = _contactRepository.Table;
            }
            else
            {
                table = _contactRepository.TableNoTracking;
            }

            return table.Where(contact =>
                contact.ContactOwnerId == ownerUserProfileId && contact.LinkedUserProfileId != null);
        }

        public async Task AddContact(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            await _contactRepository.Add(contact);
            await _unitOfWork.Commit();
        }

        public async Task AddContacts(IEnumerable<Contact> contacts)
        {
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));

            await _contactRepository.AddRange(contacts);
            await _unitOfWork.Commit();
        }

        public async Task UpdateContacts(IEnumerable<Contact> contacts)
        {
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));

            _contactRepository.UpdateRange(contacts);
            await _unitOfWork.Commit();
        }

        public async Task<bool> IsContactExists(int userProfileOwnerId, int linkedUserProfileId)
        {
            if (userProfileOwnerId <= 0)
                throw new ArgumentException(nameof(userProfileOwnerId));

            if (linkedUserProfileId <= 0)
                throw new ArgumentException(nameof(linkedUserProfileId));

            return await _contactRepository.Any(contact =>
                contact.ContactOwnerId == userProfileOwnerId && contact.LinkedUserProfileId == linkedUserProfileId);
        }

        public async Task<bool> IsContactExists(int ownerUserProfileId, string phoneNumber)
        {
            if (ownerUserProfileId <= 0)
                throw new ArgumentException(nameof(ownerUserProfileId));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            return await _contactRepository.Any(contact =>
                contact.ContactOwnerId == ownerUserProfileId && contact.PhoneNumber == phoneNumber);
        }

        #endregion
    }
}
