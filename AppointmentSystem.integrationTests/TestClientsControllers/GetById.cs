using System.Net;
using appointmentSystem.Data;
using appointmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AppointmentSystem.integrationTests.TestClientsControllers;

public class GetClientByIdTests
{
    private HttpClient _httpClient;
    private string? _databaseName = "GetClientByIdTests";
    
    [SetUp]
    public void Setup()
    {
        var factory = new TestingWebApplicationFactory(_databaseName);
        _httpClient = factory.CreateClient();
    }

    [Test]
    public async Task GetClientById_ClientDoesntExist_Returns404()
    {
        var response = await _httpClient.GetAsync($"/api/clients/{Guid.NewGuid()}");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetClientById_ClientExists_Returns200()
    {
        var clientId = await AddClientToDatabase();

        var response = await _httpClient.GetAsync($"/api/clients/{clientId}");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private async Task<Guid> AddClientToDatabase()
    {
        var clientId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId,
            Name = "Test Name",
            Phone = "Test Phone"
        };
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(_databaseName).Options;
        await using var dbContext = new AppDbContext(options);
        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();
        return clientId;
    }
}