
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

public class AuctionBusTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory;
  private readonly HttpClient _httpClient;
  private ITestHarness _testHarness;

  public AuctionBusTests(CustomWebAppFactory factory)
  {
    _factory = factory;
    _httpClient = factory.CreateClient();
    _testHarness = factory.Services.GetTestHarness();
  }

  [Fact]
  public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
  {
    // Arrange
    var auction = GetAuctionForCreate();
    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

    // Act
    var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

    // Assert
    response.EnsureSuccessStatusCode();
    Assert.True(await _testHarness.Published.Any<AuctionCreated>());
  }

  public Task InitializeAsync() => Task.CompletedTask;

  public Task DisposeAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
    DbHelper.ReinitializeDbForTests(db);

    return Task.CompletedTask;
  }

  private static CreateAuctionDto GetAuctionForCreate()
  {
    return new CreateAuctionDto
    {
      Make = "test",
      Model = "testModel",
      ImageUrl = "test",
      Color = "test",
      Mileage = 10,
      Year = 10,
      ReservePrice = 10
    };
  }
}
