

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Sdk;

public class TransferControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TransferControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAllTransfers_NoToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/transfers/getAllTransfers", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateTransfers_ValidRequest_ReturnsOK()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var fromAccount = await CreateAccount("Checking", 1000);
        var toAccount = await CreateAccount("Savings", 500);

        var response = await _client.PostAsJsonAsync("/api/transfers/createTransfer",new
        {
            FromAccountId = fromAccount.Id,
            ToAccountId = toAccount.Id,
            Amount = 200,
            Description = "Test transfer",
            Date = DateTime.UtcNow
            
        }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);


        var body = await response.Content.ReadFromJsonAsync<TransferResponse>(cancellationToken: TestContext.Current.CancellationToken);
        body.Should().NotBeNull();
        body.Amount.Should().Be(200);
        body.FromAccountId.Should().Be(fromAccount.Id);
        body.ToAccountId.Should().Be(toAccount.Id);
        body.Description.Should().Be("Test transfer");
    
    }

    [Fact]
    public async Task CreateTransfer_InsufficientFunds_ReturnsBadRequest()
    {   
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var fromAccount = await CreateAccount("Checking", 100);
        var toAccount = await CreateAccount("Savings", 1000);

        var response = await _client.PostAsJsonAsync("/api/transfers/createTransfer",new
        {
            FromAccountId = fromAccount.Id,
            ToAccountId = toAccount.Id,
            Amount = 200,
            Description = "Test transfer",
            Date = DateTime.UtcNow
            
        }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task CreateTransfer_SameAccount_ReturnsBadRequest()
    {
      
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var account = await CreateAccount("Checking", 1000);

        var response = await _client.PostAsJsonAsync("/api/transfers/createTransfer",new
        {
            FromAccountId = account.Id,
            ToAccountId = account.Id,
            Amount = 200,
            Description = "Test transfer",
            Date = DateTime.UtcNow
            
        }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task CreateTransfer_ThenCheckBalances_UpdatesBoth()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var fromAccount = await CreateAccount("Checking", 1000);
        var toAccount = await CreateAccount("Savings", 500);

        await _client.PostAsJsonAsync("/api/transfers/createTransfer",new
        {
            FromAccountId = fromAccount.Id,
            ToAccountId = toAccount.Id,
            Amount = 200,
            Description = "Test transfer",
            Date = DateTime.UtcNow
            
        }, TestContext.Current.CancellationToken);

        var response = await _client.GetAsync("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountResponse>>(cancellationToken: TestContext.Current.CancellationToken);

        var updatedFrom =  accounts!.First(a => a.Id == fromAccount.Id);
        var updatedTo =  accounts!.First(a => a.Id == toAccount.Id);

        updatedFrom.Balance.Should().Be(800);
        updatedTo.Balance.Should().Be(700);
           
    }    
}