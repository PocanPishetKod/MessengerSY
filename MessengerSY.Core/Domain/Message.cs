using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class Message : BaseEntity
    {
        /// <summary>
        /// Дата отправления сообщения
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Идентификатор отправителся сообщения
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public virtual UserProfile Sender { get; set; }

        /// <summary>
        /// Идентификатор чата, к которому привязано сообщение
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Чат, к которому привязано сообщение
        /// </summary>
        public virtual Chat Chat { get; set; }

        /// <summary>
        /// Идентфикатор чата, в котором это сообщение последнее
        /// </summary>
        public int? ThisMessageLastInChatId { get; set; }
        /// <summary>
        /// Чат, в котором это сообщение последнее
        /// </summary>
        public virtual Chat ThisMessageLastInChat { get; set; }
    }
}
