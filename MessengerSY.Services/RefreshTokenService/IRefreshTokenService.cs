using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Services.RefreshTokenService
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken(int tokenSize = 32);
        bool CompareTokens(string leftToken, string rightToken);
        void BlockToken(int refreshTokenId);
        void BlockTokens(IEnumerable<int> refreshTokenIds);
        bool CheckBlockToken(int refreshTokenId);
    }
}
