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
[Route("api/csvFileUpload")]
[Authorize]
public class CsvFileUploadController(ICsvFileUploadService _csvFileUploadService) : ControllerBase
{

    [HttpPost("uploadFile")]
    public async Task<ActionResult> UploadFileAsync(IFormFile file)
    {
       var response =  await _csvFileUploadService.UploadAsync(file, User);
       if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);
        
    }

    [HttpPost("confirmImport")]
public async Task<ActionResult> ConfirmImportAsync([FromBody] ConfirmImportRequest request)
{
    var response =  await _csvFileUploadService.ConfirmAsync(request, User);
       if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);
        
}
}