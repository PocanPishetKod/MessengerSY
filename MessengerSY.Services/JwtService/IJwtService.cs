using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateToken(string phoneNumber, int userProfileId, int refreshTokenId);
    }
}
