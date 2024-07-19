using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.ExpressionBuilders.Logging;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;
using MyDDNS.Registrar.Cloudflare.Configuration;
using MyDDNS.TestUtils;

namespace MyDDNS.Registrar.Cloudflare.Tests;

public class CloudflareDnsUpdaterTests
{
    private static readonly ILogger<CloudflareDnsUpdater> Logger = TestLoggerMock.Create<CloudflareDnsUpdater>().Object;

    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [Logger, CreateApiAdapterMock().Object, null!],
        [Logger, null!, GetTestDomainConfigs()],
        [null!, CreateApiAdapterMock().Object, GetTestDomainConfigs()]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(ILogger<CloudflareDnsUpdater> logger, ICloudflareApiAdapter cloudflareApi,
        List<CloudflareDomainConfig> domains)
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
            new CloudflareDnsUpdater(Logger, CreateApiAdapterMock().Object, new List<CloudflareDomainConfig>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("domains")
            .And.Message.Should().StartWith("At least one domain must be specified.");
    }

    [Theory]
    [InlineData(true, false, false, false, false, false, LogLevel.Error)]
    [InlineData(false, true, false, false, false, false, LogLevel.Warning)]
    [InlineData(false, false, true, false, false, false, LogLevel.Warning)]
    [InlineData(false, false, false, false, true, false, LogLevel.Error)]
    [InlineData(false, false, false, false, false, true, LogLevel.Error)]
    public async Task UpdateDnsAsync_Logs_As_Expected(
        bool nullGetDnsRecordsResponse,
        bool nullGetDnsRecordsList,
        bool emptyGetDnsRecordsList,
        bool failedGetDnsRecordsResponse,
        bool nullPatchDnsRecordResponse,
        bool failedPatchDnsRecordResponse,
        LogLevel logLevel)
    {
        // Arrange
        var loggerMock = TestLoggerMock.Create<CloudflareDnsUpdater>();

        var cloudflareApiMock = CreateApiAdapterMock(
            nullGetDnsRecordsResponse,
            nullGetDnsRecordsList,
            emptyGetDnsRecordsList,
            failedGetDnsRecordsResponse,
            nullPatchDnsRecordResponse,
            failedPatchDnsRecordResponse);

        var updater = new CloudflareDnsUpdater(loggerMock.Object, cloudflareApiMock.Object, GetTestDomainConfigs());

        // Act
        await updater.UpdateDnsAsync(IPAddress.Parse("10.10.10.10"), CancellationToken.None);

        // Assert
        loggerMock.Verify(Log.With.LogLevel(logLevel), Times.Once);
    }

    [Fact]
    public async Task UpdateDnsAsync_WhenIpDoesNotChange_LogsSkippingMessage()
    {
        // Arrange
        var loggerMock = TestLoggerMock.Create<CloudflareDnsUpdater>();
        var cloudflareApiMock = CreateApiAdapterMock();
        var updater = new CloudflareDnsUpdater(loggerMock.Object, cloudflareApiMock.Object, GetTestDomainConfigs());

        // Act
        await updater.UpdateDnsAsync(IPAddress.Parse("11.11.11.11"), CancellationToken.None);

        // Assert
        loggerMock.Verify(Log
            .With.LogLevel(LogLevel.Information)
            .And.LogMessage(m => m.StartsWith("Skipping update")), Times.Once);
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

    private static List<CloudflareDomainConfig> GetTestDomainConfigs() =>
    [
        new CloudflareDomainConfig(
            "api_token",
            "zone_identifier",
            "rdnz.dev",
            false,
            1
        )
    ];

    private static Mock<ICloudflareApiAdapter> CreateApiAdapterMock(
        bool nullGetDnsRecordsResponse = false,
        bool nullGetDnsRecordsList = false,
        bool emptyGetDnsRecordsList = false,
        bool failedGetDnsRecordsResponse = false,
        bool nullPatchDnsRecordResponse = false,
        bool failedPatchDnsRecordResponse = false
    )
    {
        var mock = new Mock<ICloudflareApiAdapter>();

        mock
            .Setup(m => m.GetDnsRecordsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullGetDnsRecordsResponse
                ? null
                : new GetDnsRecordsResponse
                {
                    Result = nullGetDnsRecordsList
                        ? null
                        : emptyGetDnsRecordsList
                            ? []
                            :
                            [
                                new CloudflareDnsRecord
                                    { Id = "aaaabbbb11112222", Name = "rdnz.dev", Type = "A", Content = "11.11.11.11" }
                            ],
                    Success = !failedGetDnsRecordsResponse
                });

        mock
            .Setup(m => m.PatchDnsRecordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<PatchDnsRecordRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullPatchDnsRecordResponse
                ? null
                : new PatchDnsRecordResponse
                {
                    Result = new CloudflareDnsRecord
                        { Id = "aaaabbbb11112222", Name = "rdnz.dev", Type = "A", Content = "10.10.10.10" },
                    Success = !failedPatchDnsRecordResponse
                });

        return mock;
    }
}