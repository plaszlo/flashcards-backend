using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardBackend.Helpers;

public class SqlHelper
{
    private readonly DataContextDapper _dapper;

    public SqlHelper(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
}