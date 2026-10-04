using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Dynamic;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        
    
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<RecurringRule> RecurringRules => Set<RecurringRule>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transfer>()
        .HasOne(t => t.FromAccount)
        .WithMany(t => t.FromTransfer)
        .HasForeignKey(t => t.FromAccountId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transfer>()
        .HasOne(t => t.ToAccount)
        .WithMany(t => t.ToTransfer)
        .HasForeignKey(t => t.ToAccountId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Account>()
        .Property(a => a.RowVersion)
        .IsRowVersion();


    }
}