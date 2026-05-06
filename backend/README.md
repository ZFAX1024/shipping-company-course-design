# 航运公司管理系统后端 API

这是航运应用软件开发课程设计的 ASP.NET Core Web API 后端项目。

## 技术栈

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server
- JWT 登录认证
- Swagger/OpenAPI 接口文档

## 启动方式

1. 安装 `.NET 8 SDK`。
2. 确保本机 SQL Server 已经启动。
3. 如果 SQL Server 实例不是 `localhost`，请修改 `appsettings.Development.json` 中的连接字符串。
4. 启动 API：

```powershell
dotnet restore
dotnet run
```

启动后可以访问 Swagger：

```text
https://localhost:7188/swagger
http://localhost:5188/swagger
```

项目启动时会自动执行 EF Core 初始迁移，并写入演示数据。

## 测试账号

所有测试账号的密码都是 `123456`。

| 用户名 | 角色 |
| --- | --- |
| admin | 管理员 |
| dispatcher | 调度员 |
| finance | 财务 |
| viewer | 查看者 |

## 主要接口分组

- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET/POST/PUT/DELETE /api/users`
- `GET/POST/PUT/DELETE /api/vessels`
- `GET/POST/PUT/DELETE /api/routes`
- `GET/POST/PUT/DELETE /api/customers`
- `GET/POST/PUT/DELETE /api/cargoes`
- `GET/POST/PUT/DELETE /api/orders`
- `POST /api/orders/{id}/dispatch`
- `PATCH /api/orders/{id}/status`
- `PATCH /api/orders/{id}/progress`
- `POST /api/orders/{id}/settlement`
- `GET /api/settlements`
- `POST /api/settlements/{id}/payments`
- `GET /api/dashboard/summary`
- `GET /api/system/backups`
- `POST /api/system/backups`

## 前端联调

默认 CORS 策略允许 Vue 3 开发服务从 `http://localhost:5173` 请求后端接口。

前端接口基础地址可以配置为：

```text
http://localhost:5188
```
