using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.JwtAuthOptions
{
    public interface IJwtInfo
    {
        string Issuer { get; }
        string Audience { get; }
        int JwtTokenLifeTimeMinutes { get; }
        string SigningAlgorithm { get; }
        SymmetricSecurityKey SymmetricSecurityKey { get; }
    }
}
