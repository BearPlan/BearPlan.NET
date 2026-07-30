using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Core.Utils;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using BearPlan.Common.Global;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Common.WebApp;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseServices<User>, IUserService
{
    #region 字段
    private readonly IDeptService _deptService;
    private readonly IRoleService _roleService;
    #endregion

    #region 构造函数
    public UserService(IDeptService deptService, IRoleService roleService)
    {
        _deptService = deptService;
        _roleService = roleService;
    }
    #endregion

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<UserDTO>> GetPageAsync(UserParam param)
    {
        var page = await GetIQueryable().Select(x => new UserDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    //[UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserInfoById)]
    public async Task<UserInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id)
            .Includes(x => x.Dept)
            .Includes(x => x.Roles)
            .Includes(x => x.Jobs)
            .Select<UserInfo>(x=> new UserInfo
            {
                Dept = x.Dept,
                Roles = x.Roles,
                Jobs   = x.Jobs
            }
            
            ,true).FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    [UseTran]
    public async Task<long> AddAsync(UpdateUserParam param)
    {
        if (await GetIQueryable(x => x.UserName == param.UserName).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.UserName)));
        }

        if (!string.IsNullOrEmpty(param.Email) &&
            await GetIQueryable(x => x.Email == param.Email).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Email)));
        }

        if (!string.IsNullOrEmpty(param.Phone) &&
            await GetIQueryable(x => x.Phone == param.Phone).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Phone)));
        }

        var user = App.Mapper.MapTo<User>(param);

        // 设置用户默认密码
        user.Password = BCryptHelper.Hash(App.GetOptions<SystemOptions>().UserDefaultPassword);
        user.Avatar = string.Empty;

        await AddAsync(user);

        // 用户角色关联
        if (param.RoleIds != null && param.RoleIds.Count > 0)
        {
            await SugarClient.Deleteable<UserRole>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
            var userRoles = param.RoleIds.Select(x => new UserRole { UserId = user.Id, RoleId = x }).ToList();
            await SugarClient.Insertable(userRoles).ExecuteCommandAsync();
        }

        // 用户岗位关联
        if (param.JobIds != null && param.JobIds.Count > 0)
        {
            await SugarClient.Deleteable<UserJob>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
            var userJobs = param.JobIds.Select(x => new UserJob { UserId = user.Id, JobId = x }).ToList();
            await SugarClient.Insertable(userJobs).ExecuteCommandAsync();
        }

        return user.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [UseTran]
    public async Task<long> UpdateAsync(UpdateUserParam param)
    {
        var oldUser = await GetIQueryable(x => x.Id == param.Id)
            .Includes(x => x.Roles).FirstAsync();
        if (oldUser == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        if (oldUser.UserName != param.UserName &&
            await GetIQueryable(x => x.UserName == param.UserName).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.UserName)));
        }

        if (oldUser.Email != param.Email &&
            !string.IsNullOrEmpty(param.Email) &&
            await GetIQueryable(x => x.Email == param.Email).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Email)));
        }

        if (oldUser.Phone != param.Phone &&
            !string.IsNullOrEmpty(param.Phone) &&
            await GetIQueryable(x => x.Phone == param.Phone).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Phone)));
        }

        // 验证角色等级
        if (oldUser.Roles != null && oldUser.Roles.Count > 0)
        {
            var levels = oldUser.Roles.Select(x => x.Level);
            await _roleService.VerificationUserRoleLevelAsync(levels.Min());
        }

        var user = App.Mapper.MapTo<User>(param);
        // 更新时排除 Password, Avatar, PasswordReSetTime
        await UpdateAsync(user, null, x => new { x.Password, x.Avatar, x.PasswordReSetTime }, false);

        // 用户角色关联
        await SugarClient.Deleteable<UserRole>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        if (param.RoleIds != null && param.RoleIds.Count > 0)
        {
            var userRoles = param.RoleIds.Select(x => new UserRole { UserId = user.Id, RoleId = x }).ToList();
            await SugarClient.Insertable(userRoles).ExecuteCommandAsync();
        }

        // 用户岗位关联
        await SugarClient.Deleteable<UserJob>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        if (param.JobIds != null && param.JobIds.Count > 0)
        {
            var userJobs = param.JobIds.Select(x => new UserJob { UserId = user.Id, JobId = x }).ToList();
            await SugarClient.Insertable(userJobs).ExecuteCommandAsync();
        }

        // 清理缓存
        await ClearUserCache(user.Id);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        if (ids.Contains(App.GetService<IHttpUser>().Id))
        {
            throw new BusException(Language.Error_ForbidToDeleteYourself);
        }

        // 验证角色等级
        await _roleService.VerificationUserRoleLevelAsync(
            await _roleService.QueryUserRoleLevelAsync(ids));

        var users = await GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        foreach (var user in users)
        {
            await ClearUserCache(user.Id);
        }

        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion

    #region 扩展

    /// <summary>
    /// 查找用户
    /// </summary>
    public async Task<UserInfo> QueryByNameAsync(string userName)
    {
        if (userName.IsEmail())
        {
            return await GetIQueryable(s => s.Email == userName)
                .Select<UserInfo>().FirstAsync();
        }

        return await GetIQueryable(s => s.UserName == userName)
            .Select<UserInfo>().FirstAsync();
    }

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    public async Task<List<UserDTO>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        return await GetIQueryable(u => deptIds.Contains(u.DeptId))
            .Select<UserDTO>().ToListAsync();
    }

    /// <summary>
    /// 更新用户公共信息
    /// </summary>
    public async Task UpdateCenterAsync(UpdateUserCenterParam param)
    {
        var user = await GetIQueryable(x => x.Id == App.GetService<IHttpUser>().Id).FirstAsync();
        if (user == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        var checkUser = await GetIQueryable(x =>
            x.Phone == param.Phone && x.Id != App.GetService<IHttpUser>().Id).FirstAsync();
        if (checkUser != null)
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Phone)));
        }

        user.NickName = param.NickName;
        user.Sex = param.Sex;
        user.Phone = param.Phone;
        await UpdateAsync(user, x => new { x.NickName, x.Sex, x.Phone });
    }

    /// <summary>
    /// 更新用户密码
    /// </summary>
    public async Task UpdatePasswordAsync(UpdateUserPassParam param)
    {
        var rsaOptions = App.GetOptions<RsaOptions>();
        var rsaHelper = new RsaHelper(rsaOptions.PrivateKey, rsaOptions.PublicKey);
        string oldPassword = rsaHelper.Decrypt(param.OldPassword);
        string newPassword = rsaHelper.Decrypt(param.NewPassword);
        string confirmPassword = rsaHelper.Decrypt(param.ConfirmPassword);

        if (oldPassword == newPassword)
        {
            throw new BusException(Language.Error_PasswordSameAsOld);
        }

        if (!newPassword.Equals(confirmPassword))
        {
            throw new BusException(Language.Error_InputsDoNotMatch);
        }

        var curUser = await GetIQueryable(x => x.Id == App.GetService<IHttpUser>().Id).FirstAsync();
        if (curUser == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        if (!BCryptHelper.Verify(oldPassword, curUser.Password))
        {
            throw new BusException(Language.Error_IncorrectOldPassword);
        }

        curUser.Password = BCryptHelper.Hash(newPassword);
        curUser.PasswordReSetTime = DateTime.Now;
        var num = await UpdateAsync(curUser, x => new { x.Password, x.PasswordReSetTime });
        if (num>0)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                        curUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey +
                                        App.GetService<IHttpUser>().JwtToken.ToMd5String16());
        }
    }

    /// <summary>
    /// 更新用户界面偏好配置
    /// </summary>
    public async Task UpdatePreferencesConfigAsync(UpdateUserPreferencesConfigParam param)
    {
        var curUser = await GetIQueryable(x => x.Id == App.GetService<IHttpUser>().Id).FirstAsync();
        if (curUser == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        curUser.PreferencesConfig = param.PreferencesConfig;
        var num = await UpdateAsync(curUser, x => x.PreferencesConfig);
        if (num>0)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                        curUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey +
                                        App.GetService<IHttpUser>().JwtToken.ToMd5String16());
        }
    }

    /// <summary>
    /// 修改邮箱
    /// </summary>
    public async Task UpdateEmailAsync(UpdateUserEmailParam param)
    {
        var curUser = await GetIQueryable(x => x.Id == App.GetService<IHttpUser>().Id).FirstAsync();
        if (curUser == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        var rsaOptions = App.GetOptions<RsaOptions>();
        var rsaHelper = new RsaHelper(rsaOptions.PrivateKey, rsaOptions.PublicKey);
        string password = rsaHelper.Decrypt(param.Password);
        if (!BCryptHelper.Verify(password, curUser.Password))
        {
            throw new BusException(Language.Error_InvalidPassword);
        }

        var code = await App.Cache.GetAsync<string>(
            GlobalConstants.CachePrefix.EmailCaptcha + param.Email.ToMd5String16());
        if (string.IsNullOrEmpty(code) || !code.Equals(param.Code))
        {
            throw new BusException(Language.Error_InvalidVerificationCode);
        }

        curUser.Email = param.Email;
        await UpdateAsync(curUser, x => x.Email);
    }

    /// <summary>
    /// 更新用户头像
    /// </summary>
    public async Task<string> UpdateAvatarAsync(IFormFile file)
    {
        var curUser = await GetIQueryable(x => x.Id == App.GetService<IHttpUser>().Id).FirstAsync();
        if (curUser == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        var prefix = App.WebHostEnvironment.WebRootPath;
        string avatarName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + IdHelper.NextId() +
                            file.FileName.Substring(Math.Max(file.FileName.LastIndexOf('.'), 0));
        string avatarPath = Path.Combine(prefix, "uploads", "file", "avatar");

        if (!Directory.Exists(avatarPath))
        {
            Directory.CreateDirectory(avatarPath);
        }

        avatarPath = Path.Combine(avatarPath, avatarName);
        await using (var fs = new FileStream(avatarPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
            fs.Flush();
        }

        string relativePath = Path.GetRelativePath(prefix, avatarPath);
        relativePath = "/" + relativePath.Replace("\\", "/");
        curUser.Avatar = relativePath;
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                    curUser.Id.ToString().ToMd5String16());
        await UpdateAsync(curUser);
        return relativePath;
    }

    /// <summary>
    /// 修改用户角色
    /// </summary>
    [UseTran]
    public async Task UpdateUserRoleAsync(UpdateUserRoleParam param)
    {
        var user = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (user == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        await SugarClient.Deleteable<UserRole>().Where(x => x.UserId == param.Id).ExecuteCommandAsync();
        if (param.RoleIds != null && param.RoleIds.Count > 0)
        {
            var userRoles = param.RoleIds.Select(r => new UserRole
            {
                UserId = param.Id, RoleId = r
            }).ToList();
            await SugarClient.Insertable(userRoles).ExecuteCommandAsync();
        }

        await ClearUserCache(param.Id);
    }

    /// <summary>
    /// 修改用户岗位
    /// </summary>
    [UseTran]
    public async Task UpdateUserJobAsync(UpdateUserJobParam param)
    {
        var user = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (user == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        await SugarClient.Deleteable<UserJob>().Where(x => x.UserId == param.Id).ExecuteCommandAsync();
        if (param.JobIds != null && param.JobIds.Count > 0)
        {
            var userJobs = param.JobIds.Select(r => new UserJob { UserId = param.Id, JobId = r }).ToList();
            await SugarClient.Insertable(userJobs).ExecuteCommandAsync();
        }

        await ClearUserCache(param.Id);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 清理用户缓存
    /// </summary>
    private async Task ClearUserCache(long userId)
    {
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthUrls +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthCodes +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserDataScopeById +
                                    userId.ToString().ToMd5String16());
    }
    #endregion
}
