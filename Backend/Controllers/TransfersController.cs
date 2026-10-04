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
[Route("api/transfers")]
[Authorize]
public class TransfersController(ITransferService _service) : ControllerBase
{

    [HttpPost("createTransfer")]
    public async Task<ActionResult> CreateTransferAsync(CreateTransferRequest request)
    {
        var result = await _service.CreateAsync(request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }


    [HttpGet("getAllAccTransfers/{accountId}")]
    public async Task<ActionResult> GetAllAccTransfersAsync(int accountId)
    {
      var result = await _service.GetAllAccAsync(accountId, User); 
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);         
    }

    [HttpGet("getAllTransfers")]
    public async Task<ActionResult> GetAllTransfersAsync()
    {
      var result = await _service.GetAllAsync(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);        
    }
}