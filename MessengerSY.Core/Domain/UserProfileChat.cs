using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class UserProfileChat : BaseEntity
    {
        /// <summary>
        /// Идентификатор участника чата
        /// </summary>
        public int UserProfileId { get; set; }

        /// <summary>
        /// Участник чата
        /// </summary>
        public virtual UserProfile UserProfile { get; set; }

        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Чат
        /// </summary>
        public virtual Chat Chat { get; set; }
    }
}
