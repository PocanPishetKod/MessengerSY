using MessengerSY.Core;
using MessengerSY.Core.JwtAuthOptions;
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

        public JwtTokenService(IJwtInfo jwtInfo)
        {
            _jwtInfo = jwtInfo;
        }

        public string GenerateToken(string phoneNumber, int userProfileId, int refreshTokenId)
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
                    new Claim(HelpJwtConstants.REFRESH_TOKEN_ID, refreshTokenId.ToString())
                },
                signingCredentials: new SigningCredentials(_jwtInfo.SymmetricSecurityKey, _jwtInfo.SigningAlgorithm),
                expires: DateTime.Now.AddMinutes(_jwtInfo.JwtTokenLifeTimeMinutes)
                );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
