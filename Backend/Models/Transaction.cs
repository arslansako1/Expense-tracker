
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction{
    
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public int AccountId {get; set;}

    [ForeignKey("AccountId")]
    public Account? Account {get; set;}
    public decimal Amount {get; set;}
    public string Description {get; set;} = string.Empty;
    public int? CategoryId {get; set;}

    [ForeignKey("CategoryId")]
    public Category? Category {get; set;}
    public int? TransferId {get; set;}
    public string Type {get; set;} = string.Empty;
    public bool IsDeleted {get; set;}
    public DateTime Date {get; set;}
    public DateTime CreatedAt {get; set;}


}



