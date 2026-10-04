

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Sdk;

public class TransactionControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TransactionControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
    }

    private async Task<string> SignupAndLogin(string email, string password = "Saloosa85.")
    {
        await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.AccessToken;
    }

    private async Task<AccountResponse> CreateAccount(string name, decimal initialBalance)
    {
        var response = await _client.PostAsJsonAsync("/api/accounts/createAccount", new
        {
            Name = name,
            Type = "Checking",
            Currency = "$"
        });

        var account = await response.Content.ReadFromJsonAsync<AccountResponse>();

        if (initialBalance > 0)
        {
            await _client.PostAsJsonAsync("/api/transactions/createTransaction", new
            {              
                AccountId = account!.Id,
                Amount = initialBalance,
                Type = "Income",
                Description = "Initial balance",
                CategoryId = 1,
                Date = DateTime.UtcNow
            });
        }

        return account!;
        
    }

    [Fact]
    public async Task GetAllTransactions_NoToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/transactions/getAllTransactions", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTransaction_ValidIncome_ReturnsOk()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var account = await CreateAccount("Checking", 1000);

        var response = await _client.PostAsJsonAsync("/api/transactions/createTransaction", new
        {
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "Test income",
            CategoryId = 1,
            Date = DateTime.UtcNow

        }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TransactionResponse>(cancellationToken: TestContext.Current.CancellationToken);
        body.Should().NotBeNull();
        body.Amount.Should().Be(100);
        body.Type.Should().Be("Income");
        body.Description.Should().Be("Test income");
        
    }
    
    [Fact]
    public async Task CreateTransaction_InsufficientFunds_ReturnsBadRequest()
    {   
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var account = await CreateAccount("Checking", 100);

        var response = await _client.PostAsJsonAsync("/api/transactions/createTransaction", new
        {
            AccountId = account.Id,
            Amount = 200,
            Type = "Expense",
            Description = "Test expense",
            CategoryId = 1,
            Date = DateTime.UtcNow

        }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task GetAllAccTransactions_OtherUsersAccount_ReturnsBadRequest()
    {
        var emailA = $"Test-{Guid.NewGuid()}@example.com";
        var tokenA = await SignupAndLogin(emailA);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", tokenA);

        var accountA = await CreateAccount("Checking", 1000);

        var emailB = $"Test-{Guid.NewGuid()}@example.com";
        var tokenB = await SignupAndLogin(emailB);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", tokenB);

        var responseB = await _client.GetAsync($"/api/transactions/getAllAccTransactions/{accountA.Id}", TestContext.Current.CancellationToken);

        responseB.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task CreateAccount_ThenTransaction_ThenGetAll_ReturnsTransaction()
    {   
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var account = await CreateAccount("Checking", 1000);

        await _client.PostAsJsonAsync("/api/transactions/createTransaction", new
        {
            AccountId = account.Id,
            Amount = 100,
            Type = "Income",
            Description = "Test income",
            CategoryId = 1,
            Date = DateTime.UtcNow

        }, TestContext.Current.CancellationToken);

        var getAllResponse = await _client.GetAsync("/api/transactions/getAllTransactions", TestContext.Current.CancellationToken);
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var transactions = await getAllResponse.Content.ReadFromJsonAsync<List<TransactionResponse>>(cancellationToken: TestContext.Current.CancellationToken);
        transactions.Should().NotBeNull();
        transactions.Should().HaveCount(2);
        transactions[0].Description.Should().Be("Test income");

        
    }
}