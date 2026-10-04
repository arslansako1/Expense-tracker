using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;

[ApiController]
[Route("api/recurringRules")]
[Authorize]
public class RecurringRulesController(IRecurringRulesService _service) : ControllerBase
{

    [HttpPost("createRecurringRule")]
    public async Task<ActionResult> CreateRecurringRuleAsync(CreateRecurringRuleRequest request)
    {
        var result = await _service.CreateAsync(request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        return Ok(result.Data);
    }
    
    [HttpGet("getAllRecurringRules")]
    public async Task<ActionResult> GetAlltRecurringRulesAsync()
    {
        var result = await _service.GetAllAsync(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        return Ok(result.Data);
    }


    [HttpDelete("deleteRecurringRule/{ruleId}")]
    public async Task<ActionResult> DeleteRecurringRuleAsync(int ruleId)
    {
       var result = await _service.DeleteAsync(ruleId, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        return Ok(result.Data);
    }

    [HttpPut("updateRecurringRule/{ruleId}")]
    public async Task<ActionResult> UpdateRecurringRuleAsync(int ruleId, UpdateRecurringRuleRequest request)
    {
        var result = await _service.UpdateAsync(ruleId, request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        return Ok(result.Data);
        
    }
}