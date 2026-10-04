


using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class CategoriesService(AppDbContext _context, UserManager<ApplicationUser> _userManager) : ICategoriesService
{
    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<CreateCategoryResponse>.Fail("User not found");
        }

        var category = new Category
        {
            UserId = user.Id,
            Name = request.Name,
            Icon = request.Icon,
            CreatedAt = DateTime.UtcNow         
        };

        _context.Categories.Add(category);

        var auditLog = new AuditLog
        {
            EntityType = "Categories",
            EntityId = category.Id,
            UserId = user.Id,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"Name: {category.Name}, Icon: {category.Icon}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);


        await _context.SaveChangesAsync();  

        var response = new CreateCategoryResponse(
            Id: category.Id,
            Name: category.Name,
            Icon: category.Icon,
            CreatedAt: category.CreatedAt
        );      

        return ServiceResult<CreateCategoryResponse>.Ok(response);
    
    }

    public async Task<ServiceResult<List<Category>>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
    { 
         var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return  ServiceResult<List<Category>>.Fail("User not found");
        }

        var categories = await _context.Categories
        .Where(c => c.UserId == user.Id && c.IsDeleted == false)
        .Include(b => b.Budgets!.Where(b => b.IsDeleted == false))
        .ToListAsync();

        if (categories is null)
        {
            return ServiceResult<List<Category>>.Fail("No categories found");
        }

        return ServiceResult<List<Category>>.Ok(categories);
        
    }
    public async Task<ServiceResult<bool>> DeleteAsync(int categoryId, ClaimsPrincipal claimsPrincipal)
    {
         var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.IsDeleted == false);
        if (category is null)
        {
            return ServiceResult<bool>.Fail("Category not found");
        }

        if (category.UserId != user.Id)
        {
            return ServiceResult<bool>.Fail("You do not own this category");
        }

        category.IsDeleted = true;

        var auditLog = new AuditLog
        {
            EntityType = "Categories",
            EntityId = category.Id,
            UserId = user.Id,
            Action = "Deleted",
            OldValue = $"Name {category.Name}, Icon: {category.Icon}",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
       
    }
    public async Task<ServiceResult<UpdateCategoryResponse>> UpdateAsync(int categoryId, UpdateCategoryRequest request, ClaimsPrincipal claimsPrincipal)
    { 
       var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<UpdateCategoryResponse>.Fail("User not found");
        }

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.IsDeleted == false);
        if (category is null)
        {
            return ServiceResult<UpdateCategoryResponse>.Fail("Category not found");
        }

        if (category.UserId != user.Id)
        {
            return ServiceResult<UpdateCategoryResponse>.Fail("You do not own this category");
        }

        var oldName = category.Name;
        var oldIcon = category.Icon;

        category.Name = request.Name;
        category.Icon = request.Icon;

        var auditLog = new AuditLog
        {
            EntityType = "Categories",
            EntityId = category.Id,
            UserId = user.Id,
            Action = "Updated",
            OldValue = $"Name {oldName}, Icon: {oldIcon}",
            NewValue = $"Name {category.Name}, Icon: {category.Icon}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        var response = new UpdateCategoryResponse(
            Id: category.Id,
            Name: category.Name,
            Icon: category.Icon
        );

        return ServiceResult<UpdateCategoryResponse>.Ok(response);
    }
}