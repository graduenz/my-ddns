using System.Net;
using FluentAssertions;
using MyDDNS.TestUtils;

namespace MyDDNS.Registrar.Cloudflare.Tests;

public class CloudflareIpAddressFetchStrategyTests
{
    [Fact]
    public void Ctor_WhenNullParams_Throws()
    {
        // Act
        var act = () => new CloudflareIpAddressFetchStrategy(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetIpAddressAsync_WhenIpCantBeExtracted_ReturnsIPNone()
    {
        // Arrange
        var strategy = new CloudflareIpAddressFetchStrategy(
            HttpClientFactoryMock.Create("this is absolutely invalid").Object
        );
        
        // Act
        var ip = await strategy.GetIpAddressAsync();
        
        // Assert
        ip.Should().Be(IPAddress.None);
    }

    [Fact]
    public async Task GetIpAddressAsync_Success()
    {
        // Arrange
        var strategy = new CloudflareIpAddressFetchStrategy(
            HttpClientFactoryMock.Create(GetCloudflareExampleResponse()).Object
        );
        
        // Act
        var ip = await strategy.GetIpAddressAsync();
        
        // Assert
        ip.ToString().Should().Be("189.113.244.7");
    }

    private static string GetCloudflareExampleResponse() => """
        fl=586f4
        h=cloudflare.com
        ip=189.113.244.7
        ts=1718974199.936
        visit_scheme=https
        uag=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36
        colo=NVT
        sliver=none
        http=http/3
        loc=BR
        tls=TLSv1.3
        sni=plaintext
        warp=off
        gateway=off
        rbi=off
        kex=X25519Kyber768Draft00
        """;
}