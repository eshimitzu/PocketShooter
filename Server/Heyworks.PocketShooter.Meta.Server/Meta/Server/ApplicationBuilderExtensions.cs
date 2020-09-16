using System.Net;
using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Heyworks.PocketShooter.Meta.Server
{
    internal static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature.Error;

                var result = JsonConvert.SerializeObject(new ResponseError
                {
                    Code = ApiErrorCode.InternalServerError,
                    Message = "An error occurred, please try again or contact the support.",
                });

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            return app;
        }
    }
}
