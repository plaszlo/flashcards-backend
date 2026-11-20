namespace FlashcardBackend.Dtos;

public class CardScheduleDto
{
    public int CardId {get; set;}
    public int UserId {get; set;}
    public string Term {get; set;} = string.Empty;
    public string Definition {get; set;} = string.Empty;
}