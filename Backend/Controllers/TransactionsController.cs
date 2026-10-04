using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;
using Microsoft.AspNetCore.Authorization;
using MyApiProject.Migrations;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController(ITransactionsService _service) : ControllerBase
{

    [HttpPost("createTransaction")]
    public async Task<ActionResult> CreateTransactionAsync(CreateTransactionRequest request)
    {
        var result = await _service.CreateAsync(request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
    

    [HttpGet("getAllAccTransactions/{accountId}")]
    public async Task<ActionResult> GetAllAccTransactionsAsync(int accountId)
    {
        var result = await _service.GetAllAccAsync(accountId, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

    [HttpGet("getAllTransactions")]
    public async Task<ActionResult> GetAllTransactionsAsync()
    {
        var result = await _service.GetAllAsync(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

    [HttpDelete("deleteTransaction/{transactionId}")]
    public async Task<ActionResult> DeleteTransactionAsync(int transactionId)
    { 
       var result = await _service.DeleteAsync(transactionId, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
  
    }

    [HttpPut("updateTransaction/{transactionId}")]
    public async Task<ActionResult> UpdateTransactionAsync(int transactionId, UpdateTransactionRequest request)
    {
        var result = await _service.UpdateAsync(transactionId, request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    public async Task CheckBudgetAlert(string userId, int? categoryId)
    {
        await _service.CheckBudget(userId, categoryId);
        
    }
}