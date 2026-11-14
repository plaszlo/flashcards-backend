using FlashcardBackend.Data;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[ApiController]
public class CardController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public CardController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Cards/{userId}")]
    public async Task<IEnumerable<Card>> GetCards(int userId = 0)
    {
        if (userId == 0)
        {
            return await _dapper.LoadDataAsync<Card>("SELECT * FROM Cards");
        }

        string sql = "SELECT * FROM Cards WHERE UserId = @UserId";

        return await _dapper.LoadDataAsync<Card>(sql, new {userId});
    }

    // [HttpPost("AddCard")]
}