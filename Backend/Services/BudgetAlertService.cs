

using Microsoft.EntityFrameworkCore;

public class BudgetAlertService(IServiceScopeFactory _scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var budgets = await context.Budgets.Where(b => b.IsDeleted == false).Include(b => b.Category).ToListAsync();
  
                foreach(var budget in budgets)
                {
                    var totalSpent = await context.Transactions.Where(t => t.CategoryId == budget.CategoryId
                    && t.Date.Year == DateTime.UtcNow.Year
                    && t.Date.Month == DateTime.UtcNow.Month
                    && t.IsDeleted == false)
                    .SumAsync(t => t.Amount);

                    if (Math.Abs(totalSpent) > budget.MonthlyLimit)
                    {
                        var nofication = new Notification
                        {
                            UserId = budget.UserId,
                            Message = $"⚠️ Budget exceeded! You've spent {totalSpent:C} of your {budget.MonthlyLimit:C} budget for {budget.Category?.Name ?? "this category"}.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };

                        context.Notifications.Add(nofication);
                    }
                }

                await context.SaveChangesAsync();           
            }    

          await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}