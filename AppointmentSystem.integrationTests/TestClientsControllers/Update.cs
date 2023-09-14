using System.Net;
using System.Net.Http.Json;
using appointmentSystem.Controllers.Features.Clients;
using appointmentSystem.Data;
using appointmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AppointmentSystem.integrationTests.TestClientsControllers;

public class UpdateClientTests
{
    private HttpClient _httpClient;
    private string? _databaseName = "UpdateClientTests";

    [SetUp]
    public void Setup()
    {
        var factory = new TestingWebApplicationFactory(_databaseName);
        _httpClient = factory.CreateClient();
    }

    [Test]
    public async Task UpdateClient_ClientExists_ReturnsOkAndUpdatesClientInDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(_databaseName).Options;
        await using var dbContext = new AppDbContext(options);
        var clientId = await AddClientToDatabase();
        
        var response = await _httpClient.PutAsJsonAsync($"/api/clients/{clientId}", new UpdateClientController.UpdateClientViewModel
        {
            Name = "New Name",
            Phone = "New Phone"
        });
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseString, Contains.Substring("New Name"));
            Assert.That(responseString, Contains.Substring("New Phone"));
            Assert.That(dbContext.Clients.Find(clientId)?.Name, Is.EqualTo("New Name"));
            Assert.That(dbContext.Clients.Find(clientId)?.Phone, Is.EqualTo("New Phone"));
        });
    }
    
    [Test]
    public async Task GetClient_ClientDoesntExist_Returns404()
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/clients/{Guid.NewGuid()}",
            new UpdateClientController.UpdateClientViewModel
            {
                Name = "New Name",
                Phone = "New Phone"
            });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetClient_ClientDoesntValid_Returns400()
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/clients/{Guid.NewGuid()}",
            new UpdateClientController.UpdateClientViewModel
            {
                Name = "New Name",
                Phone = "More than 12 chars"
            });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
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
