using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using NpgsqlDataProtection;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UniversityLink.Data;
using UniversityLink.DataApi.Repositories;
using UniversityLink.DataApi.Services;
using UniversityLink.WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region 身份验证

// Configure Forwarded Headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// 配置 OAuth2
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "InternalJWT";
        options.DefaultChallengeScheme = "InternalJWT";
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddOAuth("ExternalOAuth", options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID") ?? "your-client-id";
        options.ClientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET") ?? "your-client-secret";
        options.CallbackPath = "/Auth/Callback";

        options.AuthorizationEndpoint = "https://api.xauat.site/SSO/authorize";
        options.TokenEndpoint = "https://api.xauat.site/SSO/token";
        options.UserInformationEndpoint = "https://api.xauat.site/SSO/userinfo";

        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("openid");
        options.Scope.Add("read");

        options.SaveTokens = true;

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Role, "role");
        options.ClaimActions.MapJsonKey("userName", "name");
        options.ClaimActions.MapJsonKey("userId", "sub");
        options.ClaimActions.MapJsonKey("role", "role");
        options.ClaimActions.MapJsonKey("email", "email");
        options.ClaimActions.MapJsonKey("avatar", "avatar");
        options.ClaimActions.MapJsonKey("name", "name");
        options.ClaimActions.MapJsonKey("sub", "sub");

        options.Events.OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            context.RunClaimActions(json.RootElement);
        };

        // 2.创建Ticket失败时触发
        options.Events.OnRemoteFailure = context =>
        {
            Console.WriteLine(context.Failure?.Message);
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        };
        // 3.Ticket接收完成之后触发
        options.Events.OnTicketReceived = context =>
        {
            Console.WriteLine(context.Principal);
            return Task.CompletedTask;
        };
    })
    .AddCookie()
    .AddJwtBearer("InternalJWT", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "UniversityLink",
            ValidAudience = "UniversityLinkUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your-secret-key-here-change-in-production"))
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

builder.Services.AddScoped<IJwtGenerate, JwtGenerate>();

// 注册仓储层实现
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 注册服务层实现
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IUserService, UserService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use Forwarded Headers Middleware
app.UseForwardedHeaders();

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