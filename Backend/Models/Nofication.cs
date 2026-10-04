

public class Notification
{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public string Message {get; set;} = string.Empty;
    public bool IsRead {get; set;}
    public bool IsDeleted {get; set;}
    public DateTime CreatedAt {get; set;}
}

