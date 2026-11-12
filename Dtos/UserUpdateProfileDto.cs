namespace FlashcardBackend.Dtos;

public class UserUpdateProfileDto
{
    public string Name {get; set;}
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