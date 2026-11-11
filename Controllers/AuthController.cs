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
        if(regForm.Password == regForm.PasswordConfirm)
        {
            string sqlGetEmail = "SELECT * FROM Users WHERE Email = @Email";

            IEnumerable<User> usersWithRegEmail = await _dapper.LoadDataAsync<User>(sqlGetEmail, new {regForm.Email});

            if(usersWithRegEmail.Count() == 0)
            {
                int userId = await _userRepo.AddUser(regForm);

                return Ok(new Dictionary<string, string>{
                    {"token", _authHelper.CreateToken(userId)}
                });

                throw new Exception("Failed to register user!");
            }
            else
            {
                throw new Exception("Email already exists!");
            }
        }
        else
        {
            throw new Exception("Passwords don't match!");
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser(UserLoginDto loginForm)
    {
        string sqlGetUser = "SELECT UserId, Email, PasswordHash FROM Users WHERE Email = @Email";
        
        User? user = await _dapper.LoadDataSingleAsync<User>(sqlGetUser, new { loginForm.Email });

        if(user == null)
        {
            return Unauthorized("Email or Password doesn't match!");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginForm.Password);

        if(result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Email or Password doesn't match!");
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