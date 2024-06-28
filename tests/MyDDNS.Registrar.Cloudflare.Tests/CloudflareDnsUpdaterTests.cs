using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;
using MyDDNS.Registrar.Cloudflare.Configuration;
using MyDDNS.TestUtils;

namespace MyDDNS.Registrar.Cloudflare.Tests;

public class CloudflareDnsUpdaterTests
{
    private static readonly ILogger<CloudflareDnsUpdater> Logger = TestLogger.Create<CloudflareDnsUpdater>();

    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [Logger, CreateApiAdapterMock().Object, null!],
        [Logger, null!, GetTestDomainConfigs()],
        [null!, CreateApiAdapterMock().Object, GetTestDomainConfigs()]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(ILogger<CloudflareDnsUpdater> logger, ICloudflareApiAdapter cloudflareApi,
        List<CloudflareDomainConfiguration> domains)
    {
        // Act
        var act = () => new CloudflareDnsUpdater(logger, cloudflareApi, domains);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenEmptyDomains_Throws()
    {
        // Act
        var act = () =>
            new CloudflareDnsUpdater(Logger, CreateApiAdapterMock().Object, new List<CloudflareDomainConfiguration>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("domains")
            .And.Message.Should().StartWith("At least one domain must be specified.");
    }

    [Fact]
    public async Task UpdateDnsAsync_Success()
    {
        // Arrange
        var cloudflareApiMock = CreateApiAdapterMock();
        var updater = new CloudflareDnsUpdater(Logger, cloudflareApiMock.Object, GetTestDomainConfigs());

        // Act
        await updater.UpdateDnsAsync(IPAddress.Parse("10.10.10.10"), CancellationToken.None);

        // Assert
        cloudflareApiMock.Verify(
            m => m.GetDnsRecordsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        cloudflareApiMock.Verify(
            m => m.PatchDnsRecordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<PatchDnsRecordRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static List<CloudflareDomainConfiguration> GetTestDomainConfigs() =>
    [
        new CloudflareDomainConfiguration(
            "api_token",
            "zone_identifier",
            "rdnz.dev",
            false,
            1
        )
    ];

    private static Mock<ICloudflareApiAdapter> CreateApiAdapterMock()
    {
        var mock = new Mock<ICloudflareApiAdapter>();

        mock
            .Setup(m => m.GetDnsRecordsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDnsRecordsResponse
            {
                Result =
                [
                    new CloudflareDnsRecord
                        { Id = "aaaabbbb11112222", Name = "rdnz.dev", Type = "A", Content = "11.11.11.11" }
                ],
                Success = true
            });

        mock
            .Setup(m => m.PatchDnsRecordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<PatchDnsRecordRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PatchDnsRecordResponse
            {
                Result = new CloudflareDnsRecord
                    { Id = "aaaabbbb11112222", Name = "rdnz.dev", Type = "A", Content = "10.10.10.10" },
                Success = true
            });

        return mock;
    }
}