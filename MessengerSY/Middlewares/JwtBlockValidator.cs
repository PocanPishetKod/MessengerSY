using MessengerSY.Extensions;
using MessengerSY.Services.JwtService;
using MessengerSY.Services.RefreshTokenService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Middlewares
{
    public class JwtBlockValidator
    {
        private readonly RequestDelegate _requestDelegate;

        public JwtBlockValidator(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext, IRefreshTokenService refreshTokenService)
        {
            if (httpContext?.User?.Identity.IsAuthenticated ?? false)
            {
                var refreshTokenId = httpContext.User.GetRefreshTokenId();
                var result = refreshTokenService.CheckBlockToken(refreshTokenId);
                if (result)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    await _requestDelegate.Invoke(httpContext);
                }
            }
            else
            {
                await _requestDelegate.Invoke(httpContext);
            }
        }
    }
}
