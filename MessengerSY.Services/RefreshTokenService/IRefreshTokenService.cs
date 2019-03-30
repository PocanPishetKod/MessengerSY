using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Services.RefreshTokenService
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken(int tokenSize = 32);
        bool CompareTokens(string leftToken, string rightToken);
    }
}
