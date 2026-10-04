

public class Category{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string Icon {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}
    public bool IsDeleted {get; set;}
    public ICollection<Budget>? Budgets {get; set;}
}