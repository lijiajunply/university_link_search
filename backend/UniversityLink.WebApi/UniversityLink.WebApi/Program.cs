using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NpgsqlDataProtection;
using UniversityLink.Data;
using UniversityLink.DataApi.Repositories;
using UniversityLink.DataApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region 身份验证

builder.Services.AddAuthorizationBuilder()
    // 配置授权策略
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
    // 配置授权策略
    .AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"))
    // 配置授权策略
    .AddPolicy("UserPolicy", policy =>
        policy.RequireRole("Admin", "User"));

#endregion

#region 跨域

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.EndsWith(".zeabur.app") || // 支持所有 zeabur.app 子域名
                origin.EndsWith(".xauat.site") || // 支持所有 xauat.site 子域名
                origin.StartsWith("http://localhost")) // 支持本地开发环境
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // 如果需要发送凭据（如cookies、认证头等）
    });
});

#endregion

#region 数据库

var sql = Environment.GetEnvironmentVariable("SQL", EnvironmentVariableTarget.Process);
if (string.IsNullOrEmpty(sql))
{
}
else
{
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseNpgsql(sql,
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        ));

    builder.Services.AddDataProtection()
        .PersistKeysToPostgres(sql, true);
}

#endregion

#region 仓储和服务的依赖注入

builder.Services.AddHttpContextAccessor();

// 注册仓储层实现
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 注册服务层实现
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();