using Dapper;
using FlashcardBackend.Data;
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
    private readonly AuthHelper _authHelper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(IConfiguration config, IPasswordHasher<User> passwordHasher)
    {
        _dapper = new DataContextDapper(config);
        _passwordHasher = passwordHasher;
        _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult RegisterUser(UserRegistrationDto regForm)
    {
        if(regForm.Password == regForm.PasswordConfirm)
        {
            string sqlGetEmail = "SELECT * FROM Users WHERE Email = @Email";

            IEnumerable<User> usersWithRegEmail = _dapper.LoadData<User>(sqlGetEmail, new {regForm.Email});

            if(usersWithRegEmail.Count() == 0)
            {
                var user = new User
                {
                    Name = regForm.Name,
                    Email = regForm.Email,
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, regForm.Password);

                string sql = "spUsers_Register";

                var sqlParamaters = new DynamicParameters();
                sqlParamaters.Add("@Name", user.Name);
                sqlParamaters.Add("@Email", user.Email);
                sqlParamaters.Add("@PasswordHash", user.PasswordHash);

                if (_dapper.ExecuteSql(sql, sqlParamaters, System.Data.CommandType.StoredProcedure))
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

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult LoginUser(UserLoginDto loginForm)
    {
        string sqlGetUser = "SELECT UserId, Email, PasswordHash FROM Users WHERE Email = @Email";
        
        User? user = _dapper.LoadDataSingle<User>(sqlGetUser, new { loginForm.Email });

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
            user.PasswordHash = _passwordHasher.HashPassword(user, loginForm.Password);

            string sqlUpdateHash = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Email = @Email";

            var sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PasswordHash", user.PasswordHash);
            sqlParameters.Add("@Email", loginForm.Email);

            if(_dapper.ExecuteSql(sqlUpdateHash, sqlParameters))
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