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

        private readonly string _blockTokenKeyPrefix = "blocktoken";

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
