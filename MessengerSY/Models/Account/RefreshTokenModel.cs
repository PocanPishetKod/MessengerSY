using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Account
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }

        public int UserProfileId { get; set; }
        public int RefreshTokenId { get; set; }
    }
}
