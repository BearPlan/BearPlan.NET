using BearPlan.Business;
using BearPlan.Core.DI;
using BearPlan.Core.Global;
using BearPlan.Core;
using BearPlan.Core.Aop;
using BearPlan.Infrastructure.Aop;
using BearPlan.Core.ConfigOptions;
using BearPlan.IBusiness;
using BearPlan.Repository.SugarHandler;
using BearPlan.Repository.UnitOfWork;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Module = Autofac.Module;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// autofac注册
/// </summary>
public class AutofacExtensions : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var options = App.GetOptions<AopOptions>();

        //事务 缓存 AOP
        var registerType = new List<Type>();
        if (options.Transactions)
        {
            builder.RegisterType<TransactionAop>();
            registerType.Add(typeof(TransactionAop));
        }

        if (options.Cache)
        {
            builder.RegisterType<CacheAop>();
            registerType.Add(typeof(CacheAop));
        }

        builder.RegisterGeneric(typeof(SugarRepository<>)).As(typeof(ISugarRepository<>))
            .InstancePerDependency();
        builder.RegisterGeneric(typeof(BaseServices<>)).As(typeof(IBaseServices<>)).InstancePerDependency();

        //注册业务层
        builder
            .RegisterTypes(GlobalType.BusinessTypes.ToArray())
            .AsImplementedInterfaces()
            .InstancePerDependency()
            .PropertiesAutowired()
            .EnableInterfaceInterceptors()
            .InterceptedBy(registerType.ToArray());

        // 注册仓储层
        builder
            .RegisterTypes(GlobalType.RepositoryTypes
                .ToArray()) //.RegisterAssemblyTypes(GlobalData.GetRepositoryAssembly())
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerDependency();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired();


        //注册控制器
        //var controllerBaseType = typeof(ControllerBase);
        //builder.RegisterAssemblyTypes(typeof(Program).Assembly)
        //    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
        //    .PropertiesAutowired();

        builder.RegisterType<DisposableContainer>()
            .As<IDisposableContainer>()
            .InstancePerLifetimeScope();
    }
}