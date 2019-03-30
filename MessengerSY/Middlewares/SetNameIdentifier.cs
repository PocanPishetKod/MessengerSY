using MessengerSY.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MessengerSY.Middlewares
{
    public class SetNameIdentifier
    {
        private readonly RequestDelegate _requestDelegate;

        public SetNameIdentifier(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                user.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.GetUserProfileId().ToString()) }));
            }

            await _requestDelegate.Invoke(httpContext);
        }
    }
}
