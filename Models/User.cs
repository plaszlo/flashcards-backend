namespace FlashcardBackend.Models;

public class User
{
    public int UserId {get; set;}
    public string Name {get; set;}
    public string Email {get; set;}
    public byte[] PasswordHash {get; set;}
    public byte[] PasswordSalt {get; set;}

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

        if(PasswordHash == null)
        {
            PasswordHash = new byte[0];
        }

        if(PasswordSalt == null)
        {
            PasswordSalt = new byte[0];
        }
    }
}