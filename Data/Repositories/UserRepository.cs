using Dapper;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FlashcardBackend.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContextDapper _dapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserRepository(IConfiguration config, IPasswordHasher<User> passwordHasher)
    {
        _dapper = new DataContextDapper(config);
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(int userId = 0)
    {
        if (userId == 0)
        {
            return await _dapper.LoadDataAsync<User>("SELECT * FROM Users");
        }

        const string sql = "SELECT * FROM Users WHERE UserId = @UserId";
        return await _dapper.LoadDataAsync<User>(sql, new { UserId = userId });
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        string sql = "SELECT * FROM Users WHERE Email = @Email";

        return await _dapper.LoadDataSingleAsync<User>(sql, new { email });
    }

    public async Task<bool> UpdateUser(ClaimsPrincipal user, UserUpdateProfileDto userData)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = @"
            UPDATE Users
            SET Name = @Name,
                Email = @Email
            WHERE UserId = @UserId";

        var sqlParams = new DynamicParameters();
        sqlParams.Add("@Name", userData.Name);
        sqlParams.Add("@Email", userData.Email);
        sqlParams.Add("@UserId", userId);

        return await _dapper.ExecuteSqlAsync(sql, sqlParams);
    }

    public async Task<bool> DeleteUser(ClaimsPrincipal user)
    {
        string? claimValue = user.FindFirst("userId")?.Value;
        int.TryParse(claimValue, out int userId);

        string sql = "DELETE FROM Users WHERE UserId = @UserId";

        return await _dapper.ExecuteSqlAsync(sql, new {userId});
    }

    public async Task<int> AddUser(UserRegistrationDto regForm)
    {
        var user = new User
        {
            Name = regForm.Name,
            Email = regForm.Email,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, regForm.Password);

        string sql = @"
            INSERT INTO Users (
                [Name],
                [Email],
                [PasswordHash]
            ) 
            OUTPUT INSERTED.UserId
            VALUES (
                @Name, 
                @Email,
                @PasswordHash
            )";

        var sqlParamaters = new DynamicParameters();
        sqlParamaters.Add("@Name", user.Name);
        sqlParamaters.Add("@Email", user.Email);
        sqlParamaters.Add("@PasswordHash", user.PasswordHash);

        return await _dapper.LoadDataSingleAsync<int>(sql, sqlParamaters);
    }

    public async Task<bool> RehashPassword(User user, string password)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        string sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Email = @Email";

        var sqlParameters = new DynamicParameters();
        sqlParameters.Add("@PasswordHash", user.PasswordHash);
        sqlParameters.Add("@Email", user.Email);

        return await _dapper.ExecuteSqlAsync(sql, sqlParameters);
    }
}