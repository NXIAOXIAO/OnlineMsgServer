# 使用官方的 .NET 8 SDK 镜像进行构建
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 设置工作目录
WORKDIR /app

# 将项目文件复制到容器中
COPY . ./

# 恢复项目依赖项
RUN dotnet restore

# 编译项目
RUN dotnet publish -c Release -o out

# 使用更小的运行时镜像
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

# 设置工作目录
WORKDIR /app

# 暴露端口
EXPOSE 13173

# 从构建镜像复制发布的应用到当前镜像
COPY --from=build /app/out .

# 设置容器启动命令
ENTRYPOINT ["dotnet", "OnlineMsgServer.dll"]
