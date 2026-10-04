

using Microsoft.EntityFrameworkCore;

public class RecurringRuleService(IServiceScopeFactory _scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var today = DateTime.UtcNow.Date;

                var recurringRules = await context.RecurringRules
                .Where(r => r.NextRunDate.Date == today && r.IsActive == true && r.IsDeleted == false).ToListAsync();

                foreach (var rule in recurringRules)
                {
                    var transaction = new Transaction
                    {
                        AccountId = rule.AccountId,
                        Amount = -rule.Amount,
                        Description = $"Recurring: {rule.Frequency}",
                        CategoryId = rule.CategoryId,
                        Type = "Expense",
                        Date = today,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Transactions.Add(transaction);

                    if (rule.Frequency == "Weekly")
                    {
                        rule.NextRunDate = rule.NextRunDate.AddDays(7);
                    }

                    else if (rule.Frequency == "Monthly")
                    {
                        rule.NextRunDate = rule.NextRunDate.AddMonths(1);          
                    }

                    else if (rule.Frequency == "Yearly")
                    {
                        rule.NextRunDate = rule.NextRunDate.AddYears(1);
                    }
                }

                await context.SaveChangesAsync();
            }


          await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
