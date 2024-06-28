using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyDDNS.Core.IP;
using MyDDNS.TestUtils;

namespace MyDDNS.Core.Tests.IP;

public class DefaultIpAddressProviderTests
{
    private static readonly ILogger<DefaultIpAddressProvider> Logger = TestLogger.Create<DefaultIpAddressProvider>();

    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [Logger, null!],
        [null!, new List<IIpAddressFetchStrategy> { new Mock<IIpAddressFetchStrategy>().Object }]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(ILogger<DefaultIpAddressProvider> logger,
        IEnumerable<IIpAddressFetchStrategy> fetchStrategies)
    {
        // Act
        var act = () => new DefaultIpAddressProvider(logger, fetchStrategies);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenEmptyFetchStrategies_Throws()
    {
        // Act
        var act = () => new DefaultIpAddressProvider(Logger, new List<IIpAddressFetchStrategy>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("fetchStrategies")
            .And.Message.Should().StartWith("At least one IP fetch strategy must be specified.");
    }

    [Fact]
    public async Task GetIpAddressAsync_WhenAllStrategiesFail_Throws()
    {
        // Arrange
        var strategyMock = new Mock<IIpAddressFetchStrategy>();
        strategyMock
            .Setup(m => m.GetIpAddressAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(IPAddress.None);

        var provider = new DefaultIpAddressProvider(Logger, new[] { strategyMock.Object });

        // Act
        var act = () => provider.GetIpAddressAsync(CancellationToken.None);

        // Assert
        (await act.Should()
                .ThrowAsync<InvalidOperationException>())
            .And.Message.Should().Be("Failed to fetch the IP address: None of the strategies succeeded.");
    }

    [Fact]
    public async Task GetIpAddressAsync_Success()
    {
        // Arrange
        var strategyMock = new Mock<IIpAddressFetchStrategy>();
        strategyMock
            .Setup(m => m.GetIpAddressAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(IPAddress.Parse("10.10.10.10"));

        var provider = new DefaultIpAddressProvider(Logger, new[] { strategyMock.Object });

        // Act
        var ip = await provider.GetIpAddressAsync(CancellationToken.None);

        // Assert
        ip.ToString().Should().Be("10.10.10.10");
    }
}