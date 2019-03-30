using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class Contact : BaseEntity
    {
        /// <summary>
        /// Имя, которое указано в записной книжке на телефоне пользователя
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// Номер контакта. Устанавливается когда контакт не зарегистрирован, но есть в телефонной книжке
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Идентификатор владельца контакта
        /// </summary>
        public int ContactOwnerId { get; set; }

        /// <summary>
        /// Владелец контакта
        /// </summary>
        public virtual UserProfile ContactOwner { get; set; }

        /// <summary>
        /// Идентификатор связанного с контактом пользователя
        /// </summary>
        public int? LinkedUserProfileId { get; set; }

        /// <summary>
        /// Пользователь, связанный с контактом
        /// </summary>
        public virtual UserProfile LinkedUserProfile { get; set; }
    }
}
