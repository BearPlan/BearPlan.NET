using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Model;
using BearPlan.Core;
using BearPlan.Models.Queries.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Common;

/// <summary>
/// 基控制器
/// </summary>
//[JsonParamter]
[FormatResponse]
[ApiController]

public class BaseController : ControllerBase
{



    /// <summary>
    /// 返回Json
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected ContentResult JsonContent(object obj)
    {
        return new ContentResult
        {
            Content = obj.ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = StatusCodes.Status200OK
        };
    }

}
