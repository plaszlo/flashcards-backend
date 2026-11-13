using System.Security.Claims;
using FlashcardBackend.Dtos;
using FlashcardBackend.Models;

namespace FlashcardBackend.Data.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetUsersAsync(int userId = 0);
    public Task<User?> GetUserByEmail(string email);
    public Task<bool> UpdateUser(ClaimsPrincipal user, UserUpdateProfileDto userData);
    public Task<bool> DeleteUser(ClaimsPrincipal user);
    public Task<int> AddUser(UserRegistrationDto regForm);
    public Task<bool> RehashPassword(User user, string password);
}