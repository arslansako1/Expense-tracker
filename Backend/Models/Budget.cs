

using System.ComponentModel.DataAnnotations.Schema;

public class Budget{

    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public int CategoryId {get; set;} 
    
    [ForeignKey("CategoryId")]
    public Category? Category {get; set;}
    public decimal MonthlyLimit {get; set;}
    public string Month {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}
    public bool IsDeleted {get; set;}



}



