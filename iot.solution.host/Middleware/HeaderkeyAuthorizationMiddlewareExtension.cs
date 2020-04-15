﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Entity = iot.solution.entity;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using component.helper;

namespace host.iot.solution.Middleware
{
    public static class HeaderkeyAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseHeaderkeyAuthorization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HeaderkeyAuthorizationMiddleware>();
        }
    }

    public class HeaderkeyAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public HeaderkeyAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

                if (context.Request.Path.Value.Equals("/")
                    || context.Request.Path.Value.Contains("/api/account/FilePath")
                    || context.Request.Path.Value.Contains("/api/account/refreshtoken")
                    || context.Request.Path.Value.Contains("/api/subscriber")
                    || context.Request.Path.Value.Contains("/api/account/login")
                    || context.Request.Path.Value.StartsWith("/wwwroot/")
                    || context.Request.Path.Value.StartsWith("/api/alert/addiotalert")
                    || context.Request.Path.Value.Contains("/api/account/adminlogin")) // Nikunj
                {
                    await _next.Invoke(context);
                    return;
                }

                SolutionConfiguration.CompanyId = Guid.Parse(context.Request.Headers["company-id"]);
                if (SolutionConfiguration.CompanyId == null || SolutionConfiguration.CompanyId == Guid.Empty)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Header key is missing!");
                }

                if (context.User != null && context.User.Claims != null)
                {
                    SolutionConfiguration.BearerToken = context.User.Claims.Where(c => c.Type == "IOT_CONNECT").FirstOrDefault().Value;
                    if (!string.IsNullOrWhiteSpace(SolutionConfiguration.BearerToken) && SolutionConfiguration.BearerToken.Equals("AdminUser"))
                    {
                        SolutionConfiguration.CurrentUserId = Guid.Parse(context.User.Claims.Where(c => c.Type == "CURRENT_USERID").FirstOrDefault().Value);
                    }
                    else
                    {
                        JwtSecurityTokenHandler hand = new JwtSecurityTokenHandler();
                        var tokenS = hand.ReadJwtToken(SolutionConfiguration.BearerToken);
                        var jsonValue = tokenS.Claims?.SingleOrDefault(p => p.Type == "user")?.Value;
                        Entity.UserDetail userDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.UserDetail>(jsonValue);
                        SolutionConfiguration.CurrentUserId = Guid.Parse(userDetail.Id);
                        SolutionConfiguration.SolutionId = Guid.Parse(userDetail.SolutionGuid);
                        SolutionConfiguration.EntityGuid = Guid.Parse(userDetail.EntityGuid);
                    }
                }

                await _next.Invoke(context);
                return;

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
