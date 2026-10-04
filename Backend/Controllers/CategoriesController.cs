using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(ICategoriesService _categoriesService) : ControllerBase
{

    [HttpPost("createCategory")]
    public async Task<ActionResult> CreateCategoryAsync(CreateCategoryRequest request)
    {
        
        var response = await _categoriesService.CreateAsync(request, User);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);

    }

    [HttpGet("getAllCategories")]
    public async Task<ActionResult> GetAllCategoriesAsync()
    {
       var response = await _categoriesService.GetAllAsync(User);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);
;
    
    }

    [HttpDelete("deleteCategory/{categoryId}")]
    public async Task<ActionResult> DeleteCategoryAsync(int categoryId)
    {
      var response = await _categoriesService.DeleteAsync(categoryId, User);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);

       
    }

    [HttpPut("updateCategory/{categoryId}")]
    public async Task<ActionResult> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request)
    {
        var response = await _categoriesService.UpdateAsync(categoryId, request, User);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);

    }
}