using MyDDNS.Core.Dns;

namespace MyDDNS.Registrar.Cloudflare.Configuration;

public class CloudflareDnsConfiguration : IDnsConfiguration
{
    public List<DnsEntry>? Dns { get; set; }
    public AuthConfiguration? Auth { get; set; }
    public string? ZoneIdentifier { get; set; }
}