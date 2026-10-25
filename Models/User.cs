namespace FlashcardBackend.Models;

public class User
{
    public int UserId {get; set;}
    public string Name {get; set;}
    public string Email {get; set;}

    public User()
    {
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