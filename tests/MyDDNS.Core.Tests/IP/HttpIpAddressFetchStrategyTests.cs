using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MyDDNS.Core.IP;
using MyDDNS.TestUtils;

namespace MyDDNS.Core.Tests.IP;

public class HttpIpAddressFetchStrategyTests
{
    private static readonly ILogger<HttpIpAddressFetchStrategy>
        Logger = TestLoggerMock.Create<HttpIpAddressFetchStrategy>().Object;

    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [Logger, HttpClientFactoryMock.Create().Object, null!],
        [Logger, null!, GetTestIpProviders()],
        [null!, HttpClientFactoryMock.Create().Object, GetTestIpProviders()]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(ILogger<HttpIpAddressFetchStrategy> logger,
        IHttpClientFactory httpClientFactory, HttpIpAddressProviderList ipAddressProviders)
    {
        // Act
        var act = () => new HttpIpAddressFetchStrategy(logger, httpClientFactory, ipAddressProviders);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenEmptyIpProviders_Throws()
    {
        // Act
        var act = () => new HttpIpAddressFetchStrategy(Logger, HttpClientFactoryMock.Create().Object, new HttpIpAddressProviderList(new List<Uri>()));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("ipAddressProviders")
            .And.Message.Should().StartWith("At least one IP provider must be specified.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("some kind of error")]
    public async Task GetIpAddressAsync_WhenIpIsInvalid_ReturnsIPNone(string responseString)
    {
        // Arrange
        var strategy =
            new HttpIpAddressFetchStrategy(Logger, HttpClientFactoryMock.Create(responseString).Object,
                GetTestIpProviders());

        // Act
        var ip = await strategy.GetIpAddressAsync(CancellationToken.None);

        // Asset
        ip.Should().Be(IPAddress.None);
    }

    [Fact]
    public async Task GetIpAddressAsync_Success()
    {
        // Arrange
        var strategy =
            new HttpIpAddressFetchStrategy(Logger, HttpClientFactoryMock.Create().Object, GetTestIpProviders());

        // Act
        var ip = await strategy.GetIpAddressAsync(CancellationToken.None);

        // Asset
        ip.Should().BeEquivalentTo(IPAddress.Parse("10.10.10.10"));
    }

    private static HttpIpAddressProviderList GetTestIpProviders() => new(
    [
        new Uri("https://provider1.com"),
        new Uri("https://provider2.com")
    ]);
}