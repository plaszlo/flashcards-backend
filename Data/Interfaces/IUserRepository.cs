using FlashcardBackend.Dtos;
using FlashcardBackend.Models;

namespace FlashcardBackend.Data.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetUsersAsync(int userId = 0);
    public Task<bool> UpdateUser(UserUpdateProfileDto userData);
    public Task<bool> DeleteUser(int userId);
    public Task<int> AddUser(UserRegistrationDto regForm);
    public Task<bool> RehashPassword(User user, string password);
}