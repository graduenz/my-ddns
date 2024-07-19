﻿using System.Net;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using MyDDNS.Registrar.Cloudflare.Api;
using MyDDNS.Registrar.Cloudflare.Api.Requests;

namespace MyDDNS.Registrar.Cloudflare.Tests.Api;

public class DefaultCloudflareApiAdapterTests
{
    private const string ApiToken = "test_token";
    private const string ZoneIdentifier = "test_zone_identifier";
    private const string RecordName = "test_record_name";
    private const string RecordId = "test_record_id";

    [Fact]
    public void Ctor_WhenNullParams_Throws()
    {
        // Act
        var act = () => new DefaultCloudflareApiAdapter(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetDnsRecordsAsync_WhenResponseIsInvalid_Throws()
    {
        // Arrange
        var mock = CreateHttpClientFactoryMock(HttpStatusCode.OK, "[]");
        var api = new DefaultCloudflareApiAdapter(mock.Object);

        // Act
        var act = () => api.GetDnsRecordsAsync(ApiToken, ZoneIdentifier, RecordName, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task GetDnsRecordsAsync_Success()
    {
        // Arrange
        var responseString = """
                             {
                               "result": [
                                 {
                                   "id": "test_record_id",
                                   "zone_id": "test_zone_id",
                                   "zone_name": "rdnz.dev",
                                   "name": "rdnz.dev",
                                   "type": "A",
                                   "content": "11.11.11.11",
                                   "proxiable": true,
                                   "proxied": false,
                                   "ttl": 1,
                                   "locked": false,
                                   "meta": {
                                     "auto_added": false,
                                     "managed_by_apps": false,
                                     "managed_by_argo_tunnel": false
                                   },
                                   "comment": null,
                                   "tags": [],
                                   "created_on": "2024-05-11T13:45:46.574363Z",
                                   "modified_on": "2024-05-11T13:47:56.526634Z"
                                 }
                               ],
                               "success": true,
                               "errors": [],
                               "messages": [],
                               "result_info": {
                                 "page": 1,
                                 "per_page": 100,
                                 "count": 1,
                                 "total_count": 1,
                                 "total_pages": 1
                               }
                             }
                             """;
        var mock = CreateHttpClientFactoryMock(HttpStatusCode.OK, responseString);
        var api = new DefaultCloudflareApiAdapter(mock.Object);

        // Act
        var response = await api.GetDnsRecordsAsync(ApiToken, ZoneIdentifier, RecordName, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response!.Result.Should().NotBeNull().And.HaveCount(1);
        response.Result![0].Content.Should().Be("11.11.11.11");
    }

    [Fact]
    public async Task PatchDnsRecordAsync_WhenResponseIsInvalid_Throws()
    {
        // Arrange
        var mock = CreateHttpClientFactoryMock(HttpStatusCode.OK, null);
        var api = new DefaultCloudflareApiAdapter(mock.Object);

        var payload = new PatchDnsRecordRequest
            { Content = "10.10.10.10", Name = "rdnz.dev", Type = "A", Ttl = 1, Proxied = false };

        // Act
        var act = () => api.PatchDnsRecordAsync(ApiToken, ZoneIdentifier, RecordId, payload, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task PatchDnsRecordAsync_Success()
    {
        // Arrange
        var responseString = """
                             {
                               "result": {
                                 "id": "test_record_id",
                                 "zone_id": "test_zone_id",
                                 "zone_name": "rdnz.dev",
                                 "name": "rdnz.dev",
                                 "type": "A",
                                 "content": "10.10.10.10",
                                 "proxiable": true,
                                 "proxied": false,
                                 "ttl": 1,
                                 "locked": false,
                                 "meta": {
                                   "auto_added": false,
                                   "managed_by_apps": false,
                                   "managed_by_argo_tunnel": false
                                 },
                                 "comment": null,
                                 "tags": [],
                                 "created_on": "2024-05-11T13:45:46.574363Z",
                                 "modified_on": "2024-05-11T13:47:56.526634Z"
                               },
                               "success": true,
                               "errors": [],
                               "messages": []
                             }
                             """;
        var mock = CreateHttpClientFactoryMock(HttpStatusCode.OK, responseString);
        var api = new DefaultCloudflareApiAdapter(mock.Object);

        var payload = new PatchDnsRecordRequest
            { Content = "10.10.10.10", Name = "rdnz.dev", Type = "A", Ttl = 1, Proxied = false };

        // Act
        var response = await api.PatchDnsRecordAsync(ApiToken, ZoneIdentifier, RecordId, payload, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response!.Result.Should().NotBeNull();
        response.Result!.Content.Should().Be("10.10.10.10");
    }

    private static Mock<IHttpClientFactory> CreateHttpClientFactoryMock(
        HttpStatusCode httpStatusCode = HttpStatusCode.OK, string? responseString = null)
    {
        var responseMessage = new HttpResponseMessage(httpStatusCode)
        {
            Content = responseString == null ? null : new StringContent(responseString)
        };

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns(Task.FromResult(responseMessage))
            .Verifiable();

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();

        httpClientFactoryMock
            .Setup(m => m.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(httpMessageHandlerMock.Object));

        return httpClientFactoryMock;
    }
}