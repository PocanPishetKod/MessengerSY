using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MessengerSY.Core.JwtAuthOptions
{
    public class JwtInfo : IJwtInfo
    {
        private readonly SymmetricSecurityKey _key;

        public JwtInfo(string key)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public JwtInfo(string issuer, string audience, int jwtLifeTime, string key, string algorithm)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            Issuer = issuer;
            Audience = audience;
            JwtTokenLifeTimeMinutes = jwtLifeTime;
            SigningAlgorithm = algorithm;
        }

        public string Issuer { get; private set; }

        public string Audience { get; private set; }

        public int JwtTokenLifeTimeMinutes { get; private set; }

        public string SigningAlgorithm { get; private set; }

        public SymmetricSecurityKey SymmetricSecurityKey => _key;
    }
}
