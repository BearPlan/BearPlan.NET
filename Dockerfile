# 使用官方.NET 10.0行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# 设置工作目录
WORKDIR /publish



# 设置环境变量
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV TZ=Asia/Shanghai
ENV ASPNETCORE_ENVIRONMENT=Production

# 设置入口点
ENTRYPOINT ["dotnet", "BearPlan.Api.dll"]
