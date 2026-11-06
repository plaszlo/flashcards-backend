namespace FlashcardBackend.Dtos;

public class UserUpdateProfileDto
{
    public int UserId {get; set;}
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