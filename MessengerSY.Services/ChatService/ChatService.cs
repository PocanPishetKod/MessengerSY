using MessengerSY.Core.Domain;
using MessengerSY.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<UserProfileChat> _userProfileChatRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IRepository<Chat> chatRepository, IRepository<Message> messageRepository, IRepository<UserProfileChat> userProfileChatRepository, IUnitOfWork unitOfWork)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _userProfileChatRepository = userProfileChatRepository;
            _unitOfWork = unitOfWork;
        }

        #region chat

        public async Task<Chat> GetChatById(int chatId)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            return await _chatRepository.GetById(chatId);
        }

        public IEnumerable<Chat> GetUserProfileChats(int userProfileId, int chatCount = 20, bool isTracking = false)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            IQueryable<UserProfileChat> table;
            if (isTracking)
            {
                table = _userProfileChatRepository.Table;
            }
            else
            {
                table = _userProfileChatRepository.TableNoTracking;
            }

            return table.Where(userProfileChat => userProfileChat.UserProfileId == userProfileId)
                .Select(userProfileChat => userProfileChat.Chat)
                .OrderByDescending(chat => chat.LastMessageSendDate)
                .Take(chatCount);
        }

        public IEnumerable<Chat> GetAllUserProfileChats(int userProfileId, bool isTracking = false)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            IQueryable<UserProfileChat> table;
            if (isTracking)
            {
                table = _userProfileChatRepository.Table;
            }
            else
            {
                table = _userProfileChatRepository.TableNoTracking;
            }

            return table.Where(userProfileChat => userProfileChat.UserProfileId == userProfileId)
                .Select(userProfileChat => userProfileChat.Chat)
                .OrderByDescending(chat => chat.LastMessageSendDate);
        }

        public IEnumerable<UserProfile> GetChatParticipants(int chatId, bool isTracking = false)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            IQueryable<UserProfileChat> table;
            if (isTracking)
            {
                table = _userProfileChatRepository.Table;
            }
            else
            {
                table = _userProfileChatRepository.TableNoTracking;
            }

            return table.Where(userProfileChat => userProfileChat.ChatId == chatId)
                .Select(userProfileChat => userProfileChat.UserProfile);
        }

        public IEnumerable<int> GetChatParticipantIds(int chatId)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            return _userProfileChatRepository.TableNoTracking
                .Where(userProfileChat => userProfileChat.ChatId == chatId)
                .Select(userProfileChat => userProfileChat.UserProfileId);
        }

        public async Task AddChat(Chat chat)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat));

            await _chatRepository.Add(chat);
            await _unitOfWork.Commit();
        }

        public async Task UpdateChat(Chat chat)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat));

            _chatRepository.Update(chat);
            await _unitOfWork.Commit();
        }

        public async Task<bool> IsParticipant(int chatId, int userProfileId)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            return await _userProfileChatRepository.Any(userProfileChat =>
                userProfileChat.ChatId == chatId && userProfileChat.UserProfileId == userProfileId);
        }

        public async Task<bool> IsChatExists(int chatId)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            return await _chatRepository.Any(chat => chat.Id == chatId);
        }

        public bool IsChatNoGroupExists(int participantLeft, int participantRight)
        {
            if (participantLeft <= 0)
                throw new ArgumentException(nameof(participantLeft));

            if (participantRight <= 0)
                throw new ArgumentException(nameof(participantRight));

            return _userProfileChatRepository.TableNoTracking
                .Where(userProfileChat => userProfileChat.UserProfileId == participantLeft || userProfileChat.UserProfileId == participantRight)
                .GroupBy(userProfileChat => userProfileChat.ChatId)
                .Any(userProfileChatGroup => userProfileChatGroup.Count() == 2);
        }

        #endregion

        #region message

        public IEnumerable<Message> GetChatMessages(int chatId, DateTime? startLoadMessageDate, int messagesCount = 30, bool isTracking = false)
        {
            if (chatId <= 0)
                throw new ArgumentException(nameof(chatId));

            IQueryable<Message> table;
            if (isTracking)
            {
                table = _messageRepository.Table;
            }
            else
            {
                table = _messageRepository.TableNoTracking;
            }

            if (startLoadMessageDate != null)
            {
                return table.Where(message => message.ChatId == chatId && message.SendDate < startLoadMessageDate)
                    .OrderByDescending(message => message.SendDate)
                    .Take(messagesCount);
            }
            else
            {
                return table.Where(message => message.ChatId == chatId)
                    .OrderByDescending(message => message.SendDate)
                    .Take(messagesCount);
            }
        }

        public async Task AddMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await _messageRepository.Add(message);
            await _unitOfWork.Commit();
        }

        #endregion
    }
}
