using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class Chat : BaseEntity
    {
        /// <summary>
        /// Дата создания чата
        /// </summary>
        public string CreationDate { get; set; }

        /// <summary>
        /// Название чата. Доступно если чат является группой
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Показывает является ли чат группой
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Показывает является ли чат секретным
        /// </summary>
        public bool IsSecret { get; set; }

        /// <summary>
        /// Идентификатор создателя чата
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Создатель чата
        /// </summary>
        public virtual UserProfile Creator { get; set; }

        /// <summary>
        /// Участники чата
        /// </summary>
        public virtual ICollection<UserProfileChat> Participants { get; set; } = new List<UserProfileChat>();

        /// <summary>
        /// Сообщения чата
        /// </summary>
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
