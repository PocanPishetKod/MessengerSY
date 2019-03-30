using MessengerSY.Core;
using MessengerSY.Core.JwtAuthOptions;
using MessengerSY.Core.RefreshTokenOptions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MessengerSY.Services.RefreshTokenService
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenInfo _refreshTokenInfo;
        private readonly IJwtInfo _jwtInfo;
        private readonly IMemoryCache _memoryCache;

        public RefreshTokenService(IRefreshTokenInfo refreshTokenInfo, IJwtInfo jwtInfo, IMemoryCache memoryCache)
        {
            _refreshTokenInfo = refreshTokenInfo;
            _jwtInfo = jwtInfo;
            _memoryCache = memoryCache;
        }

        public string GenerateRefreshToken(int tokenSize = 32)
        {
            if (tokenSize < _refreshTokenInfo.MinimumRefreshTokenSize)
                tokenSize = 32;

            return StringGenerator.GenerateString(tokenSize);
        }

        public bool CompareTokens(string leftToken, string rightToken)
        {
            if (string.IsNullOrWhiteSpace(leftToken))
                throw new ArgumentNullException(nameof(leftToken));

            if (string.IsNullOrWhiteSpace(rightToken))
                throw new ArgumentNullException(nameof(rightToken));

            return string.Equals(leftToken, rightToken);
        }
    }
}
