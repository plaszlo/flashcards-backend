using System.ComponentModel.DataAnnotations;

namespace FlashcardBackend.Dtos;

public class UserUpdateProfileDto
{
    [Required]
    public string Name {get; set;}
    [Required]
    [EmailAddress]
    public string Email {get; set;}

    public UserUpdateProfileDto(){
        if(Name == null)
        {
            Name = "";
        }
        
        if(Email == null)
        {
            Email = "";
        }
    }
}