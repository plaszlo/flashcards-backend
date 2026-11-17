using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Data.Interfaces;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CardController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ICardRepository _cardRepo;

    public CardController(IConfiguration config, ICardRepository cardRepo)
    {
        _dapper = new DataContextDapper(config);
        _cardRepo = cardRepo;
    }

    [AllowAnonymous]
    [HttpGet("Dev/{userId}")]
    public async Task<IEnumerable<Card>> DevGetCards(int userId = 0)
    {
        if (userId == 0)
        {
            return await _dapper.LoadDataAsync<Card>("SELECT * FROM Cards");
        }

        string sql = "SELECT * FROM Cards WHERE UserId = @UserId";

        return await _dapper.LoadDataAsync<Card>(sql, new {userId});
    }
    
    [HttpGet()]
    public async Task<IEnumerable<Card>> GetCards()
    {
        return await _cardRepo.GetCards(User);
    }

    [HttpPost()]
    public async Task<Card> AddCard(CardContentDto cardContent)
    {
        Card? newCard = await _cardRepo.AddCard(User, cardContent);

        if(newCard != null)
            return newCard;

        throw new Exception("Failed to add new card!");
    }

    [HttpPatch("{cardId}")]
    public async Task<IActionResult> UpdateCard(CardContentDto cardContent, int cardId)
    {
        if(await _cardRepo.EditCard(User, cardContent, cardId))
        {
            return NoContent();
        }

        throw new Exception("Failed to update card!");
    }

    [HttpDelete("{cardId}")]
    public async Task<IActionResult> DeleteCard(int cardId)
    {
        if(await _cardRepo.DeleteCard(User, cardId))
        {
            return NoContent();
        }

        throw new Exception("Failed to delete card!");
    }
}