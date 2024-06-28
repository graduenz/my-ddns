using Microsoft.Extensions.Logging;

namespace MyDDNS.TestUtils;

public static class TestLogger
{
    public static ILogger<T> Create<T>()
    {
        var factory = LoggerFactory.Create(_ => { });
        return factory.CreateLogger<T>();
    }
}