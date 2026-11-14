using FlashcardBackend.Data;
using FlashcardBackend.Data.Interfaces;

namespace FlashcardBackend.Data.Repositories;

public class CardRepository : ICardRepository
{
    private readonly DataContextDapper _dapper;

    public CardRepository(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
}