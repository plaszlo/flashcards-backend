namespace FlashcardBackend.Models;

public class Card
{
    public int CardId {get; set;}
    public int UserId {get; set;}
    public string Term {get; set;} = string.Empty;
    public string Definition {get; set;} = string.Empty;
    public int Repetition {get; set;}
    public DateOnly? LastReview {get; set;}
    public DateOnly? NextReview {get; set;}
}