using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MessengerSY.Core.Domain
{
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Содержмое токена
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Дата окончания действия токена
        /// </summary>
        public DateTime Expires { get; set; }
        
        [NotMapped]
        public bool IsActive => DateTime.Now < Expires;

        /// <summary>
        /// Идентификатор владельца токена
        /// </summary>
        public int UserProfileId { get; set; }

        /// <summary>
        /// Владелец токена
        /// </summary>
        public virtual UserProfile UserProfile { get; set; }
    }
}
