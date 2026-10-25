using System.Data;
using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
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

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(User user)
    {
        string sql = "spUsers_Upsert";

        DynamicParameters sqlParam = new DynamicParameters();
        sqlParam.Add("@Name", user.Name);
        sqlParam.Add("@Email", user.Email);

        if(user.UserId != 0)
        {
            sqlParam.Add("@UserId", user.UserId);
        }

        if(_dapper.ExecuteSql(sql, sqlParam, CommandType.StoredProcedure))
        {
            return Ok();
        }

        throw new Exception("Failed to upsert user!");
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