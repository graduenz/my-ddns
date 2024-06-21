using System.Net;
using FluentAssertions;
using Moq;
using MyDDNS.Core.IP;

namespace MyDDNS.Core.Tests.IP;

public class DefaultIpAddressProviderTests
{
    [Fact]
    public void Ctor_WhenNullParams_Throws()
    {
        // Act
        var act = () => new DefaultIpAddressProvider(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenEmptyFetchStrategies_Throws()
    {
        // Act
        var act = () => new DefaultIpAddressProvider(new List<IIpAddressFetchStrategy>());

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
            .Setup(m => m.GetIpAddressAsync())
            .ReturnsAsync(IPAddress.None);

        var provider = new DefaultIpAddressProvider(new[] { strategyMock.Object });

        // Act
        var act = () => provider.GetIpAddressAsync();

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
            .Setup(m => m.GetIpAddressAsync())
            .ReturnsAsync(IPAddress.Parse("10.10.10.10"));
        
        var provider = new DefaultIpAddressProvider(new[] { strategyMock.Object });
        
        // Act
        var ip = await provider.GetIpAddressAsync();
        
        // Assert
        ip.ToString().Should().Be("10.10.10.10");
    }
}