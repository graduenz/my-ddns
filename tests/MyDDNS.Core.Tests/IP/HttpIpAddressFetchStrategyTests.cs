using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using MyDDNS.Core.IP;

namespace MyDDNS.Core.Tests.IP;

public class HttpIpAddressFetchStrategyTests
{
    private const string TestIp = "10.10.10.10";

    public static object[][] Ctor_WhenNullParams_Throws_Data() =>
    [
        [CreateHttpClientFactoryMock().Object, null!],
        [null!, GetTestIpProviders()]
    ];

    [Theory]
    [MemberData(nameof(Ctor_WhenNullParams_Throws_Data))]
    public void Ctor_WhenNullParams_Throws(IHttpClientFactory httpClientFactory, IEnumerable<Uri> ipProviders)
    {
        // Act
        var act = () => new HttpIpAddressFetchStrategy(httpClientFactory, ipProviders);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenEmptyIpProviders_Throws()
    {
        // Act
        var act = () => new HttpIpAddressFetchStrategy(CreateHttpClientFactoryMock().Object, new List<Uri>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("ipProviders")
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
            new HttpIpAddressFetchStrategy(CreateHttpClientFactoryMock(responseString).Object, GetTestIpProviders());

        // Act
        var ip = await strategy.GetIpAddressAsync();

        // Asset
        ip.Should().Be(IPAddress.None);
    }

    [Fact]
    public async Task GetIpAddressAsync_Success()
    {
        // Arrange
        var strategy = new HttpIpAddressFetchStrategy(CreateHttpClientFactoryMock().Object, GetTestIpProviders());

        // Act
        var ip = await strategy.GetIpAddressAsync();

        // Asset
        ip.Should().Be(IPAddress.None);
    }

    private static List<Uri> GetTestIpProviders() =>
    [
        new Uri("https://provider1.com"),
        new Uri("https://provider2.com")
    ];

    private static Mock<IHttpClientFactory> CreateHttpClientFactoryMock(string? responseString = TestIp)
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
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

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://127.0.0.1/")
        };

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();

        httpClientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return httpClientFactoryMock;
    }
}