using MessengerSY.Core.Domain;
using MessengerSY.Extensions;
using MessengerSY.Hubs.ClientMethods;
using MessengerSY.Models.Chat;
using MessengerSY.Services.ChatService;
using MessengerSY.Services.OnlineStatusService;
using MessengerSY.Services.UserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Hubs
{
    [Authorize]
    public class Main : Hub<IMainClientMethods>
    {
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly IChatService _chatSrevice;
        private readonly IHubContext<Notification> _notificationHub;
        private readonly IUserProfileService _userProfileService;

        public Main(IChatService chatSrevice, IOnlineStatusService onlineStatusService, IHubContext<Notification> notificationHub, IUserProfileService userProfileService)
        {
            _chatSrevice = chatSrevice;
            _onlineStatusService = onlineStatusService;
            _notificationHub = notificationHub;
            _userProfileService = userProfileService;
        }

        public async override Task OnConnectedAsync()
        {
            await _onlineStatusService.SetOnlineStatus(Context.User.GetUserProfileId());
            
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await _onlineStatusService.SetOfflineStatus(Context.User.GetUserProfileId());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int chatId, string messageText)
        {
            if (chatId > 0 && !string.IsNullOrWhiteSpace(messageText))
            {
                var chat = await _chatSrevice.GetChatById(chatId);
                if (chat != null && await _chatSrevice.IsParticipant(chatId, Context.User.GetUserProfileId()))
                {
                    var message = new Message()
                    {
                        ChatId = chat.Id,
                        SenderId = Context.User.GetUserProfileId(),
                        ThisMessageLastInChatId = chat.Id,
                        SendDate = DateTime.Now,
                        MessageText = messageText
                    };

                    await _chatSrevice.AddMessage(message);

                    chat.LastMessageSendDate = message.SendDate;
                    await _chatSrevice.UpdateChat(chat);

                    var participantIds = _chatSrevice.GetChatParticipantIds(chat.Id);

                    var onlineUserProfileIds = new List<string>();
                    var offlineUserProfileIds = new List<string>();
                    foreach (var participantId in participantIds)
                    {
                        if (await _onlineStatusService.IsOnline(participantId))
                        {
                            onlineUserProfileIds.Add(participantId.ToString());
                        }
                        else
                        {
                            offlineUserProfileIds.Add(participantId.ToString());
                        }
                    }

                    var messageModel = new MessageModel()
                    {
                        MessageId = message.Id,
                        SendDate = message.SendDate,
                        TextContent = message.MessageText,
                        Sender = new UserProfileModel()
                        {
                            UserProfileId = Context.User.GetUserProfileId(),
                            PhoneNumber = Context.User.GetUserProfilePhone()
                        }
                    };

                    await Clients.Users(onlineUserProfileIds).ReceiveMessage(messageModel);
                    await _notificationHub.Clients.Users(offlineUserProfileIds).SendAsync("ReceiveMessage", messageModel);
                }
            }
        }

        public async Task CreateChatWithMessage(int participantId, string messageText)
        {
            if (participantId > 0 && !string.IsNullOrWhiteSpace(messageText))
            {
                if (await _userProfileService.IsUserProfileExists(participantId))
                {
                    if (!_chatSrevice.IsChatNoGroupExists(participantId, Context.User.GetUserProfileId()))
                    {
                        var newChat = new Chat()
                        {
                            CreatorId = Context.User.GetUserProfileId(),
                            CreationDate = DateTime.Now
                        };

                        var message = new Message()
                        {
                            MessageText = messageText,
                            SendDate = DateTime.Now,
                            SenderId = Context.User.GetUserProfileId()
                        };

                        newChat.Messages.Add(message);
                        newChat.LastMessage = message;
                        newChat.LastMessageSendDate = message.SendDate;
                        newChat.Participants.Add(new UserProfileChat()
                        {
                            UserProfileId = Context.User.GetUserProfileId()
                        });
                        newChat.Participants.Add(new UserProfileChat()
                        {
                            UserProfileId = participantId
                        });

                        await _chatSrevice.AddChat(newChat);
                    }
                }
            }
        }
    }
}
