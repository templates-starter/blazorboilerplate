﻿using BlazorBoilerplate.Infrastructure.Server.Models;
using BlazorBoilerplate.Shared;
using BlazorBoilerplate.Shared.Interfaces;
using Finbuckle.MultiTenant;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorBoilerplate.Server.Middleware
{
    public class UserSessionMiddleware
    {
        private ILogger<UserSessionMiddleware> _logger;

        //https://trailheadtechnology.com/aspnetcore-multi-tenant-tips-and-tricks/
        private readonly RequestDelegate _next;

        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<UserSessionMiddleware> logger, IUserSession userSession)
        {
            _logger = logger;
            try
            {
                var request = httpContext.Request;

                //First setup the userSession, then call next midleware
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    userSession.UserId = new Guid(httpContext.User.Claims.Where(c => c.Type == JwtClaimTypes.Subject).First().Value);
                    userSession.UserName = httpContext.User.Identity.Name;
                    userSession.TenantId = httpContext.GetMultiTenantContext().TenantInfo.Identifier;
                    userSession.Roles = httpContext.User.Claims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value).ToList();

                    if (userSession.Roles.Contains(DefaultRoleNames.Administrator))
                        userSession.DisableTenantFilter = true;
                }

                // Call the next delegate/middleware in the pipeline
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                // We can't do anything if the response has already started, just abort.
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("A Middleware exception occurred, but response has already started!");
                    throw;
                }

                await HandleExceptionAsync(httpContext, ex);
                throw;
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, System.Exception exception)
        {
            _logger.LogError("Api Exception:", exception);

            ApiError apiError = null;
            ApiResponse apiResponse = null;
            int code = 0;

            if (exception is ApiException)
            {
                var ex = exception as ApiException;
                apiError = new ApiError(ResponseMessage.GetDescription(ex.StatusCode), ex.Errors)
                {
                    ValidationErrors = ex.Errors,
                    ReferenceErrorCode = ex.ReferenceErrorCode,
                    ReferenceDocumentLink = ex.ReferenceDocumentLink
                };
                code = ex.StatusCode;
                httpContext.Response.StatusCode = code;
            }
            else if (exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                code = Status401Unauthorized;
                httpContext.Response.StatusCode = code;
            }
            else
            {
//-:cnd:noEmit
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
                var msg = exception.GetBaseException().Message;
                string stack = exception.StackTrace;
#endif
//-:cnd:noEmit
                apiError = new ApiError(msg)
                {
                    Details = stack
                };
                code = Status500InternalServerError;
                httpContext.Response.StatusCode = code;
            }

            httpContext.Response.ContentType = "application/json";

            apiResponse = new ApiResponse(code, ResponseMessage.GetDescription(code), null, apiError);

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
        }
    }
}