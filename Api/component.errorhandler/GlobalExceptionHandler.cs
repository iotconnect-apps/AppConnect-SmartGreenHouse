using component.common.model;
using component.exception;
using component.logger;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace component.errorhandler
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public GlobalExceptionHandler(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await _logger.Error(ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }

        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            switch (exception)
            {
                case GenericCustomException genericException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new GenericError() { Message = genericException.Message }));
                case BadRequestCustomException badRequestCustomException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new GenericError() { Message = badRequestCustomException.Message }));
                case NotFoundCustomException notFoundException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new NotFoundError() { Message = notFoundException.Message }));
                case UnauthorizedCustomException unauthorizedCustomException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new NotFoundError() { Message = unauthorizedCustomException.Message }));
            }

            _logger.Error(exception.ToString());

            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new GenericError() { Message = "System Error" }));

        }
    }


}
