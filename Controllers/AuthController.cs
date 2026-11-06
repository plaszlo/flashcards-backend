using FlashcardBackend.Data;
using FlashcardBackend.Dtos;
using FlashcardBackend.Helpers;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    [HttpPost("/Register")]
    public IActionResult RegisterUser(UserRegistrationDto userRegDetails)
    {
        if(userRegDetails.Password == userRegDetails.PasswordConfirm)
        {
            string sqlEmailCheck = "SELECT * FROM Users WHERE Email = @Email";

            IEnumerable<User> usersWithRegEmail = _dapper.LoadData<User>(sqlEmailCheck, new {userRegDetails.Email});

            if(usersWithRegEmail.Count() == 0)
            {
                if (_authHelper.SetPassword(userRegDetails))
                {
                    return Ok();
                }
                else
                {
                    throw new Exception("Failed to register user!");
                }
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
}