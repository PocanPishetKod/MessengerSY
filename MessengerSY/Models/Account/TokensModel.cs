using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Account
{
    public class TokensModel
    {
        public string Jwt { get; set; }
        public RefreshTokenModel RefreshToken { get; set; }
    }
}
