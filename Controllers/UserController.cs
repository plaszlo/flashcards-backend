using FlashcardBackend.Data;
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

    [HttpGet("Test")]
    public DateTime Test()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
}