using MessengerSY.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.ChatService
{
    public interface IChatService
    {
        Task<Chat> GetChatById(int chatId);
        IEnumerable<Chat> GetAllUserProfileChats(int userProfileId, bool isTracking = false);
        IEnumerable<Chat> GetUserProfileChats(int userProfileId, int chatCount = 20, bool isTracking = false);
        IEnumerable<UserProfile> GetChatParticipants(int chatId, bool isTracking = false);
        IEnumerable<int> GetChatParticipantIds(int chatId);
        Task AddChat(Chat chat);
        Task UpdateChat(Chat chat);
        Task<bool> IsParticipant(int chatId, int userProfileId);
        Task<bool> IsChatExists(int chatId);
        bool IsChatNoGroupExists(int participantLeft, int participantRight);


        IEnumerable<Message> GetChatMessages(int chatId, DateTime? startLoadMessageDate, int messagesCount = 30, bool isTracking = false);
        Task AddMessage(Message message);
        Task UpdateMessage(Message message);
        Task UpdateMessages(IEnumerable<Message> messages);
    }
}
