using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Test;
using BearPlan.Models.Common;

namespace BearPlan.Models.Test
{
    /// <summary>
    /// 测试订单
    /// </summary>
    #region 查询参数
    public class TestOrderParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(TestOrderDTO), typeof(TestOrder))]
    public class TestOrderDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(TestOrderInfo), typeof(TestOrder))]
    public class TestOrderInfo : TestOrder
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateTestOrderParam), typeof(TestOrder))]
    public class UpdateTestOrderParam : TestOrder
    {
    }
    #endregion
}
