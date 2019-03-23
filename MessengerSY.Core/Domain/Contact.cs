using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class Contact : BaseEntity
    {
        /// <summary>
        /// Идентификатор владельца контакта
        /// </summary>
        public int FirstSideId { get; set; }

        /// <summary>
        /// Владелец контакта
        /// </summary>
        public virtual UserProfile FirstSide { get; set; }

        /// <summary>
        /// Идентификатор связанного с контактом пользователя
        /// </summary>
        public int SecondSideId { get; set; }

        /// <summary>
        /// Пользователь, связанный с контактом
        /// </summary>
        public virtual UserProfile SecondSide { get; set; }
    }
}
