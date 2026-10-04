

using System.Security.Claims;

public interface ICategoriesService
{
    Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Category>>> GetAllAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<bool>> DeleteAsync(int categoryId, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<UpdateCategoryResponse>> UpdateAsync(int CategoryId, UpdateCategoryRequest request, ClaimsPrincipal claimsPrincipal);

}