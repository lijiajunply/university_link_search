using Microsoft.EntityFrameworkCore;
using UniversityLink.Data;

namespace UniversityLink.DataApi.Repositories;

public class LinkRepository(LinkContext context) : ILinkRepository
{
    // 获取所有链接
    public async Task<IEnumerable<LinkModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Links
            // 移除对不存在的Category属性的引用
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);
    }
    
    // 根据Key获取链接
    public async Task<LinkModel?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Links
            // 移除对不存在的Category属性的引用
            .FirstOrDefaultAsync(l => l.Key == key, cancellationToken);
    }
    
    // 根据分类获取链接
    public async Task<IEnumerable<LinkModel>> GetByCategoryAsync(string categoryKey, CancellationToken cancellationToken = default)
    {
        return await context.Links
            .Where(l => l.CategoryKey == categoryKey)
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);
    }
    
    // 创建链接
    public async Task<LinkModel> CreateAsync(LinkModel link, CancellationToken cancellationToken = default)
    {
        // 生成唯一Key（如果未提供）
        if (string.IsNullOrEmpty(link.Key))
        {
            link.Key = GenerateUniqueKey(link.Name);
        }

        // 设置排序值为当前分类下最大排序值+1
        var maxSort = await context.Links
            .Where(l => l.CategoryKey == link.CategoryKey)
            .MaxAsync(l => (int?)l.Index, cancellationToken) ?? 0;
        link.Index = maxSort + 1;
        
        await context.Links.AddAsync(link, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return link;
    }
    
    // 更新链接
    public async Task<LinkModel> UpdateAsync(LinkModel link, CancellationToken cancellationToken = default)
    {
        context.Links.Update(link);
        await context.SaveChangesAsync(cancellationToken);
        return link;
    }
    
    // 删除链接
    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var link = await GetByKeyAsync(key, cancellationToken);
        if (link == null)
            return false;
        
        context.Links.Remove(link);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    // 检查链接是否存在
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Links.AnyAsync(l => l.Key == key, cancellationToken);
    }
    
    // 获取链接总数
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Links.CountAsync(cancellationToken);
    }
    
    // 根据分类获取链接数量
    public async Task<int> CountByCategoryAsync(string categoryKey, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .Include(c => c.Links)
            .FirstOrDefaultAsync(c => c.Key == categoryKey, cancellationToken);
        
        return category?.Links.Count ?? 0;
    }
    
    // 批量更新链接
    public async Task<int> BulkUpdateAsync(IEnumerable<LinkModel> links, CancellationToken cancellationToken = default)
    {
        context.Links.UpdateRange(links);
        return await context.SaveChangesAsync(cancellationToken);
    }
    
    // 批量删除链接
    public async Task<int> BulkDeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var keyList = keys.ToList();
        var linksToDelete = await context.Links
            .Where(l => keyList.Contains(l.Key))
            .ToListAsync(cancellationToken);
        
        context.Links.RemoveRange(linksToDelete);
        return await context.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 生成唯一Key
    /// </summary>
    private string GenerateUniqueKey(string name)
    {
        // 转换为小写并替换空格为连字符
        var key = name.ToLower()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace("--", "-");
        
        // 移除特殊字符
        key = System.Text.RegularExpressions.Regex.Replace(key, "[^a-z0-9-]", "");

        // 名称无法生成有效 key（例如纯中文）时，回退到短随机串
        if (string.IsNullOrEmpty(key))
        {
            key = System.Guid.NewGuid().ToString("N")[..12];
        }
        
        // 确保Key唯一
        var counter = 1;
        var originalKey = key;
        while (context.Links.Any(l => l.Key == key))
        {
            key = $"{originalKey}-{counter++}";
        }
        
        return key;
    }
}