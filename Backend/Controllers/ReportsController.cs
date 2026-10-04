using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController(IReportsService _service) : ControllerBase
{

    [HttpGet("getCategoriesSpend")]
    public async Task<ActionResult> GetCategoriesSpendAsync()
    {
        var result = await _service.GetSpendAsync(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


     [HttpGet("getBalanceHistory")]
    public async Task<ActionResult> GetBalanceHistoryAsync(int months = 1)
    {
           var result = await _service.GetBalanceAsync(User, months);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

     [HttpGet("getIncomeExpense")]
    public async Task<ActionResult> GetIncomeExpenseAsync() 
    {
        var result = await _service.GetIncomeAsync(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);   
    }
}