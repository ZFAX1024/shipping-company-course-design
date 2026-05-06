# 航运公司管理系统课程设计

本仓库是航运应用软件开发课程设计项目，当前包含 ASP.NET Core 后端接口和课程设计文档。

## 项目结构

- `backend/`：后端 Web API 项目
- `doc/`：课程设计相关文档和演示材料

## 技术栈

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server
- JWT 登录认证
- Swagger/OpenAPI 接口文档

## 后端启动方式

1. 安装 `.NET 8 SDK`。
2. 安装并启动 SQL Server。
3. 进入后端目录：

```powershell
cd backend
```

4. 还原依赖并启动项目：

```powershell
dotnet restore
dotnet run
```

启动后可以访问 Swagger 接口文档：

```text
https://localhost:7188/swagger
http://localhost:5188/swagger
```

## 数据库说明

默认连接本机 SQL Server：

```text
Server=localhost;Database=ShippingCompanyDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

如果本机 SQL Server 实例名不是 `localhost`，请修改 `backend/appsettings.Development.json` 中的 `DefaultConnection`。

项目启动时会自动执行数据库迁移，并初始化演示数据。

## 测试账号

所有测试账号的密码都是 `123456`。

| 用户名 | 角色 |
| --- | --- |
| admin | 管理员 |
| dispatcher | 调度员 |
| finance | 财务 |
| viewer | 查看者 |

## 前端联调

前端开发服务默认使用 Vue 3 常见端口：

```text
http://localhost:5173
```

接口基础地址可以配置为：

```text
http://localhost:5188
```

更多接口说明见 `backend/README.md` 或启动后查看 Swagger。
