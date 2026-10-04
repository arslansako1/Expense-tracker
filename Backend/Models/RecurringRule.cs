
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

public class RecurringRule
{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public int AccountId {get; set;}

    [ForeignKey("AccountId")]
    public Account? Account {get; set;}
    public decimal Amount {get; set;}
    public int CategoryId {get; set;}

    [ForeignKey("CategoryId")]
    public Category? Category {get; set;}
    public string Frequency {get; set;} = string.Empty;
    public DateTime NextRunDate {get; set;}
    public bool IsActive {get; set;}
    public bool IsDeleted {get; set;}

}

