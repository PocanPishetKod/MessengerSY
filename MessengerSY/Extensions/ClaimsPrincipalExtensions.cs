using MessengerSY.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MessengerSY.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetRefreshTokenId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == HelpJwtConstants.REFRESH_TOKEN_ID);
            if (claim != null)
            {
                return Convert.ToInt32(claim.Value);
            }

            return 0;
        }

        public static int GetUserProfileId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == HelpJwtConstants.ID);
            if (claim != null)
            {
                return Convert.ToInt32(claim.Value);
            }

            return 0;
        }

        public static string GetUserProfilePhone(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == HelpJwtConstants.PHONE_NUMBER);
            if (claim != null)
            {
                return claim.Value;
            }

            return string.Empty;
        }
    }
}
