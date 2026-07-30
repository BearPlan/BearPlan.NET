using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.IdGenerator;
using BearPlan.Core;
using BearPlan.EventBus.Weixin.MP;
using BearPlan.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.NeuChar.MessageHandlers;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities.Request;
using BearPlan.Common.Global;

namespace BearPlan.Api.Controllers.Auth
{

    /// <summary>
    ///  微信
    /// </summary>
    [Route("/api/[controller]/[action]")]
    public class WeiXinController : BaseApiController
    {

        public static readonly string _token = Senparc.Weixin.Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string _encodingAESKey = Senparc.Weixin.Config.SenparcWeixinSetting.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string _appId = Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string _appSecret = Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppSecret;

        //private readonly WinXinOptions _options;
        public WeiXinController()
        {
        }
        [HttpGet]
        [ActionName("Index")]
        [AllowAnonymous]
        [NotAudit]
        [NotFormatResponse]
        [ApiVersion((int)VersionEnum.Def, Deprecated = false)]
        public ActionResult Get([FromQuery] PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, _token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }
        /// <summary>
        /// 【异步方法】用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]

        [AllowAnonymous]
        [NotAudit]
        [NotFormatResponse]
        [ApiVersion((int)VersionEnum.Def, Deprecated = false)]
        public async Task<ActionResult> Post([FromQuery] PostModel postModel, [FromQuery] string appid)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _token))
            {
                return Content("参数错误！");
            }

            #region 打包 PostModel 信息

            postModel.Token = _token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = _encodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = _appId;//根据自己后台的设置保持一致（必须提供）

            #endregion
            //return Content("参数错误！");
            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制（实际最大限制 99999）
            //注意：如果使用分布式缓存，不建议此值设置过大，如果需要储存历史信息，请使用数据库储存
            var maxRecordCount = 10;

            // 1. 获取原始请求 Body（新版没有 GetRequestMemoryStream）
            Request.EnableBuffering();  // 必须启用，否则只能读一次
            var requestStream = new MemoryStream();
            await Request.Body.CopyToAsync(requestStream);
            requestStream.Position = 0;

            // 2. 创建自定义 MessageHandler（传递 IServiceProvider 以支持依赖注入）
            var messageHandler = CustomMessageHandler.GenerateMessageHandler(
                requestStream,
                postModel,
                maxRecordCount,
                HttpContext.RequestServices  // 传递 IServiceProvider
            );

            // 3. 消息去重（新版仍支持）
            //messageHandler.OmitRepeatedMessage = true;

            // 4. 同步优先策略
            messageHandler.DefaultMessageHandlerAsyncEvent = DefaultMessageHandlerAsyncEvent.DefaultResponseMessageAsync;

            // 5. 执行处理
            var ct = new CancellationToken();
            await messageHandler.ExecuteAsync(ct);

            // 6. 返回结果（新版不再使用 FixWeixinBugWeixinResult）
            string responseXml = messageHandler.ResponseDocument?.ToString()
                                 ?? messageHandler.TextResponseMessage;

            // 返回原始 XML 给微信（非常重要）
            return Content(responseXml, "text/xml");


        }
        /// <summary>
        /// 创建微信扫码登录二维码
        /// </summary>
        /// <remarks>
        /// 每次调用生成唯一 ticket（雪花 id）作为扫码会话标识：SSE 通道、登录凭证均以 ticket 隔离。
        /// 二维码场景值 sceneStr 编码为 "{version}_{ticket}"，扫码回调据此区分来源端并反查会话。
        /// 调用方拿到 { ticket, url } 后，用 ticket 建立 SSE 连接监听扫码状态。
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [NotAudit]
        [NotCors]
        [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
        public async Task<WeiXinQrCodeDTO> CreateQrcodeAsync(ApiVersion version)
        {
            // ticket = sceneStr：扫码回推的 EventKey 就是它，前端和回调用同一个值串起 SSE 通道与缓存键
            var ticket = $"{Enum.GetName(typeof(VersionEnum), version.MajorVersion)}_{IdHelper.NextId()}";
            var expireSeconds = 5000;
            var expire = TimeSpan.FromSeconds(expireSeconds);

            var res = await QrCodeApi.CreateAsync(
                Config.SenparcWeixinSetting.MpSetting.WeixinAppId, expireSeconds, 1, QrCode_ActionName.QR_STR_SCENE, ticket);
            if (res.errcode != ReturnCode.请求成功)
            {
                throw new BusException(res.errcode.ToString());
            }

            // 写入扫码状态为 waiting，过期时间与二维码有效期同步
            await App.Cache.SetAsync(
                GlobalConstants.CachePrefix.WeixinScanStatus + ticket,
                ScanLoginStatus.Waiting.ToString(),
                expire, CacheExpireType.Absolute);

            return new WeiXinQrCodeDTO { Ticket = ticket, Url = res.url };
        }


    }
}
