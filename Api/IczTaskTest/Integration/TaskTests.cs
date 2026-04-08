using System.Net.Http.Json;
using IczTask;
using IczTaskTest.Integration.Infrastructure;
using IczTaskTest.Integration.Seeders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Task = IczTask.Models.Task;

namespace IczTaskTest.Integration;

public class TaskTests
{
    private readonly HttpClient _client;

    private readonly CustomWebApplicationFactory<Program> _factory = new();

    public TaskTests()
    {
        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }


    [SetUp]
    public void MockSetup()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is string seeder)
        {
            var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
            seederInstance
                .Seed(
                    _factory
                        .Services.CreateScope()
                        .ServiceProvider.GetRequiredService<ApplicationDbContext>()
                );
        }

        if (TestContext.CurrentContext.Test.Properties.Get("MockUser") is not string userInfo) return;
        var parts = userInfo.Split(';');
        _factory
            .Services.GetRequiredService<TestAuthHandlerUserProvider>().SetUser(parts[0], parts[1].Split(","));
    }


    [TearDown]
    public void MockTeardown()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder) return;
        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance
            .Clear(
                _factory
                    .Services.CreateScope()
                    .ServiceProvider.GetRequiredService<ApplicationDbContext>()
            );
    }


    [OneTimeTearDown]
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }


    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task Create()
    {
        var defaultPage = await _client.PostAsJsonAsync("/api/tasks",
            new Task { Name = "Task2", Description = "654321456", Done = false });
        defaultPage.EnsureSuccessStatusCode();
    }


    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task Update()
    {
        var defaultPage = await _client.PutAsJsonAsync("/api/tasks",
            new Task { Id = 2, Name = "Task2", Description = "654321456", Done = false });
        defaultPage.EnsureSuccessStatusCode();
    }


    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task GetById()
    {
        var dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var task = dbContext.Tasks.First();

        var defaultPage = await _client.GetAsync($"/api/tasks/{task.Id}");
        defaultPage.EnsureSuccessStatusCode();
    }

    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task GetAll()
    {
        var defaultPage = await _client.GetAsync("/api/tasks");
        defaultPage.EnsureSuccessStatusCode();
    }

    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task GetAll_FilterByName()
    {
        var response = await _client.GetAsync("/api/tasks?name=task");
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<Task>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.Count, Is.EqualTo(2));
        Assert.That(tasks.Select(t => t.Name), Is.EquivalentTo(new[] { "task2", "task3" }));
    }


    [Test]
    [Property("Seeder", "IczTaskTest.Integration.Seeders.DefaultSeeder")]
    [Property("MockUser", "admin;Admin")]
    public async System.Threading.Tasks.Task Delete()
    {
        var defaultPage = await _client.DeleteAsync("/api/tasks/1");
        defaultPage.EnsureSuccessStatusCode();
    }
}