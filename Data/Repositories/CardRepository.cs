using System.Security.Claims;
using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Data.Interfaces;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;

namespace FlashcardBackend.Data.Repositories;

public class CardRepository : ICardRepository
{
    private readonly DataContextDapper _dapper;

    public CardRepository(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    public async Task<IEnumerable<Card>> GetCards(ClaimsPrincipal user)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = "SELECT * FROM Cards WHERE UserId = @UserId";

        return await _dapper.LoadDataAsync<Card>(sql, new {userId});
    }

    public async Task<bool> AddCard(ClaimsPrincipal user, CardContentDto cardContent)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = @"
            INSERT INTO Cards (
                UserId, Term, Definition
            ) VALUES (
                @UserId, @Term, @Definition
            )
        ";

        var sqlParams = new DynamicParameters();
        sqlParams.Add("@UserId", userId);
        sqlParams.Add("@Term", cardContent.Term);
        sqlParams.Add("@Definition", cardContent.Definition);

        return await _dapper.ExecuteSqlAsync(sql, sqlParams);
    }

    public async Task<bool> EditCard(ClaimsPrincipal user, CardContentDto cardContent, int cardId)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = @"
            UPDATE Cards
            SET Term = @Term,
                Definition = @Definition
            WHERE CardId = @CardId AND UserId = @UserId
        ";

        var sqlParams = new DynamicParameters();
        sqlParams.Add("@UserId", userId);
        sqlParams.Add("@Term", cardContent.Term);
        sqlParams.Add("@Definition", cardContent.Definition);
        sqlParams.Add("@CardId", cardId);

        return await _dapper.ExecuteSqlAsync(sql, sqlParams);
    }

    public async Task<bool> DeleteCard(ClaimsPrincipal user, int cardId)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = "DELETE FROM Cards WHERE CardId = @CardId AND UserId = @UserId";

        var sqlParams = new DynamicParameters();
        sqlParams.Add("@CardId", cardId);
        sqlParams.Add("@UserId", userId);

        return await _dapper.ExecuteSqlAsync(sql, sqlParams);
    }
}