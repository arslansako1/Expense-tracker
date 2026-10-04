

using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

public class Transfer{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public decimal Amount {get; set;}
    public DateTime Date {get; set;}
    public string Description {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}
    public int FromAccountId {get; set;}

    [ForeignKey("FromAccountId")]
    public Account? FromAccount {get; set;}

    public int ToAccountId {get; set;}

    [ForeignKey("ToAccountId")]
    public Account? ToAccount {get; set;}
    public bool IsDeleted {get; set;}

}