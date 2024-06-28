using Microsoft.Extensions.Logging;
using Moq;

namespace MyDDNS.TestUtils;

public static class TestLoggerMock
{
    public static Mock<ILogger<T>> Create<T>()
    {
        var mock = new Mock<ILogger<T>>();
        return mock;
    }
}