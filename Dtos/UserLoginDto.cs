using System.ComponentModel.DataAnnotations;

namespace FlashcardBackend.Dtos;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email {get; set;}
    [Required]
    public string Password {get; set;}

    public UserLoginDto()
    {
        if(Email == null)
        {
            Email = "";
        }

        if(Password == null)
        {
            Password = "";
        }
    }
}