using System.Data;
using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Dtos;
using FlashcardBackend.Helpers;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly SqlHelper _sqlHelper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _sqlHelper = new SqlHelper(config);
    }

    [HttpGet("Users/{userId}")]
    public IEnumerable<User> GetUsers(int userId)
    {
        string sql = "spUsers_Get";

        DynamicParameters sqlParam = new DynamicParameters();

        if(userId != 0)
        {
            sqlParam.Add("@UserId", userId);
        }

        return _dapper.LoadData<User>(sql, sqlParam, CommandType.StoredProcedure);
    }

    [HttpPut("UpdateUser")]
    public IActionResult UpdateUser(UserUpdateProfileDto userDetails)
    {
        string sql = "spUsers_Update";

        DynamicParameters sqlParam = new DynamicParameters();
        sqlParam.Add("@UserId", userDetails.UserId);
        sqlParam.Add("@Name", userDetails.Name);
        sqlParam.Add("@Email", userDetails.Email);

        if (_dapper.ExecuteSql(sql, sqlParam, System.Data.CommandType.StoredProcedure))
        {
            return Ok();
        }

        throw new Exception("Failed to update user profile details!");
    }

    [HttpDelete("User/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = "spUsers_Delete";

        DynamicParameters sqlParams = new DynamicParameters();
        sqlParams.Add("@UserId", userId);

        if(_dapper.ExecuteSql(sql, sqlParams, CommandType.StoredProcedure))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user!");
    }

}