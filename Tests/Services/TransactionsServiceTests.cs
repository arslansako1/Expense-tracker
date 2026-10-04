

using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyApiProject.Migrations;

public class TransactionsServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        return mock;
    }

    [Fact]
    public async Task CreateAsync_ValidIncome_ReturnsSuccess()
    {
        var context = CreateInMemoryContext();
        var mockUserManager = CreateMockUserManager();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var service = new TransactionService(mockUserManager.Object, context);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransactionRequest(
            AccountId: account.Id,
            Amount: 100,
            Type: "Income",
            Description: "Income test",
            CategoryId: 1,
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.AccountId.Should().Be(account.Id);
        result.Data.Amount.Should().Be(100);
        result.Data.Type.Should().Be("Income");
        result.Data.Description.Should().Be("Income test");
        result.Data.CategoryId.Should().Be(1);

        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        updatedAccount.Should().NotBeNull();
        updatedAccount.Balance.Should().Be(1100);


    }

    [Fact]
    public async Task CreateAsync_ValidExpense_DecreasesAccBalance()
    {
        var context = CreateInMemoryContext();
        var mockUserManager = CreateMockUserManager();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var service = new TransactionService(mockUserManager.Object, context);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransactionRequest(
            AccountId: account.Id,
            Amount: 100,
            Type: "Expense",
            Description: "Expense test",
            CategoryId: 1,
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.AccountId.Should().Be(account.Id);
        result.Data.Amount.Should().Be(100);
        result.Data.Type.Should().Be("Expense");
        result.Data.Description.Should().Be("Expense test");
        result.Data.CategoryId.Should().Be(1);

        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        updatedAccount.Should().NotBeNull();
        updatedAccount.Balance.Should().Be(900);


    }

    [Fact]
    public async Task CreateAsync_ExpenseExceedsBalance_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var mockUserManager = CreateMockUserManager();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var service = new TransactionService(mockUserManager.Object, context);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 50
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransactionRequest(
            AccountId: account.Id,
            Amount: 100,
            Type: "Expense",
            Description: "Expense test",
            CategoryId: 1,
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Not enough balance");


        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        updatedAccount.Should().NotBeNull();
        updatedAccount.Balance.Should().Be(50);


    }

    [Fact]
    public async Task CreateAsync_OtherUsersAccount_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var mockUserManager = CreateMockUserManager();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var service = new TransactionService(mockUserManager.Object, context);

        var account = new Account
        {
            UserId = "user-456",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransactionRequest(
            AccountId: account.Id,
            Amount: 100,
            Type: "Income",
            Description: "Income test",
            CategoryId: 1,
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own this account");


        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        updatedAccount!.Balance.Should().Be(1000);

    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUserTransactions()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account1 = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var account2 = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var category = new Category
        {
            UserId = "user-123",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.AddRange(account1, account2);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        context.Transactions.AddRange(
        new Transaction
        {
            UserId = "user-123",
            Amount = 100,
            Type = "Income",
            Description = "Mine 1",
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new Transaction
        {
            UserId = "user-123",
            Amount = 200,
            Type = "Income",
            Description = "Mine 2"
        ,
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new Transaction
        {
            UserId = "user-999",
            Amount = 300,
            Type = "Income",
            Description = "Not mine",
            AccountId = account2.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        }
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAsync(claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(t => t.UserId == "user-123");
    }


    [Fact]
    public async Task GetAllAsync_ExcludesSoftDeleted()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account1 = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var category = new Category
        {
            UserId = "user-123",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.Add(account1);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        context.Transactions.AddRange(
        new Transaction
        {
            UserId = "user-123",
            Amount = 100,
            Type = "Income",
            Description = "Mine 1",
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new Transaction
        {
            UserId = "user-123",
            Amount = 200,
            Type = "Income",
            Description = "Mine 2"
        ,
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        },
        new Transaction
        {
            UserId = "user-123",
            Amount = 300,
            Type = "Income",
            Description = "Not mine",
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true
        }
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAsync(claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(t => t.UserId == "user-123");
    }

    [Fact]
    public async Task GetAllAccAsync_ReturnsTransactionsForAcc()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account1 = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var account2 = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var category = new Category
        {
            UserId = "user-123",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.AddRange(account1, account2);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        context.Transactions.AddRange(
        new Transaction
        {
            UserId = "user-123",
            Amount = 100,
            Type = "Income",
            Description = "Mine 1",
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new Transaction
        {
            UserId = "user-123",
            Amount = 200,
            Type = "Income",
            Description = "Mine 2"
        ,
            AccountId = account1.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new Transaction
        {
            UserId = "user-123",
            Amount = 300,
            Type = "Income",
            Description = "Not mine",
            AccountId = account2.Id,
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        }
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAccAsync(account1.Id, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(t => t.AccountId == account1.Id);
    }

    [Fact]
    public async Task GetAllAccAsync_OtherUsersAcc_ReturnsFailure()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.Add(account);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAccAsync(account.Id, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own");
    }


    [Fact]
    public async Task DeleteAsync_ValidTransaction_ReturnsSuccess()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1100
        };

        var category = new Category
        {
            UserId = "user-123",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.Add(account);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        var transaction = new Transaction
        {
            UserId = "user-123",
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "To delete",
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Transactions.Add(transaction);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.DeleteAsync(transaction.Id, claimsPrincipal);

        result.Success.Should().BeTrue();

        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        updatedAccount!.Balance.Should().Be(1000);

        var updatedTransaction = await context.Transactions.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        updatedTransaction!.IsDeleted.Should().Be(true);
    }

    [Fact]
    public async Task DeleteAsync_OtherUsersTransaction_ReturnsFailure()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.Add(account);

        var transaction = new Transaction
        {
            UserId = "user-999",
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "To delete",
            CategoryId = 1,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Transactions.Add(transaction);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);


        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.DeleteAsync(transaction.Id, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own");
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsSuccess()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var category = new Category
        {
            UserId = "user-123",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.Add(account);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        var transaction = new Transaction
        {
            UserId = "user-123",
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "To delete",
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Transactions.Add(transaction);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        (
            Amount: 200,  
            Type: "Income",
            Description: "Updated",
            CategoryId: category.Id
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.UpdateAsync(transaction.Id, request, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Amount.Should().Be(200);

        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        updatedAccount!.Balance.Should().Be(1100);
        
    }

    [Fact]
    public async Task UpdateAsync_OtherUsersTransaction_ReturnsFailure()
    {
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "Test@example.com",
            UserName = "testTest"
        };

        mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

        var account = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var category = new Category
        {
            UserId = "user-999",
            Name = "Food",
            Icon = "🍔"
        };

        context.Accounts.Add(account);
        context.Categories.Add(category);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransactionService(mockUserManager.Object, context);

        var transaction = new Transaction
        {
            UserId = "user-999",
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "To delete",
            CategoryId = category.Id,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Transactions.Add(transaction);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        (
            Amount: 200,  
            Type: "Income",
            Description: "Updated",
            CategoryId: category.Id
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.UpdateAsync(transaction.Id, request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own");
    }
}
