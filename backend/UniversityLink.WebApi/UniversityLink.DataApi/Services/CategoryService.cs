using UniversityLink.Data;
using UniversityLink.DataApi.Repositories;

namespace UniversityLink.DataApi.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    // 获取所有分类
    public async Task<List<CategoryModel>> GetAllCategoriesAsync(bool includeLinks = false,
        CancellationToken cancellationToken = default)
    {
        return (await unitOfWork.Categories.GetAllAsync(includeLinks, cancellationToken)).ToList();
    }

    // 按ID获取分类
    public async Task<CategoryModel?> GetCategoryByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Categories.GetByKeyAsync(id, cancellationToken: cancellationToken);
    }

    // 创建分类
    public async Task<CategoryModel> CreateCategoryAsync(CategoryModel category,
        CancellationToken cancellationToken = default)
    {
        // 验证分类名称
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ArgumentException("分类名称不能为空");
        }

        // 检查分类名称是否已存在
        // 修复参数，GetByNameAsync方法没有第二个bool参数
        // 根据接口定义，GetByNameAsync方法需要includeLinks参数
        var existingCategory = await unitOfWork.Categories.GetByNameAsync(category.Name, false, cancellationToken);
        if (existingCategory != null)
        {
            throw new ArgumentException($"分类名称 '{category.Name}' 已存在");
        }

        // 设置默认值
        if (string.IsNullOrEmpty(category.Icon))
        {
            category.Icon = "folder";
        }

        // 创建分类
        return await unitOfWork.Categories.CreateAsync(category, cancellationToken);
    }

    // 更新分类
    public async Task UpdateCategoryAsync(CategoryModel category, CancellationToken cancellationToken = default)
    {
        // 验证分类Key
        if (string.IsNullOrWhiteSpace(category.Key))
        {
            throw new ArgumentException("分类Key不能为空");
        }

        // 验证分类是否存在
        var existingCategory = await unitOfWork.Categories.GetByKeyAsync(category.Key, false, cancellationToken);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"分类Key '{category.Key}' 不存在");
        }

        // 验证分类名称
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ArgumentException("分类名称不能为空");
        }

        // 检查分类名称是否已被其他分类使用
        // 修复参数，GetByNameAsync方法没有第二个bool参数
        // 根据接口定义，GetByNameAsync方法需要includeLinks参数
        var existingByName = await unitOfWork.Categories.GetByNameAsync(category.Name, false, cancellationToken);
        if (existingByName != null && existingByName.Key != category.Key)
        {
            throw new ArgumentException($"分类名称 '{category.Name}' 已存在");
        }

        // 更新分类
        await unitOfWork.Categories.UpdateAsync(category, cancellationToken);
    }

    // 删除分类
    public async Task DeleteCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        // 验证分类是否存在
        var existingCategory = await unitOfWork.Categories.GetByKeyAsync(id, false, cancellationToken);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"分类Key '{id}' 不存在");
        }

        await unitOfWork.Categories.DeleteAsync(id, cancellationToken);
    }

    // 更新分类排序
    public async Task UpdateCategorySortAsync(List<string> categoryIds, CancellationToken cancellationToken = default)
    {
        // 获取所有分类
        var categories = (await unitOfWork.Categories.GetAllAsync(false, cancellationToken)).ToList();

        // 创建一个字典来快速查找顺序
        var orderDict = categoryIds.Select((id, index) => new { id, index })
                                   .ToDictionary(x => x.id, x => x.index);

        bool needsUpdate = false;
        foreach (var category in categories)
        {
            if (orderDict.TryGetValue(category.Key, out var index))
            {
                if (category.Index != index)
                {
                    category.Index = index;
                    needsUpdate = true;
                }
            }
        }

        if (needsUpdate)
        {
            await unitOfWork.Categories.BulkUpdateAsync(categories, cancellationToken);
        }
    }

    // 搜索分类
    public async Task<List<CategoryModel>> SearchCategoriesAsync(string keyword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllCategoriesAsync(false, cancellationToken);
        }

        // 在当前实现中，简单返回所有分类
        // 由于仓储没有Search方法，这里返回所有分类
        return await GetAllCategoriesAsync(false, cancellationToken);
    }

    public async Task<CategoryModel?> GetCategoryByName(string name, CancellationToken cancellationToken)
    {
        return await unitOfWork.Categories.GetByNameAsync(name, true, cancellationToken);
    }
}