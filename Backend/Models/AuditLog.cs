

public class AuditLog{

    public int Id {get; set;}
    public string EntityType {get; set;} = string.Empty;
    public int EntityId {get; set;}
    public string UserId {get; set;} = string.Empty;
    public string Action {get; set;} = string.Empty;
    public string OldValue {get; set;} = string.Empty;
    public string NewValue {get; set;} = string.Empty;
    public DateTime Timestamp {get; set;}

}

