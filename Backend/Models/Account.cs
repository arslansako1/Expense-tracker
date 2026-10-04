using System.Security.Principal;


using System.Transactions;

public class Account{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string Type {get; set;} = string.Empty;
    public decimal Balance {get; set;}
    public string Currency {get; set;} = string.Empty;

    public byte[]? RowVersion {get; set;}
    public DateTime CreatedAt {get; set;}
    public List<Transaction>? Transactions {get; set;}
    public List<Transfer>?ToTransfer {get; set;}
    public List<Transfer>? FromTransfer {get; set;}
    public bool IsDeleted {get; set;}



}
