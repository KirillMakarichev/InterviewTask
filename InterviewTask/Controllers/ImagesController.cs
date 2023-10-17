using System.Security.Claims;
using InterviewTask.Mappers;
using InterviewTask.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTask.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ImagesController : ApiBaseController
{
    private readonly IUsersService _usersService;
    private string Name => User.FindFirst(ClaimTypes.Name).Value;
    
    public ImagesController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> SaveImage(IFormFile imageFile)
    {
        if (imageFile is not { Length: > 0 })
        {
            return BadRequest();
        }

        var fileExtension = Path.GetExtension(imageFile.FileName);
        
        var image = await _usersService.SaveImageAsync(imageFile.OpenReadStream(), fileExtension, Id);
        return Ok(ImageMapper.ConvertFromDBImage(image));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetImages(string? userOwner)
    {
        if (string.IsNullOrWhiteSpace(userOwner))
        {
            userOwner = Name;
        }

        var images = await _usersService.GetAllImagesAsync(userOwner, Id);

        if (images.Error != Status.Ok)
        {
            return GetResponseByError(images.Error);
        }

        var response = images.Response.Select(ImageMapper.ConvertFromDBImage).ToList();
        return Ok(response);
    }
    
    [HttpGet("download")]
    public async Task<IActionResult> DownloadImage(string? userOwner, string imageName)
    {
        if (string.IsNullOrWhiteSpace(userOwner))
        {
            userOwner = Name;
        }

        if (string.IsNullOrWhiteSpace(imageName))
        {
            return BadRequest();
        }

        var image = await _usersService.GetImageAsync(imageName, userOwner, Id);

        if (image.Error != Status.Ok)
        {
            return GetResponseByError(image.Error);
        }
        
        return File(image.Response, "image/png");
    }
}