using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Entity.ConfigProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.ConfigProvider
{
    /// <remarks>
    ///  表单列表
    /// </remarks>
    /// <param name="logic"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [Route("/api/[controller]/[action]")]
    [AllowAnonymous]

    public class TableFormController(ITableFormService service) : BaseApiController
    {
        private readonly ITableFormService _service = service ?? throw new ArgumentNullException(nameof(service));

        #region 表头信息
        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0", Deprecated = false)]
        public async Task<TableForm> GetEditAsync([FromQuery] TableFormEditParam param) => await _service.GetEditAsync(param);

        /// <summary>
        /// 编辑模型
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0", Deprecated = false)]
        public async Task<long> SetEditAsync(UpdateTableFormParam param)=>await _service.SetEditAsync(param);
        #endregion

        #region 表头信息获取
        /// <summary>
        /// 表头信息获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0", Deprecated = false)]
        public async Task<TableForm> GetViewAsync([FromQuery] TableFormEditParam param) => await _service.GetViewAsync(param);


        #endregion
    }
}
