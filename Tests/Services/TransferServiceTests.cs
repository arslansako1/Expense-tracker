

using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyApiProject.Migrations;

public class TransferServiceTests
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
    public async Task CreateAsync_ValidTransfer_ReturnsSuccess()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var toAccount = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Amount.Should().Be(200);
        result.Data.FromAccountId.Should().Be(fromAccount.Id);
        result.Data.ToAccountId.Should().Be(toAccount.Id);
        result.Data.Description.Should().Be("Test transfer");

    }

    [Fact]
    public async Task CreateAsync_ValidTransfer_UpdateBothBalance()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var toAccount = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        await service.CreateAsync(request, claimsPrincipal);

        var updatedFromAccount = await context.Accounts.FindAsync(new object?[] { fromAccount.Id }, TestContext.Current.CancellationToken);
        var updatedToAccount = await context.Accounts.FindAsync(new object?[] { toAccount.Id }, TestContext.Current.CancellationToken);
        updatedFromAccount!.Balance.Should().Be(800);
        updatedToAccount!.Balance.Should().Be(700);

    }

    [Fact]
    public async Task CreateAsync_ValidTransfer_CreateTwoTransactions()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var toAccount = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        await service.CreateAsync(request, claimsPrincipal);

        var transactions = await context.Transactions.ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        transactions.Should().HaveCount(2);

        var checking = transactions.First(t => t.AccountId == fromAccount.Id);
        checking.Amount.Should().Be(-200);
        checking.Type.Should().Be("Transfer");

        var savings = transactions.First(t => t.AccountId == toAccount.Id);
        savings.Amount.Should().Be(200);
        savings.Type.Should().Be("Transfer");


    }

    [Fact]
    public async Task CreateAsync_InsufficientFunds_ReturnsFailure()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 100
        };

        var toAccount = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Not enough balance");

        var updatedFromAccount = await context.Accounts.FindAsync(new object?[] { fromAccount.Id }, TestContext.Current.CancellationToken);
        updatedFromAccount!.Balance.Should().Be(100);

        var updatedToAccount = await context.Accounts.FindAsync(new object?[] { toAccount.Id }, TestContext.Current.CancellationToken);
        updatedToAccount!.Balance.Should().Be(500);

    }


    [Fact]
    public async Task CreateAsync_InsufficientFunds_DoesNotChangeBalance()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 100
        };

        var toAccount = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        var updatedFromAccount = await context.Accounts.FindAsync(new object?[] { fromAccount.Id }, TestContext.Current.CancellationToken);
        updatedFromAccount!.Balance.Should().Be(100);

        var updatedToAccount = await context.Accounts.FindAsync(new object?[] { toAccount.Id }, TestContext.Current.CancellationToken);
        updatedToAccount!.Balance.Should().Be(500);

        var transactions = await context.Transactions.ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        transactions.Should().BeEmpty();

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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var toAccount = new Account
        {
            UserId = "user-999",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(fromAccount, toAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: toAccount.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own");

    }

    [Fact]
    public async Task CreateAsync_SameAccount_ReturnsFailure()
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

        var service = new TransferService(mockUserManager.Object, context);

        var account = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.AddRange(account);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: account.Id,
            ToAccountId: account.Id,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("same");

    }

    [Fact]
    public async Task CreateAsync_NonExistentAccount_ReturnsFailure()
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

        var service = new TransferService(mockUserManager.Object, context);

        var fromAccount = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };


        context.Accounts.Add(fromAccount);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreateTransferRequest(
            FromAccountId: fromAccount.Id,
            ToAccountId: 9999,
            Amount: 200,
            Description: "Test transfer",
            Date: DateTime.UtcNow
        );

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.CreateAsync(request, claimsPrincipal);

        result.Success.Should().BeFalse();

    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUserTransfers()
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

        var service = new TransferService(mockUserManager.Object, context);

        var acc1 = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var acc2 = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        var acc3 = new Account
        {
            UserId = "user-999",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        var acc4 = new Account
        {
            UserId = "user-999",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(acc1, acc2, acc3, acc4);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        context.Transfers.AddRange(
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = acc1.Id,
              ToAccountId = acc2.Id,
              Amount = 100,
              Description = "Mine 1",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = acc1.Id,
              ToAccountId = acc2.Id,
              Amount = 200,
              Description = "Mine 2",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-999",
              FromAccountId = acc3.Id,
              ToAccountId = acc4.Id,
              Amount = 300,
              Description = "Not mine",
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

        var service = new TransferService(mockUserManager.Object, context);

        var acc1 = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var acc2 = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(acc1, acc2);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        context.Transfers.AddRange(
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = acc1.Id,
              ToAccountId = acc2.Id,
              Amount = 100,
              Description = "Mine 1",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = acc1.Id,
              ToAccountId = acc2.Id,
              Amount = 200,
              Description = "Mine 2",
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
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAccAsync_ReturnsTransfersForAcc()
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

        var service = new TransferService(mockUserManager.Object, context);

        var checking = new Account
        {
            UserId = "user-123",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var savings = new Account
        {
            UserId = "user-123",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 500
        };

        context.Accounts.AddRange(checking, savings);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        context.Transfers.AddRange(
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = checking.Id,
              ToAccountId = savings.Id,
              Amount = 100,
              Description = "Mine 1",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = checking.Id,
              ToAccountId = savings.Id,
              Amount = 200,
              Description = "Mine 2",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-123",
              FromAccountId = savings.Id,
              ToAccountId = savings.Id,
              Amount = 300,
              Description = "Not mine",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          }
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAccAsync(checking.Id, claimsPrincipal);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(t => t.FromAccountId == checking.Id || t.ToAccountId == checking.Id);
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

        var checking = new Account
        {
            UserId = "user-999",
            Name = "Checking",
            Type = "Checking",
            Currency = "USD",
            Balance = 1000
        };

        var savings = new Account
        {
            UserId = "user-999",
            Name = "Savings",
            Type = "Savings",
            Currency = "USD",
            Balance = 1000
        };

        context.Accounts.AddRange(checking, savings);

        context.Transfers.AddRange(
          new Transfer
          {
              UserId = "user-999",
              FromAccountId = checking.Id,
              ToAccountId = savings.Id,
              Amount = 100,
              Description = "Mine 1",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-999",
              FromAccountId = checking.Id,
              ToAccountId = savings.Id,
              Amount = 200,
              Description = "Mine 2",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          },
          new Transfer
          {
              UserId = "user-999",
              FromAccountId = savings.Id,
              ToAccountId = checking.Id,
              Amount = 300,
              Description = "Not mine",
              Date = DateTime.UtcNow,
              CreatedAt = DateTime.UtcNow
          }
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new TransferService(mockUserManager.Object, context);

        var claimsPrincipal = new ClaimsPrincipal();

        var result = await service.GetAllAccAsync(checking.Id, claimsPrincipal);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("own");
    }
}
