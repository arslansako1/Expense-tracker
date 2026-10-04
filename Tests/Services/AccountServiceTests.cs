using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class AccountServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsSuccess()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request = new CreateAccRequest("Checking", "Checking", "$");

        var result = await service.CreateAsync("user-123", request);

        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Checking");
        result.Data!.Type.Should().Be("Checking");
        result.Data!.Currency.Should().Be("$");
        result.Data!.Balance.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_SavesToDatabase()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request = new CreateAccRequest("Checking", "Checking", "$");

        await service.CreateAsync("user-123", request);

        var saved = await context.Accounts.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        saved.Should().NotBeNull();
        saved.Name.Should().Be("Checking");
        saved.Type.Should().Be("Checking");
        saved.Currency.Should().Be("$");
        saved.Balance.Should().Be(0);
        saved.UserId.Should().Be("user-123");
        saved.IsDeleted.Should().BeFalse();

    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesAuditLog()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request = new CreateAccRequest("Checking", "Checking", "$");
        
        await service.CreateAsync("user-123", request);

        var auditAccount = await context.AuditLogs.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        auditAccount.Should().NotBeNull();
        auditAccount.EntityType.Should().Be("Accounts");
        auditAccount.Action.Should().Be("Created");
        auditAccount.OldValue.Should().Be("N/A");
        auditAccount.NewValue.Should().Contain("Checking");
    }

    [Fact]
    public async Task CreateAsync_EmptyUserId_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request = new CreateAccRequest("Checking", "Checking", "$");

        var result = await service.CreateAsync("", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.Data.Should().BeNull();
    
    }

    [Fact]
    public async Task CreateAsync_EmptyName_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request = new CreateAccRequest("", "Checking", "$");

        var result = await service.CreateAsync("user-123", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.Data.Should().BeNull();
    
    }

    [Fact]
    public async Task GetAllAsync_ValidRequest_ReturnsSuccess()
    {
       var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request1 = new CreateAccRequest("A", "Checking", "$");
        var request2 = new CreateAccRequest("B", "Savings", "$");
        var request3 = new CreateAccRequest("C", "Checking", "$");

        await service.CreateAsync("user-1", request1);
        await service.CreateAsync("user-1", request2);
        await service.CreateAsync("user-2", request3);

        var result = await service.GetAllAsync("user-1");

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(a => a.UserId == "user-1");
    
    }

    [Fact]
    public async Task GetAllAsync_EmptyUserId_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request1 = new CreateAccRequest("A", "Checking", "$");
        var request2 = new CreateAccRequest("B", "Savings", "$");
        var request3 = new CreateAccRequest("C", "Checking", "$");

        await service.CreateAsync("user-1", request1);
        await service.CreateAsync("user-1", request2);
        await service.CreateAsync("user-2", request3);

        var result = await service.GetAllAsync("");

        result.Success.Should().BeFalse();
    
    }

    [Fact]
    public async Task GetAllAsync_EmptyList_ReturnsFailure()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

 

        var result = await service.GetAllAsync("user-123");

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    
    }

    [Fact]
    public async Task GetAllAsync_ExcludesSoftDeletedAccounts()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request1 = new CreateAccRequest("A", "Checking", "$");
        var request2 = new CreateAccRequest("B", "Savings", "$");

        var createdAcc1 = await service.CreateAsync("user-1", request1);
        var createdAcc2 = await service.CreateAsync("user-1", request2);

        await service.DeleteAsync("user-1", createdAcc1!.Data!.Id);

        var result = await service.GetAllAsync("user-1");

        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("B");
    
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        var context = CreateInMemoryContext();
        var service = new AccountService(context);

        var request1 = new CreateAccRequest("A", "Checking", "$");
        var request2 = new CreateAccRequest("B", "Savings", "$");

        var createdAcc1 = await service.CreateAsync("user-1", request1);
        await Task.Delay(100, TestContext.Current.CancellationToken);

        var createdAcc2 = await service.CreateAsync("user-1", request2);

        var result = await service.GetAllAsync("user-1");

        result.Data.Should().HaveCount(2);
        result.Data.First().Name.Should().Be("B");
        result.Data.Last().Name.Should().Be("A");

    
    }
}
