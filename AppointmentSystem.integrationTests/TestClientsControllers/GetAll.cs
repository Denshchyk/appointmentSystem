using System.Net;
using System.Net.Http.Json;
using appointmentSystem.Controllers.Features.Clients;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AppointmentSystem.integrationTests.TestClientsControllers;

public class GetAllClientsTests
{
    private HttpClient _httpClient;
    private string? _databaseName = "GetAllClientsTests";

    [SetUp]
    public void Setup()
    {
        var factory = new TestingWebApplicationFactory(_databaseName);
        _httpClient = factory.CreateClient();
    }
    
    const int numberOfClients = 3;
    [Test]
    public async Task GetArticles_ThreeClientsExists_ReturnsListWithThreeClients_Returns200()
    {
        var clientsId = await AddClientsToDatabase();
        var response = await _httpClient.GetAsync("/api/clients");
        
        var clients = await response.Content.ReadFromJsonAsync<List<GetAllClientsController.GetAllClientsQuery>>();
        Assert.That(numberOfClients, Is.EqualTo(clients.Count));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public async Task<List<Guid>> AddClientsToDatabase()
    {
        var clientsId = new List<Guid>();

        var clientsToAdd = new List<Client>
        {
            new Client
            {
                Name = "Client 1",
                Phone = "Phone 1"
            },
            new Client
            {
                Name = "Client 2",
                Phone = "Phone 2"
            },
            new Client
            {
                Name = "Client 3",
                Phone = "Phone 3"
            }
        };

        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(_databaseName).Options;

        await using var dbContext = new AppDbContext(options);

        foreach (var client in clientsToAdd)
        {
            var clientId = await AddClientToDatabase(client, dbContext);
            clientsId.Add(clientId);
        }
        return clientsId;
    }
    private async Task<Guid> AddClientToDatabase(Client client, AppDbContext dbContext)
    {
        var clientId = Guid.NewGuid();
        client.Id = clientId;

        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();

        return clientId;
    }
}