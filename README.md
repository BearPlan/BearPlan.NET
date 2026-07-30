#### 重新定义 .NET 全栈开发模式

#### 为什么选择 BearPlan？

- 语法简洁：基于 .NET 10 与 SqlSugar ORM，代码清晰易懂，上手快。
- 生态完善：内置权限控制、AOP 拦截、日志、定时任务、缓存、多库适配等常用能力，开箱即用。
- 架构优秀：自主设计的分层架构（接口 / 业务 / 仓储 / 实体 / 基础设施），结构清晰、易扩展、易维护。
- 多库适配：SqlSugar 支持 MySQL、SQL Server、PostgreSQL、Oracle、SQLite 等主流数据库。
- 无业务入侵：核心代码不绑定具体业务，适用于任何 .NET / C# 应用程序。

#### 📚系统说明

- 后端：.NET 10、SqlSugar ORM、Serilog、Autofac（DI/AOP）、Mapster、StackExchange.Redis、Quartz、JWT
- 前端：Vue 3.5+、TypeScript、Element Plus、Pinia、Vue Router、UnoCSS、Alova.js
- 无业务逻辑代码入侵，适用于任何 .NET / C# 应用程序。
- 开发文档：[https://bear.js.org/](https://bear.js.org/)
- 默认账号：`admin / 123456`

#### 💒代码仓库

- Gitee：[https://gitee.com/BearPlan/BearPlan.NET](https://gitee.com/BearPlan/BearPlan.NET)
- GitHub：[https://github.com/BearPlan/BearPlan.NET](https://github.com/BearPlan/BearPlan.NET)

> 本项目通过 Git Submodule 引入两个子模块：
> - `BearPlan.Core`（通用框架核心库，独立仓库：<https://gitee.com/BearPlan/BearPlan.NET.Core>）
> - `BearPlan.Admin`（前端管理后台，独立仓库：<https://gitee.com/BearPlan/BearPlan.Admin>）
>
> 克隆主仓库后需执行 `git submodule update --init --recursive` 拉取子模块；或首次克隆直接带 `--recursive`。

#### ⚙️模块说明

|#|模块功能|项目文件|说明|
|--|--|--|--|
|1|Web 控制器|BearPlan.Api|接口交互层|
|2|业务接口实现|BearPlan.Business|业务服务接口实现|
|3|项目专属库|BearPlan.Common|项目业务专属：业务缓存 key、业务枚举（VersionEnum / WebSocketModelTypeEnum）、HttpUser 实现、多语言资源、AppConfig 常量等|
|4|通用框架库 ⭐|BearPlan.Core *(submodule)*|业务无关的框架基础设施：通用枚举/特性/Helper/扩展/App 全局类/Caches/ConfigOptions/纯中间件。[独立仓库](https://gitee.com/BearPlan/BearPlan.NET.Core)，后续发布 NuGet|
|5|系统实体|BearPlan.Entity|数据库实体映射类|
|6|事件总线|BearPlan.EventBus|事件总线（含微信消息处理）|
|7|业务接口|BearPlan.IBusiness|业务服务接口|
|8|应用装配层|BearPlan.Infrastructure|依赖注入装配、JWT 鉴权、过滤器、中间件、种子数据、AOP/日志实现|
|9|仓储|BearPlan.Repository|数据库仓储扩展、工作单元事务|
|10|共享模型|BearPlan.Models|请求 DTO、查询参数对象、响应模型|
|11|作业调度|BearPlan.TaskService|系统定时任务（Quartz）|
|12|前端管理后台|BearPlan.Admin *(submodule)*|基于 Vue 3 + Element Plus 的前端管理后台，[独立仓库](https://gitee.com/BearPlan/BearPlan.Admin)|

#### 🚀技术支持

- 前端框架

    SoybeanAdmin（Element Plus）：[https://github.com/soybeanjs/soybean-admin-element-plus](https://github.com/soybeanjs/soybean-admin-element-plus)

    Alova.js：[https://alova.js.org/](https://alova.js.org/)

- 后端框架

    SqlSugar：[https://www.donet5.com/](https://www.donet5.com/)

#### 🙋欢迎您的加入

|作者微信|
|:--:|
|<img src="wx.jpg" alt="微信图片" width="200" />|
|备注: 框架|
