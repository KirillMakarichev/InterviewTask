using InterviewTask.Mappers;
using InterviewTask.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterviewTask.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ApiBaseController
{
    private readonly IFriendshipsService _friendshipsService;
    private readonly IAuthService _authService;
    private readonly IUsersService _usersService;

    public UsersController(IFriendshipsService friendshipsService, 
        IAuthService authService, IUsersService usersService)
    {
        _friendshipsService = friendshipsService;
        _authService = authService;
        _usersService = usersService;
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpModel signUpModel)
    {
        var result = await _authService.SignUpAsync(signUpModel.Login, signUpModel.Password);
        return result ? Ok() : BadRequest();
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddFriend(string loginForAdding)
    {
        var addingUser = await _usersService.GetUserByLoginAsync(loginForAdding);
        if (addingUser == null)
        {
            return BadRequest();
        }

        var added = await _friendshipsService.AddFriendAsync(Id, addingUser.Id);
        return added ? Ok() : BadRequest();
    }
}
