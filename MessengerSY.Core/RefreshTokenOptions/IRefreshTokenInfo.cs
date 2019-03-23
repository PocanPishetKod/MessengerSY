using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.RefreshTokenOptions
{
    public interface IRefreshTokenInfo
    {
        int MinimumRefreshTokenSize { get; }
        int RefreshTokenLifeTimeDays { get; }
        int ExtendTimeHours { get; }
    }
}
