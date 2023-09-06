using appointmentSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace AppointmentSystem.integrationTests;

public class TestingWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string? _databaseName;

    public TestingWebApplicationFactory(string? databaseName = null)
    {
        _databaseName = databaseName ?? Guid.NewGuid().ToString();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContextPool<AppDbContext>(options => 
            {
                options.UseInMemoryDatabase(_databaseName);
            });
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            appContext.Database.EnsureCreated();
        });
    }
}