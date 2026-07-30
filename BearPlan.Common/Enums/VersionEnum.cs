using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

// 写在同一文件夹内 方便前端拷贝
public enum VersionEnum
{

    [Display(Name = "公共/第三方")]
    Def = 0,
    /// <summary>
    /// Pc端
    /// </summary>
    [Display(Name = "Web端网站")]
    Pc = 1,
    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "移动端/微信小程序/钉钉/H5")]
    App = 2,

}
