

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public class ReportsService(UserManager<ApplicationUser> _userManager, AppDbContext _context) : IReportsService
{
    public async Task<ServiceResult<List<GetCategorieSpendResponse>>> GetSpendAsync(ClaimsPrincipal claimsPrincipal)
    {
       
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<GetCategorieSpendResponse>>.Fail("User not found");
        }

        var categories = await _context.Categories.
        Where(c => c.UserId == user.Id && c.IsDeleted == false).ToListAsync();

        if (categories is null)
        {
            return ServiceResult<List<GetCategorieSpendResponse>>.Fail("Categories not found");
        }

        var results = new List<GetCategorieSpendResponse>();

        foreach (var category in categories)
        {
            var transactions = await _context.Transactions
            .Where(t => t.CategoryId == category.Id
             && t.IsDeleted == false
             && t.Date.Month == DateTime.UtcNow.Month 
             && t.Date.Year == DateTime.UtcNow.Year).ToListAsync();
            
            var categoryName = category.Name;
            var createdAt = category.CreatedAt;
            var totalSpent = Math.Abs(transactions.Sum(t => t.Amount));

            results.Add(
                new GetCategorieSpendResponse
                (
                CategoryName: category.Name,
                TotalSpent: totalSpent,
                CreatedAt: createdAt
                ) 
            );
        }

        return ServiceResult<List<GetCategorieSpendResponse>>.Ok(results);
    }

    public async Task<ServiceResult<List<GetBalanceHistoryResponse>>> GetBalanceAsync(ClaimsPrincipal claimsPrincipal, int months = 1)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<GetBalanceHistoryResponse>>.Fail("User is null");
        }

        var accounts = await _context.Accounts
        .Where(a => a.UserId == user.Id && a.IsDeleted == false).ToListAsync();

        if (accounts is null)
        {
            return ServiceResult<List<GetBalanceHistoryResponse>>.Fail("Accounts not found");
        }
        
        var CurrentNetWorth = Math.Abs(accounts.Sum(a => a.Balance));


        var startDate = DateTime.UtcNow.AddMonths(-months);
        var endDate = DateTime.UtcNow;

        var allTransactions = await _context.Transactions
        .Where(t => t.UserId == user.Id).ToListAsync();

        var results = new List<GetBalanceHistoryResponse>();

        for (int i = months; i > 0; i--)
        {
            var monthDate = DateTime.UtcNow.AddMonths(-i);
            var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var netWorthAtMonthEnd = allTransactions.Where(t => t.Date <= monthEnd).Sum(t => t.Amount);

            results.Add(new GetBalanceHistoryResponse(
                Month: monthStart.ToString("MMMM yyyy"),
                Networth: netWorthAtMonthEnd
            ));
        }

        results.Add(new GetBalanceHistoryResponse(
            Month: "Current",
            Networth: CurrentNetWorth
        ));
   

        return ServiceResult<List<GetBalanceHistoryResponse>>.Ok(results);
    }

    public async Task<ServiceResult<GetIncomeExpenseResponse>> GetIncomeAsync(ClaimsPrincipal claimsPrincipal)
    {
        
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<GetIncomeExpenseResponse>.Fail("User is null");
        }

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var transactions = await _context.Transactions
        .Where(t => t.UserId == user.Id
         && t.IsDeleted == false
         && t.Date.Month == currentMonth
         && t.Date.Year == currentYear)
        .ToListAsync();

        var income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);

        var savings = income - expenses;
        var savingRates = income > 0 ? (savings / income) * 100 : 0;

        var response = new GetIncomeExpenseResponse(
            Income: income,
            Expenses: expenses,
            Savings: savings,
            SavingRates: savingRates
        );

        return ServiceResult<GetIncomeExpenseResponse>.Ok(response);

    }
}