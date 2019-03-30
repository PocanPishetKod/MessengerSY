using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessengerSY.Core.Domain;
using MessengerSY.Extensions;
using MessengerSY.Models.Chat;
using MessengerSY.Services.ChatService;
using MessengerSY.Services.OnlineStatusService;
using MessengerSY.Services.UserProfileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerSY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatSrevice;
        private readonly IOnlineStatusService _onlineStatusService;

        public ChatController(IChatService chatSrevice, IOnlineStatusService onlineStatusService)
        {
            _chatSrevice = chatSrevice;
            _onlineStatusService = onlineStatusService;
        }

        [HttpGet("getchats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserProfileChats()
        {
            var chats = _chatSrevice.GetAllUserProfileChats(User.GetUserProfileId());

            var chatList = new List<ChatModel>(chats.Count());
            foreach (var chat in chats)
            {
                var chatModel = new ChatModel()
                {
                    IsGroup = chat.IsGroup,
                    ChatId = chat.Id,
                    Title = chat.Title
                };

                var chatParticipants = _chatSrevice.GetChatParticipants(chat.Id);
                chatModel.Participants = chatParticipants.Select(chatParticipant => new UserProfileModel()
                {
                    UserProfileId = chatParticipant.Id,
                    Nickname = chatParticipant.Nickname,
                    PhoneNumber = chatParticipant.PhoneNumber
                });

                chatModel.LastMessage = new LastMessageModel()
                {
                    TextContent = chat.LastMessage.MessageText
                };

                var sender = chatParticipants.First(chatParticipant => chatParticipant.Id == chat.LastMessage.SenderId);

                chatModel.LastMessage.Sender = new UserProfileModel()
                {
                    UserProfileId = sender.Id,
                    Nickname = sender.Nickname,
                    PhoneNumber = sender.PhoneNumber
                };
            }

            return Ok(new ChatListModel()
            {
                Chats = chatList
            });
        }

        [HttpGet("getchatmessages/{chatid}/{count}/{startDate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetChatMessages(int chatId, int count, DateTime? startDate)
        {
            if (await _chatSrevice.IsParticipant(chatId, User.GetUserProfileId()))
            {
                var messages = _chatSrevice.GetChatMessages(chatId, startDate, count);

                return Ok(new MessageListModel()
                {
                    Messages = messages.Select(message => new MessageModel()
                    {

                        MessageId = message.Id,
                        SendDate = message.SendDate,
                        TextContent = message.MessageText,
                        Sender = new UserProfileModel()
                        {
                            UserProfileId = message.SenderId,
                            Nickname = message.Sender.Nickname,
                            PhoneNumber = message.Sender.PhoneNumber
                        }
                    })
                });
            }

            return Forbid();
        }
    }
}