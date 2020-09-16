using System;
using System.Linq;
using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Heyworks.PocketShooter.Meta.Server.ActionFilters
{
    public class CheckClientVersionFilter : IActionFilter
    {
        private readonly Type[] excludeControllerTypes = new[] { typeof(StatusController) };

        private readonly string[] acceptedClientVersions;

        public CheckClientVersionFilter(IConfiguration configuration)
        {
            this.acceptedClientVersions = configuration.GetSection("Meta:Front:AcceptedClientVersions").Get<string[]>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (excludeControllerTypes.Contains(context.Controller.GetType()))
            {
                return;
            }

            var request = context.HttpContext.Request;

            if (request.Headers.TryGetValue(RequestHeaders.ClientVersion, out var clientVersion))
            {
                // if client version is accepted.
                if (acceptedClientVersions.Any(item => item == clientVersion))
                {
                    return;
                }
            }

            context.Result = new ConflictObjectResult(
                new ResponseError(ApiErrorCode.InvalidClientVersion, "The client version is not supported."));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
