
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;

    public List<Account>? Accounts {get; set;}

    public List<Budget>? Budgets {get; set;}

    public List<Category>? Categories {get; set;}

    public List<Notification>? Notifications {get; set;}

    public List<AuditLog>? AuditLog {get; set;}
}