using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;
using Microsoft.AspNetCore.Http;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 文件记录接口
/// </summary>
public interface IFileRecordService : IBaseServices<FileRecord>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<FileRecordDTO>> GetPageAsync(FileRecordParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<FileRecordInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateFileRecordParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateFileRecordParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 上传文件
    /// </summary>
    Task<long> UploadAsync(IFormFile file);
    #endregion
}
