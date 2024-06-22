using System.Net;

namespace MyDDNS.Core.Dns;

public interface IDnsUpdater
{
    Task UpdateDnsAsync(IPAddress ip);
}