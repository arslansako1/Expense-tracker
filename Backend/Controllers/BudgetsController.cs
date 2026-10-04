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
[Route("api/budgets")]
[Authorize]
public class budgetsController(IBudgetService _budgetService) : ControllerBase
{

    [HttpPost("createBudget")]
    public async Task<ActionResult> CreateBudgetAsync(CreateBudgetRequest request)
    {

       var result = await _budgetService.CreateAsync(request, User);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
        
    }
    

    [HttpGet("getAllBudgets")]
    public async Task<ActionResult> GetAllBudgetsAsync()
    {
       
       var result = await _budgetService.GetAllAsync(User);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


    [HttpDelete("deleteBudget/{budgetId}")]
    public async Task<ActionResult> DeleteBudgetAsync(int budgetId)
    {
     
       var result = await _budgetService.DeleteAsync(budgetId, User);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


    [HttpPut("updateBudget/{budgetId}")]
    public async Task<ActionResult> UpdateBudgetAsync(int budgetId, UpdateBudgetRequest request)
    {
       
       var result = await _budgetService.UpdateAsync(budgetId, request, User);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}