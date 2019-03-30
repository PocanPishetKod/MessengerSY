using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class UserProfileModel
    {
        public int UserProfileId { get; set; }
        public string PhoneNumber { get; set; }
        public string Nickname { get; set; }
    }
}
