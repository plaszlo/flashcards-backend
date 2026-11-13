using FlashcardBackend.Data;
using FlashcardBackend.Data.Repositories;
using FlashcardBackend.Dtos;
using FlashcardBackend.Helpers;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthHelper _authHelper;
    private readonly IUserRepository _userRepo;

    public AuthController(IConfiguration config, IPasswordHasher<User> passwordHasher, IUserRepository userRepo)
    {
        _dapper = new DataContextDapper(config);
        _passwordHasher = passwordHasher;
        _authHelper = new AuthHelper(config);
        _userRepo = userRepo;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(UserRegistrationDto regForm)
    {
        if(regForm.Password != regForm.PasswordConfirm)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "Passwords don't match!"
            });
        }

        User? user = await _userRepo.GetUserByEmail(regForm.Email);

        if(user == null)
        {
            int userId = await _userRepo.AddUser(regForm);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });

            throw new Exception("Failed to register user!");
        }
        else
        {
            return Conflict(new ProblemDetails
            {
                Detail = "Email already exists!"
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser(UserLoginDto loginForm)
    {        
        User? user = await _userRepo.GetUserByEmail(loginForm.Email);

        if(user == null)
        {
            return NotFound(new ProblemDetails
            {
                Detail = "Email not found!"
            });
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginForm.Password);

        if(result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Email or Password doesn't match!"
            });
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            if(await _userRepo.RehashPassword(user, loginForm.Password))
            {
                return Ok();
            }
            
            throw new Exception("Password match but failed to rehash!");
        }

        return Ok(new Dictionary<string, string> {
            {"token", _authHelper.CreateToken(user.UserId)}
        });
    }
}