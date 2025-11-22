using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NpgsqlDataProtection;
using Scalar.AspNetCore;
using UniversityLink.Data;
using UniversityLink.DataApi.Repositories;
using UniversityLink.DataApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region 身份验证

// 配置 OAuth2
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false, // 对于外部OAuth令牌，可能需要特殊处理
            ValidateIssuerSigningKey = false, // 对于外部OAuth令牌，可能没有我们期望的签名密钥
        };

        // 允许处理 OAuth2 不透明令牌
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // 从 Authorization 头中提取令牌
                var authorizationHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    context.Token = authorizationHeader["Bearer ".Length..];
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Challenge: {context.AuthenticateFailure?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = _ =>
            {
                Console.WriteLine("Token validated successfully:");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    // 配置授权策略
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
    // 配置授权策略
    .AddPolicy("Admin", policy =>
        policy.RequireRole("Founder", "President", "Minister", "Department"))
    // 配置授权策略
    .AddPolicy("User", policy =>
        policy.RequireRole("Member", "Founder", "President", "Minister", "Department"));

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
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseSqlite("Data Source=universitylink.db"));

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("./keys"));
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LinkContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
            await context.Database.EnsureCreatedAsync();
        }
    }

    await context.SaveChangesAsync();
    await context.DisposeAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapScalarApiReference();

app.Run();