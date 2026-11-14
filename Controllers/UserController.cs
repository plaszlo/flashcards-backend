using FlashcardBackend.Data.Repositories;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [AllowAnonymous]
    [HttpGet("Users/{userId}")]
    public async Task<IEnumerable<User>> GetUsers(int userId = 0)
    {
        return await _userRepo.GetUsersAsync(userId);
    }

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser(UserUpdateProfileDto userDetails)
    {
        User? user = await _userRepo.GetUserByEmail(userDetails.Email);

        if(user != null)
        {
            return Conflict(new ProblemDetails
            {
                Detail = "Email is already taken!"
            });
        }

        if (await _userRepo.UpdateUser(User, userDetails))
        {
            return Ok(new {});
        }

        return NotFound(new ProblemDetails
        {
            Detail = "User not found!"
        });
    }

    [HttpDelete("User")]
    public async Task<IActionResult> DeleteUser()
    {
        if(await _userRepo.DeleteUser(User))
        {
            return Ok(new {});
        }

        return NotFound(new ProblemDetails
        {
            Detail = "User not found!"
        });
    }
}