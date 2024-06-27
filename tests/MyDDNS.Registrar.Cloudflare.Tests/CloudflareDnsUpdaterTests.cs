﻿using System.Net;
using FluentAssertions;
using Moq;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;
using MyDDNS.Registrar.Cloudflare.Configuration;

namespace MyDDNS.Registrar.Cloudflare.Tests;

public class CloudflareDnsUpdaterTests
{
    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [CreateApiAdapterMock().Object, null!],
        [null!, GetTestCloudflareDnsConfiguration()]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(ICloudflareApiAdapter cloudflareApi,
        CloudflareDnsConfiguration configuration)
    {
        // Act
        var act = () => new CloudflareDnsUpdater(cloudflareApi, configuration);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateDnsAsync_Success()
    {
        // Arrange
        var cloudflareApiMock = CreateApiAdapterMock();
        var updater = new CloudflareDnsUpdater(cloudflareApiMock.Object, GetTestCloudflareDnsConfiguration());

        // Act
        await updater.UpdateDnsAsync(IPAddress.Parse("10.10.10.10"), CancellationToken.None);

        // Assert
        cloudflareApiMock.Verify(
            m => m.GetDnsRecordsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);

        cloudflareApiMock.Verify(
            m => m.PatchDnsRecordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PatchDnsRecordRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CloudflareDnsConfiguration GetTestCloudflareDnsConfiguration() =>
        new(
            auth: new AuthConfiguration(
                "test@test.com",
                "test_token"
            ),
            zoneIdentifier: "a2b2c3d4e5f6",
            dns:
            [
                new DnsEntry("rdnz.dev", false, 1)
            ]
        );

    private static Mock<ICloudflareApiAdapter> CreateApiAdapterMock()
    {
        var mock = new Mock<ICloudflareApiAdapter>();

        mock
            .Setup(m => m.GetDnsRecordsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
            .Setup(m => m.PatchDnsRecordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PatchDnsRecordRequest>(),
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