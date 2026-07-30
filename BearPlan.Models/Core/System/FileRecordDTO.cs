using BearPlan.Core.Attributes;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using BearPlan.Core.Pager;
using SqlSugar;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// 文件记录
    /// </summary>
    #region 查询参数
    public class FileRecordParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(FileRecordDTO), typeof(FileRecord))]
    public class FileRecordDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 文件描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ContentType { get; set; }

        /// <summary>
        /// 文件类别
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ContentTypeName { get; set; }

        /// <summary>
        /// 文件类别英文名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ContentTypeNameEn { get; set; }

        /// <summary>
        /// 文件原名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string OriginalName { get; set; }

        /// <summary>
        /// 文件新名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string NewName { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string FilePath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Size { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(FileRecordInfo), typeof(FileRecord))]
    public class FileRecordInfo : FileRecord
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateFileRecordParam), typeof(FileRecord))]
    public class UpdateFileRecordParam : FileRecord
    {
    }
    #endregion
}
