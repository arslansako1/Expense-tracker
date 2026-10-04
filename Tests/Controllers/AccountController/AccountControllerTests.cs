

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Sdk;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountControllerTests(CustomWebApplicationFactory factory)
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

    [Fact]
    public async Task GetAllAccounts_NoToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllAccounts_ValidToken_ReturnsOk()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
    }
    
    [Fact]
    public async Task CreateAccount_ValidRequest_ReturnsOK()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts/createAccount", new
        {
            Name = "Checking",
            Type = "Checking",
            Currency = "$"
        }, cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AccountResponse>(cancellationToken: TestContext.Current.CancellationToken);
        body.Should().NotBeNull();
        body.Name.Should().Be("Checking");
        body.Type.Should().Be("Checking");
        body.Currency.Should().Be("$");
    
    }

    [Fact]
    public async Task CreateAccount_EmptyName_ReturnsBadRequest()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts/createAccount", new
        {
            Name = "",
            Type = "Checking",
            Currency = "$"
        }, cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);      
    }

    [Fact]
    public async Task CreateAccount_ThenGetAll_ReturnsCreatedAccount()
    {
        var email = $"Test-{Guid.NewGuid()}@example.com";
        var token = await SignupAndLogin(email);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/accounts/createAccount", new
        {
            Name = "Checking",
            Type = "Checking",
            Currency = "$"
        }, cancellationToken: TestContext.Current.CancellationToken);

        var getAllResponse = await _client.GetAsync("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);

        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var accounts = await getAllResponse.Content.ReadFromJsonAsync<List<AccountResponse>>(TestContext.Current.CancellationToken);
        accounts.Should().NotBeNull();
        accounts.Should().HaveCount(1);
        accounts[0].Name.Should().Be("Checking");

    }

    [Fact]
    public async Task CreateAccount_OtherUsersCannotSee()
    {
        var emailA = $"Test-{Guid.NewGuid()}@example.com";
        var tokenA = await SignupAndLogin(emailA);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", tokenA);

        await _client.PostAsJsonAsync("/api/accounts/createAccount", new
        {
            Name = "Checking",
            Type = "Checking",
            Currency = "$"
        }, cancellationToken: TestContext.Current.CancellationToken);

        var accountA = await _client.GetFromJsonAsync<List<AccountResponse>>("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);
        accountA.Should().HaveCount(1);

        var emailB = $"Test-{Guid.NewGuid()}@example.com";
        var tokenB = await SignupAndLogin(emailB);
        _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", tokenB);

        var accountB = await _client.GetFromJsonAsync<List<AccountResponse>>("/api/accounts/getAllAccounts", TestContext.Current.CancellationToken);
        accountB.Should().NotBeNull();
        accountB.Should().BeEmpty();
              
    }    
}