/*----------------------------------------------------------------
    Copyright (C) 2024 Senparc
    
    文件名：CustomMessageHandler_Events.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

//DPBMARK_FILE MP
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.Caches;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness.Core.System;
using Microsoft.Extensions.DependencyInjection;
using Senparc.NeuChar.Entities;
using Senparc.Weixin;
using Senparc.Weixin.MP.Entities;
using BearPlan.Common.Global;


namespace BearPlan.EventBus.Weixin.MP
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
       
        /// <summary>
        /// 获取微信服务
        /// </summary>
        private IUserWeiXinService _userWinXinService =>
            base.ServiceProvider?.GetService<IUserWeiXinService>();

        /// <summary>
        /// 获取微信公众号 AppId
        /// </summary>
        private string _appId => Config.SenparcWeixinSetting.MpSetting.WeixinAppId;
        public override async Task<IResponseMessageBase> OnTextOrEventRequestAsync(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

            if (requestMessage.Content == "OneClick")
            {
                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                return strongResponseMessage;
            }
            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ClickRequestAsync(RequestMessageEvent_Click requestMessage)
        {
            var reponseMessage = CreateResponseMessage<ResponseMessageText>();

            if (requestMessage.EventKey == "OneClick")
            {
                reponseMessage.Content = "您点击了【单击测试】按钮";
            }
            else
            {
                reponseMessage.Content = "您点击了其他事件按钮";
            }

            return reponseMessage;
        }

        /// <summary>
        /// 进入事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_EnterRequestAsync(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        /// <summary>
        /// 位置事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_LocationRequestAsync(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        /// <summary>
        /// 通过二维码扫描关注扫描事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ScanRequestAsync(RequestMessageEvent_Scan requestMessage)
        {
      
            //通过扫描关注
            var responseMessage = CreateResponseMessage<ResponseMessageText>();

            var any = await _userWinXinService.GetIQueryable(x => x.OpenId == requestMessage.FromUserName).AnyAsync();
            if (any)
            {
                await _userWinXinService.UpdateAsync(x => x.OpenId == requestMessage.FromUserName, x => new UserWeiXin { Subscribe = 1, SubscribeTime = DateTime.Now.ToUnixTimeStampSecond() });
            }
            else
            {

                var userInfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(_appId, requestMessage.FromUserName, Language.zh_CN);
                var model = new UserWeiXin
                {
                    OpenId = userInfo.openid,
                    Subscribe = userInfo.subscribe,
                    AvatarUrl = userInfo.headimgurl,
                    NickName = userInfo.nickname,
                    SubscribeTime = userInfo.subscribe_time,
                    UnionId = userInfo.unionid,
                    CreateTime = DateTime.Now,
                    AppId = _appId,
                    Source = requestMessage.EventKey
                };
                await _userWinXinService.AddAsync(model);
                if (!userInfo.unionid.IsNullOrEmpty())
                {
                    var userId = await _userWinXinService.GetIQueryable(x => x.UnionId == userInfo.unionid && x.UserId != null).Select(x => x.UserId).FirstAsync();
                    if (!userId.IsNullOrEmpty())
                        await _userWinXinService.UpdateAsync(x => x.UnionId == userInfo.unionid && x.UserId == null, x => new UserWeiXin { UserId = userId });
                }

            }

            // 触发扫码登录状态回推（EventKey 即二维码场景值 ticket），通知 SSE 端点推送状态
            await HandleScanLoginEventAsync(requestMessage.EventKey, requestMessage.FromUserName);

            responseMessage.Content = responseMessage.Content ?? string.Format("通过扫描二维码进入，场景值：{0}", requestMessage.EventKey);

            return responseMessage;
        }

        /// <summary>
        /// 打开网页事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ViewRequestAsync(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        /// <summary>
        /// 群发完成事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_MassSendJobFinishRequestAsync(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_SubscribeRequestAsync(RequestMessageEvent_Subscribe requestMessage)
        {

            var any = await _userWinXinService.GetIQueryable(x => x.OpenId == requestMessage.FromUserName).AnyAsync();
            if (any)
            {
                await _userWinXinService.UpdateAsync(x => x.OpenId == requestMessage.FromUserName, x=> new UserWeiXin { Subscribe = 1, SubscribeTime = DateTime.Now.ToUnixTimeStampSecond() });
            }
            else
            {

                var userInfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(_appId, requestMessage.FromUserName, Language.zh_CN);
                var model = new UserWeiXin
                {
                    OpenId = userInfo.openid,
                    Subscribe = userInfo.subscribe,
                    AvatarUrl= userInfo.headimgurl,
                    NickName = userInfo.nickname,
                    SubscribeTime = userInfo.subscribe_time,
                    UnionId = userInfo.unionid,
                    CreateTime = DateTime.Now,
                    AppId = _appId,
                };
                await _userWinXinService.AddAsync(model);
                if (!userInfo.unionid.IsNullOrEmpty()) {
                    var userId =await _userWinXinService.GetIQueryable(x => x.UnionId == userInfo.unionid && x.UserId != null).Select(x => x.UserId).FirstAsync();
                    if(!userId.IsNullOrEmpty())
                    await _userWinXinService.UpdateAsync(x=>x.UnionId== userInfo.unionid && x.UserId == null, x=> new UserWeiXin {  UserId = userId });
                }

            }
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                // 首次关注扫码场景，EventKey 形如 qrscene_{ticket}，触发扫码登录状态回推
                await HandleScanLoginEventAsync(requestMessage.EventKey, requestMessage.FromUserName);
                responseMessage.Content += "\r\n============\r\n场景值：" + requestMessage.EventKey;
            }
            var defaultResponseMessage = base.CreateResponseMessage<ResponseMessageText>();
            defaultResponseMessage.Content = @"感谢您的关注" + requestMessage.FromUserName;
            return defaultResponseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_UnsubscribeRequestAsync(RequestMessageEvent_Unsubscribe requestMessage)
        {
          
           await _userWinXinService.UpdateAsync(x => x.OpenId == requestMessage.FromUserName, x => new UserWeiXin { Subscribe = 0, SubscribeTime = DateTime.Now.ToUnixTimeStampSecond() });
         

          
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ScancodePushRequestAsync(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ScancodeWaitmsgRequestAsync(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_PicPhotoOrAlbumRequestAsync(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_PicSysphotoRequestAsync(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_PicWeixinRequestAsync(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_LocationSelectRequestAsync(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }

        #region 微信认证事件推送

        public override async Task<IResponseMessageBase> OnEvent_QualificationVerifySuccessRequestAsync(RequestMessageEvent_QualificationVerifySuccess requestMessage)
        {
            //以下方法可以强制定义返回的字符串值
            //TextResponseMessage = "your content";
            //return null;

            return new SuccessResponseMessage();//返回"success"字符串
        }

        #endregion

        #region 扫码登录

        /// <summary>
        /// Redis 订阅能力，用于扫码后向 SSE 端点跨实例推送状态
        /// </summary>
        private IRedisSubscriber _redisSubscriber =>
            base.ServiceProvider?.GetService<IRedisSubscriber>();

        /// <summary>
        /// 处理扫码登录：按 OpenId/UnionId 查询绑定的系统用户，更新扫码状态并通过 Redis 通知 SSE 端点
        /// </summary>
        /// <param name="ticket">扫码会话标识（即二维码场景值 EventKey，前端 SSE 通道与缓存键均用它）</param>
        /// <param name="openId">扫码者的微信 OpenId</param>
        private async Task NotifyScanLoginAsync(string ticket, string openId)
        {
            if (string.IsNullOrEmpty(ticket) || _redisSubscriber == null)
            {
                return;
            }

            // 按 OpenId 查询已绑定的微信记录，拿到系统 UserId
            var weixinUser = await _userWinXinService
                .GetIQueryable(x => x.OpenId == openId)
                .Select(x => new { x.UserId, x.UnionId })
                .FirstAsync();

            // 未绑定系统用户时，按 UnionId 兜底关联（跨应用场景）
            if (weixinUser?.UserId == null && !string.IsNullOrEmpty(weixinUser?.UnionId))
            {
                var userId = await _userWinXinService
                    .GetIQueryable(x => x.UnionId == weixinUser.UnionId && x.UserId != null)
                    .Select(x => x.UserId)
                    .FirstAsync();
                if (userId != null)
                {
                    weixinUser = new { UserId = userId, weixinUser.UnionId };
                }
            }

            var status = weixinUser?.UserId != null
                ? ScanLoginStatus.Confirmed
                : ScanLoginStatus.Unbound;

            var statusKey = GlobalConstants.CachePrefix.WeixinScanStatus + ticket;
            await App.Cache.SetAsync(statusKey, status.ToString(),
                TimeSpan.FromSeconds(5000), CacheExpireType.Absolute);

            // 已绑定：写入一次性登录凭证，供浏览器换取 Token
            if (weixinUser?.UserId != null)
            {
                var loginTicket = new WeixinScanLoginTicket
                {
                    UserId = weixinUser.UserId.Value,
                    ApiVersion = VersionEnum.Pc
                };
                await App.Cache.SetAsync(
                    GlobalConstants.CachePrefix.WeixinScanLogin + ticket,
                    loginTicket,
                    TimeSpan.FromSeconds(5000), CacheExpireType.Absolute);
            }

            // 通过 Redis Pub/Sub 通知 SSE 端点推送状态变更（跨实例生效）
            await _redisSubscriber.PublishAsync(
                GlobalConstants.CachePrefix.WeixinScanNotify + ticket,
                status.ToString());
        }

        /// <summary>
        /// 统一的扫码事件入口，处理扫码登录逻辑后返回微信响应消息
        /// </summary>
        private async Task HandleScanLoginEventAsync(string ticket, string openId)
        {
            await NotifyScanLoginAsync(ticket, openId);
        }

        #endregion
    }
}
