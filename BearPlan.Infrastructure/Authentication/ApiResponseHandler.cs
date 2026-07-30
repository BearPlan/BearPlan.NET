using System.Text.Encodings.Web;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Model;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BearPlan.Common.Global;

namespace BearPlan.Infrastructure.Authentication;

public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) :
        base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Response.WriteAsync(new ActionResultVm
        {
            Status = StatusCodes.Status401Unauthorized,
            ActionError = new ActionError(),
            Message = Language.Sys_HttpUnauthorized,
            Path = App.HttpContext?.Request.Path.Value?.ToLower()
        }.ToJson());
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var loginUserInfo = await App.Cache.GetAsync<LoginUserInfo>(
            GlobalConstants.CachePrefix.OnlineKey +
            App.GetService<IHttpUser>().JwtToken.ToMd5String16());
        if (loginUserInfo.IsNull())
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status401Unauthorized,
                ActionError = new ActionError(),
                Message = Language.Sys_HttpUnauthorized,
                Path = App.HttpContext?.Request.Path.Value?.ToLower()
            }.ToJson());
        }
        else
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status403Forbidden,
                ActionError = new ActionError(),
                Message = Language.Sys_HttpForbidden,
                Path = App.HttpContext?.Request.Path.Value?.ToLower()
            }.ToJson());
        }
    }
}
