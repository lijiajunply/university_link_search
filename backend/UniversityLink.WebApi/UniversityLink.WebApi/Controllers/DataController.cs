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

                return File(Encoding.UTF8.GetBytes(markdownBuilder.ToString()),
                    "text/markdown; charset=utf-8", "university_links.md");
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
                return File(Encoding.UTF8.GetBytes(json),
                    "application/json; charset=utf-8", "university_links.json");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "数据下载失败", error = ex.Message });
        }
    }

    // POST: api/data/upload
    // 仅支持 JSON 文件，合并导入分类与链接（按 Key 匹配）
    [HttpPost("upload")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> JsonUpload([FromForm] IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "请上传文件" });
            }

            // 仅支持 JSON 文件
            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".json")
            {
                return BadRequest(new { message = "仅支持 JSON 文件" });
            }

            // 读取并反序列化 JSON
            AllDataModel? data;
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                var content = await reader.ReadToEndAsync(cancellationToken);
                data = JsonConvert.DeserializeObject<AllDataModel>(content);
            }

            if (data is null || data.Categories is null)
            {
                return BadRequest(new { message = "JSON 文件内容格式不正确" });
            }

            await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);

            // 合并导入分类及其链接（按 Key 匹配）
            foreach (var category in data.Categories)
            {
                if (string.IsNullOrWhiteSpace(category.Key) || string.IsNullOrWhiteSpace(category.Name))
                {
                    continue;
                }

                var existingCategory = await context.Categories
                    .Include(c => c.Links)
                    .FirstOrDefaultAsync(c => c.Key == category.Key, cancellationToken);

                if (existingCategory is null)
                {
                    // 新分类：连同其链接一起新增
                    context.Categories.Add(category);
                    continue;
                }

                // 已有分类：更新字段
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.Icon = category.Icon;
                existingCategory.Index = category.Index;

                // 合并链接
                foreach (var link in category.Links ?? new List<LinkModel>())
                {
                    if (string.IsNullOrWhiteSpace(link.Key))
                    {
                        continue;
                    }

                    var existingLink = existingCategory.Links.FirstOrDefault(l => l.Key == link.Key);
                    if (existingLink is null)
                    {
                        existingCategory.Links.Add(link);
                    }
                    else
                    {
                        existingLink.Name = link.Name;
                        existingLink.Icon = link.Icon;
                        existingLink.Url = link.Url;
                        existingLink.Description = link.Description;
                        existingLink.Index = link.Index;
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "导入成功" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "数据导入失败", error = ex.Message });
        }
    }
}