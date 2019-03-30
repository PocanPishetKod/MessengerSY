using MessengerSY.Core;
using MessengerSY.Core.JwtAuthOptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MessengerSY.Services.JwtService
{
    public class JwtTokenService : IJwtService
    {
        private readonly IJwtInfo _jwtInfo;
        private readonly IMemoryCache _memoryCache;

        private readonly string _blockTokenKeyPrefix = "blocktoken";

        public JwtTokenService(IJwtInfo jwtInfo, IMemoryCache memoryCache)
        {
            _jwtInfo = jwtInfo;
            _memoryCache = memoryCache;
        }

        public string GenerateToken(string phoneNumber, int userProfileId, string nickname, int refreshTokenId)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            if (refreshTokenId <= 0)
                throw new ArgumentException(nameof(refreshTokenId));

            var token = new JwtSecurityToken(
                issuer: _jwtInfo.Issuer,
                audience: _jwtInfo.Audience,
                claims: new Claim[]
                {
                    new Claim(HelpJwtConstants.PHONE_NUMBER, phoneNumber),
                    new Claim(HelpJwtConstants.ID, userProfileId.ToString()),
                    new Claim(HelpJwtConstants.REFRESH_TOKEN_ID, refreshTokenId.ToString()),
                    new Claim(HelpJwtConstants.NICKNAME, nickname)
                },
                signingCredentials: new SigningCredentials(_jwtInfo.SymmetricSecurityKey, _jwtInfo.SigningAlgorithm),
                expires: DateTime.Now.AddMinutes(_jwtInfo.JwtTokenLifeTimeMinutes)
                );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void BlockToken(int refreshTokenId)
        {
            if (refreshTokenId <= 0)
                throw new ArgumentException(nameof(refreshTokenId));

            _memoryCache.Set(BuildValue(refreshTokenId), 1, TimeSpan.FromMinutes(_jwtInfo.JwtTokenLifeTimeMinutes));
        }

        public void BlockTokens(IEnumerable<int> refreshTokenIds)
        {
            if (refreshTokenIds == null)
                throw new ArgumentNullException(nameof(refreshTokenIds));

            foreach (var refreshTokenId in refreshTokenIds)
            {
                BlockToken(refreshTokenId);
            }
        }

        public bool CheckBlockToken(int refreshTokenId)
        {
            if (refreshTokenId <= 0)
                throw new ArgumentException(nameof(refreshTokenId));

            return _memoryCache.TryGetValue(BuildValue(refreshTokenId), out var value);
        }

        private string BuildValue(int refreshTokenId)
        {
            return _blockTokenKeyPrefix + refreshTokenId.ToString();
        }
    }
}
