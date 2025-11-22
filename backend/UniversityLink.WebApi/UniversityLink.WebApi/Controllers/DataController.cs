using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UniversityLink.Data;

namespace UniversityLink.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "InternalJWT")]
public class DataController(IDbContextFactory<LinkContext> dbFactory)
    : ControllerBase
{
    // GET: api/data/download
    [HttpGet("download")]
    [Authorize(AuthenticationSchemes = "InternalJWT")]
    public async Task<IActionResult> JsonDownload(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);

            // 获取当前用户身份（从JWT token或其他身份验证机制中）
            var identity = User.FindFirst("role")?.Value ?? "Member";

            if (identity == "Member")
            {
                // 普通成员只下载Markdown格式的链接数据
                var categories = await context.Categories
                    .Include(x => x.Links.OrderBy(y => y.Index))
                    .OrderBy(x => x.Index)
                    .ToListAsync(cancellationToken);

                var markdownBuilder = new StringBuilder();
                markdownBuilder.AppendLine("# 校园服务导航");
                markdownBuilder.AppendLine();

                foreach (var category in categories)
                {
                    markdownBuilder.AppendLine($"## {category.Name}");
                    markdownBuilder.AppendLine();

                    foreach (var link in category.Links)
                    {
                        markdownBuilder.AppendLine($"- [{link.Name}]({link.Url}) - {link.Description}");
                    }

                    markdownBuilder.AppendLine();
                }

                return Content(markdownBuilder.ToString(), "text/markdown; charset=utf-8");
            }
            else
            {
                // 管理员下载完整的JSON数据
                var users = await context.Users.ToListAsync(cancellationToken);
                var categories = await context.Categories
                    .Include(x => x.Links.OrderBy(y => y.Index))
                    .OrderBy(x => x.Index)
                    .ToListAsync(cancellationToken);

                var data = new AllDataModel
                {
                    Users = users,
                    Categories = categories
                };

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                return Content(json, "application/json; charset=utf-8");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "数据下载失败", error = ex.Message });
        }
    }
}