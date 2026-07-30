using System.Threading.Tasks;
using BearPlan.Entity;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models;

namespace BearPlan.IBusiness.ConfigProvider
{
    public interface ITableViewService : IBaseServices<TableView>
    {

        #region 表头信息设置
        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<TableViewInfo> GetEditAsync(TableViewEditParam param);



        /// <summary>
        /// 编辑模型
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
       Task<long> SetEditAsync(UpdateTableViewParam param);


        #endregion

        #region 信息获取
        /// <summary>
        /// 表头信息获取
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<TableView> GetViewAsync(TableViewEditParam param);
        #endregion
    }
}
