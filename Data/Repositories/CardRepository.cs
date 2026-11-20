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

    public async Task<Card?> AddCard(ClaimsPrincipal user, CardContentDto cardContent)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        DateOnly nextReview = DateOnly.FromDateTime(DateTime.Today).AddDays(Random.Shared.Next(2, 4));

        string sql = @"
            INSERT INTO Cards (
                UserId, Term, Definition, NextReview
            ) 
            OUTPUT INSERTED.*
            VALUES (
                @UserId, @Term, @Definition, @NextReview
            )
        ";

        var sqlParams = new DynamicParameters();
        sqlParams.Add("@UserId", userId);
        sqlParams.Add("@Term", cardContent.Term);
        sqlParams.Add("@Definition", cardContent.Definition);
        sqlParams.Add("@NextReview", nextReview);

        return await _dapper.LoadDataSingleAsync<Card>(sql, sqlParams);
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

    public async Task<IEnumerable<Card>> GetPracticeCards(ClaimsPrincipal user)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = @"
            SELECT * FROM Cards
            WHERE UserId = @UserId
            AND NextReview >= CAST(GETUTCDATE() AS DATE)
            AND NextReview <= DATEADD(WEEK, 3, CAST(GETUTCDATE() AS DATE))
            ORDER BY NextReview ASC
        ";

        return await _dapper.LoadDataAsync<Card>(sql, new { userId });
    }
}