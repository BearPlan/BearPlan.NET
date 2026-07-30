using System.Data;
using System.Reflection;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Repository.UnitOfWork;
using Mapster;
using Newtonsoft.Json;
using SqlSugar;

namespace BearPlan.Business.ConfigProvider
{
    /// <summary>
    ///  数据表
    /// </summary>
    public class TableFormService : BaseServices<TableForm>, ITableFormService
    {

        public TableFormService()
        {
        }


        #region 表头信息设置
        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<TableForm> GetEditAsync(TableFormEditParam param)
        {
            param.Tableof = param.Tableof.Trim();
            param.Router = (param.Router ?? "").Trim();
            var sysList = new List<FormComponent>();
            if (!param.ConfigId.IsNullOrEmpty())
            {
                sysList = await GetTableColumnAsync(param.Tableof, param.ConfigId);
            }
            else
            {

                sysList = GetXml(param.Tableof);
            }

            //获取自定义字段
            var entity = await GetIQueryable(x => x.Tableof == param.Tableof && x.Router == param.Router).Select<TableForm>().FirstAsync();

            if (entity == null)
            {
                entity = new TableForm()
                {
                    Tableof = param.Tableof,
                    Router = param.Router,
                    ConfigId = param.ConfigId,
                    Components = new List<FormComponent>(),
                };
            }


            var keys1 = entity?.Components?.Select(x => x.Prop).ToList() ?? new List<string>();
            var keys2 = sysList.Select(x => x.Prop).ToList();
            var keys = keys1.Union(keys2).Distinct().ToList();
            var list = new List<FormComponent>();
            int index = 0;
            foreach (var item in keys)
            {
                var model = entity?.Components?.FirstOrDefault(x => x.Prop == item);
                if (model != null)
                {
                    entity.Components.Remove(model);
                }

                model ??= sysList.FirstOrDefault(x => x.Prop == item);
                model.Sort = index++;
                list.Add(model);
            }
            entity.Components = list;
            return entity;
        }
        /// <summary>
        /// 编辑模型
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [UseTran]
        public async Task<long> SetEditAsync(UpdateTableFormParam param)
        {
            var model = App.Mapper.MapTo<TableForm>(param);
            model.Components = model.Components.Where(x => !x.IsEditDel).ToList();
            if (model.Id == 0)
            {
                await AddAsync(model);
            }
            else
            {
                await UpdateAsync(model);
            }

            return model.Id;
        }
        #endregion





        #region 信息获取
        /// <summary>
        /// 表头信息获取
        /// </summary>
        /// <returns></returns>
        public async Task<TableForm> GetViewAsync(TableFormEditParam param)
        {
            var entity = await GetIQueryable(x => x.Tableof == param.Tableof).FirstAsync();
            return entity;
        }
        #endregion
        #region 私有方法

        /// <summary>
        /// 反射中找到XML
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<FormComponent> GetXml(string tableName)
        {


            var xmlCommentHelper = new XmlCommentHelper();
            //var xmlFile = AppDomain.CurrentDomain.BaseDirectory + typeName + ".xml";
            //"E:\\MyCode\\LY_WMSCloud\\LY_WMSCloud.Business\\bin\\Debug\\net6.0\\LY_WMSCloud.Models.xml"
            //xmlCommentHelper.Load(new string[] { xmlFile });
            xmlCommentHelper.LoadAll();
            //type
            //var path = $"LY_WMSCloud.Models.{model}";
            //Type type= Type.GetType(path);

            //Assembly assIBll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/" + typeName + ".dll");
            ////加载dll后,需要使用dll中某类.
            //Type type = assIBll.GetType($"{typeName}.{tableName}");//获取类名，必须 命名空间+类名

            Type type = null;
            IList<Assembly> assemblys = RuntimeHelper.GetAllAssemblies();
            foreach (var assembly in assemblys)
            {
                var aa = assembly.GetTypes();
                type = assembly.GetTypes().Where(x => x.Name == tableName).FirstOrDefault();
                if (type != null)
                {
                    break;
                }

            }
            var props = type.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null).ToArray();


            //entity.Comment = xmlCommentHelper.GetComment($"T:{type.FullName}", "summary");
            var list = new List<FormComponent>();
            for (int i = 0; i < props.Length; i++)
            {
                MemberInfo prop = props[i];
                var common = xmlCommentHelper.GetFieldOrPropertyComment(prop);
                var model = new FormComponent()
                {
                    Label = common.Trim(),
                    Prop = prop.Name?.ToFirstLowerStr(),//转小写,
                    IsEditDel = true,
                };
                list.Add(model);
            }

            return list;
        }

        /// <summary>
        ///  获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private async Task<List<FormComponent>> GetTableColumnAsync(string tableName, string configId)
        {
            App.GetService<IUnitOfWork>().GetDbClient().GetConnection(configId).DbMaintenance.GetColumnInfosByTableName(tableName, false);

            var columnView = SugarClient.AsTenant().GetConnection(configId).DbMaintenance.GetColumnInfosByTableName(tableName, false);//true 走缓存 false不走缓存
            var columns = columnView.Select(x => new FormComponent
            {
                Prop = x.DbColumnName,
                Label = x.ColumnDescription,
            }).ToList();
            return columns;
            #endregion
        }

    }
}
