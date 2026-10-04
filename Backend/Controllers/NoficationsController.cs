using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices.Java;

[ApiController]
[Route("api/nofications")]
[Authorize]
public class NoficationsController(INotificationsService _service) : ControllerBase
{

    [HttpGet("getAllNofications")]
    public async Task<ActionResult> GetAllNoficationsAsync()
    {
        var result = await _service.GetAll(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

      
    }


    [HttpDelete("deleteNofication/{noficationId}")]
    public async Task<ActionResult> DeleteNofication(int noficationId)
    {
        var result = await _service.Delete(noficationId, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

     [HttpDelete("deleteAllNofications")]
    public async Task<ActionResult> DeleteAllNofications()
    {
       var result = await _service.DeleteAll(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

    [HttpPut("markNofication/{noficationId}")]
    public async Task<ActionResult> MarkNoficationAsync(int noficationId, MarkAsReadRequest request)
    {
       var result = await _service.Mark(noficationId, request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

    [HttpPut("markAllAsReadNofication/")]
    public async Task<ActionResult> MarkAllAsReadNoficationAsync()
    {
       var result = await _service.MarkAll(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }

    [HttpPut("markAllAsUnreadNofication")]
    public async Task<ActionResult> MarkAllAsUnreadNoficationAsync()
    {
        var result = await _service.MarkAllUnread(User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);

    }
}