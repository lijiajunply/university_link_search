using Microsoft.EntityFrameworkCore;
using UniversityLink.Data;

namespace UniversityLink.DataApi.Repositories;

public class CategoryRepository(LinkContext context) : ICategoryRepository
{
    // 获取所有分类
    public async Task<IEnumerable<CategoryModel>> GetAllAsync(bool includeLinks = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();

        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }

        return await query.OrderBy(c => c.Index).ToListAsync(cancellationToken);
    }

    // 根据Key获取分类
    public async Task<CategoryModel?> GetByKeyAsync(string key, bool includeLinks = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();

        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }

        return await query.FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
    }

    // 根据名称获取分类
    public async Task<CategoryModel?> GetByNameAsync(string name, bool includeLinks = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();

        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }

        return await query.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    // 创建分类
    public async Task<CategoryModel> CreateAsync(CategoryModel category, CancellationToken cancellationToken = default)
    {
        // 生成唯一Key（如果未提供）
        if (string.IsNullOrEmpty(category.Key))
        {
            category.Key = GenerateUniqueKey(category.Name);
        }

        // 设置排序值为当前最大排序值+1
        var maxSort = await context.Categories.MaxAsync(c => (int?)c.Index, cancellationToken) ?? 0;
        category.Index = maxSort + 1;

        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return category;
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
        while (context.Categories.Any(c => c.Key == key))
        {
            key = $"{originalKey}-{counter++}";
        }

        return key;
    }

    // 更新分类
    public async Task<CategoryModel> UpdateAsync(CategoryModel category, CancellationToken cancellationToken = default)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    // 删除分类
    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var category = await GetByKeyAsync(key, true, cancellationToken);
        if (category == null)
            return false;

        // 删除分类下的所有链接
        context.Links.RemoveRange(category.Links);

        // 删除分类
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    // 检查分类是否存在
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Categories.AnyAsync(c => c.Key == key, cancellationToken);
    }

    // 获取分类总数
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.CountAsync(cancellationToken);
    }

    // 批量更新分类
    public async Task<int> BulkUpdateAsync(IEnumerable<CategoryModel> categories,
        CancellationToken cancellationToken = default)
    {
        var categoryList = categories.ToList();
        context.Categories.UpdateRange(categoryList);
        return await context.SaveChangesAsync(cancellationToken);
    }
}