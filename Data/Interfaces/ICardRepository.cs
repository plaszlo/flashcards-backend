using System.Security.Claims;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;

namespace FlashcardBackend.Data.Interfaces;

public interface ICardRepository
{
    public Task<IEnumerable<Card>> GetCards(ClaimsPrincipal user);
    public Task<bool> AddCard(ClaimsPrincipal user, CardContentDto cardContent);
    public Task<bool> EditCard(ClaimsPrincipal user, CardContentDto cardContent, int cardId);
    public Task<bool> DeleteCard(ClaimsPrincipal user, int cardId);
}