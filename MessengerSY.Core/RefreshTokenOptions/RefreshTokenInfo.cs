using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.RefreshTokenOptions
{
    public class RefreshTokenInfo : IRefreshTokenInfo
    {
        public RefreshTokenInfo(int minimumSize, int lifeTimeDays, int extendTimeHours)
        {
            MinimumRefreshTokenSize = minimumSize;
            RefreshTokenLifeTimeDays = lifeTimeDays;
            ExtendTimeHours = extendTimeHours;
        }

        public int MinimumRefreshTokenSize { get; private set; }

        public int RefreshTokenLifeTimeDays { get; private set; }

        public int ExtendTimeHours { get; set; }
    }
}
