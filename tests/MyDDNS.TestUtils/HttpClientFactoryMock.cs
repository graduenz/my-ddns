using System.Net;
using Moq;
using Moq.Protected;

namespace MyDDNS.TestUtils;

public static class HttpClientFactoryMock
{
    private const string TestIp = "10.10.10.10";
    
    public static Mock<IHttpClientFactory> Create(string? responseString = TestIp)
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