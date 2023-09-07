using System.Net;
using System.Net.Http.Json;
using appointmentSystem.Controllers.Features.Clients;
using appointmentSystem.Data;
using appointmentSystem.Models.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AppointmentSystem.integrationTests.TestClientsControllers;

public class CreateClientTests
{
    private HttpClient _httpClient;
    private string? _databaseName = "CreateClientTest";

    [SetUp]
    public void Setup()
    {
        var factory = new TestingWebApplicationFactory(_databaseName);
        _httpClient = factory.CreateClient();
    }
    
    [Test]
    public async Task CreateClient_ClientIsValid_Returns200_AndSamePhoneClientIsBadRequest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(_databaseName).Options;
        await using var dbContext = new AppDbContext(options);
        var phone = "Phone";
        var response = await _httpClient.PostAsJsonAsync("/api/clients", new CreateClientController.CreateClientCommand
        {
            Name = "Name",
            Phone = phone
        });
        var responseSamePhone = await _httpClient.PostAsJsonAsync("/api/clients", new CreateClientController.CreateClientCommand
        {
            Name = "Name",
            Phone = phone
        });
        
        var responseString = await response.Content.ReadAsStringAsync();
        var responseSamePhoneString = await responseSamePhone.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseString, Contains.Substring("Name"));
            Assert.That(responseString, Contains.Substring($"{phone}"));
            Assert.That(dbContext.Clients.Count(), Is.EqualTo(1));
            Assert.That(responseSamePhone.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseSamePhoneString, Contains.Substring($"A client with the phone {phone} already exists."));
        });
    }
}