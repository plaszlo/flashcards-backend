using System.ComponentModel.DataAnnotations;

namespace FlashcardBackend.Dtos;

public class UserRegistrationDto
{
    [Required]
    public string Name {get; set;}
    [Required]
    [EmailAddress]
    public string Email {get; set;}
    [Required]
    [MinLength(8)]
    public string Password {get; set;}
    [Required]
    public string PasswordConfirm {get; set;}

    public UserRegistrationDto()
    {
        if(Name == null)
        {
            Name = "";
        }
        if(Email == null)
        {
            Email = "";
        }
        if(Password == null)
        {
            Password = "";
        }
        if(PasswordConfirm == null)
        {
            PasswordConfirm = "";
        }
    }
}