using InterviewTask.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTask.Controllers;

public class ApiBaseController : ControllerBase
{
    protected int Id => int.Parse(User.FindFirst("Id").Value);

    protected IActionResult GetResponseByError(Status error)
    {
        return error switch
        {
            Status.Ok => Ok(),
            Status.NoAccess => Forbid(),
            Status.OwnerNotFound => NotFound(),
            Status.FileNotFound => NotFound(),
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };
    }
}