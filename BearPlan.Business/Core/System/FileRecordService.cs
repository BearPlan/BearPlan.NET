using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace BearPlan.Business.Core.System;

/// <summary>
/// 文件记录服务
/// </summary>
public class FileRecordService : BaseServices<FileRecord>, IFileRecordService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<FileRecordDTO>> GetPageAsync(FileRecordParam param)
    {
        var page = await GetIQueryable().Select(x => new FileRecordDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<FileRecordInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<FileRecordInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateFileRecordParam param)
    {
        var model = App.Mapper.MapTo<FileRecord>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateFileRecordParam param)
    {
        var model = App.Mapper.MapTo<FileRecord>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var fileList = await GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        var result = await DeleteAsync(x => ids.Contains(x.Id));
        foreach (var file in fileList)
        {
            FileHelper.Delete(file.FilePath);
        }
        return result;
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 上传文件
    /// </summary>
    public async Task<long> UploadAsync(IFormFile file)
    {
        var fileExtensionName = FileHelper.GetExtensionName(file.FileName);
        var fileTypeName = FileHelper.GetFileTypeName(fileExtensionName);
        var fileTypeNameEn = FileHelper.GetFileTypeNameEn(fileTypeName);

        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + IdHelper.NextId() +
                          file.FileName.Substring(Math.Max(file.FileName.LastIndexOf('.'), 0));

        var prefix = App.WebHostEnvironment.WebRootPath;
        string filePath = Path.Combine(prefix, "uploads", "file", fileTypeNameEn);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        filePath = Path.Combine(filePath, fileName);
        await using (var fs = new FileStream(filePath, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
            fs.Flush();
        }

        string relativePath = Path.GetRelativePath(prefix, filePath);
        relativePath = "/" + relativePath.Replace("\\", "/");
        var fileRecord = new FileRecord
        {
            Description = file.FileName,
            OriginalName = file.FileName,
            NewName = fileName,
            FilePath = relativePath,
            Size = FileHelper.GetFileSize(file.Length),
            ContentType = file.ContentType,
            ContentTypeName = fileTypeName,
            ContentTypeNameEn = fileTypeNameEn
        };
        await AddAsync(fileRecord);
        return fileRecord.Id;
    }
    #endregion
}
