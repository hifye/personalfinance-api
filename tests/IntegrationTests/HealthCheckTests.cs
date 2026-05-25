using System.Net;
using FluentAssertions;
using Xunit;

namespace PersonalFinance.IntegrationTests;

public class HealthCheckTests : BaseIntegrationTest
{
    public HealthCheckTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        // Act
        var response = await HttpClient.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task HealthCheckReady_ShouldReturnHealthy()
    {
        // Act
        var response = await HttpClient.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}
