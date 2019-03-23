using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class UserProfile : BaseEntity
    {
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Аватарка пользователя
        /// </summary>
        public byte[] Avatar { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Контакты, которые есть у пользователя
        /// </summary>
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        /// <summary>
        /// Пользователи, у которых текущий пользователь добавлен в контакты
        /// </summary>
        public virtual ICollection<Contact> AddedMe { get; set; } = new List<Contact>();

        /// <summary>
        /// Созданные чаты
        /// </summary>
        public virtual ICollection<Chat> CreatedChats { get; set; } = new List<Chat>();

        /// <summary>
        /// Чаты, в которых участвует пользователь
        /// </summary>
        public virtual ICollection<UserProfileChat> Chats { get; set; } = new List<UserProfileChat>();

        /// <summary>
        /// Отправленные сообщения
        /// </summary>
        public virtual ICollection<Message> SendMessages { get; set; } = new List<Message>();


        /// <summary>
        /// Токены, предназначенные для обновления access_token
        /// </summary>
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
