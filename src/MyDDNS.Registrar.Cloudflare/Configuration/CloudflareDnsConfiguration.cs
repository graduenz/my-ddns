using MyDDNS.Core.Dns;

namespace MyDDNS.Registrar.Cloudflare.Configuration;

public class CloudflareDnsConfiguration
{
    public CloudflareDnsConfiguration(List<DnsEntry> dns, AuthConfiguration auth, string zoneIdentifier)
    {
        Dns = dns;
        Auth = auth;
        ZoneIdentifier = zoneIdentifier;
    }

    public List<DnsEntry> Dns { get; }
    public AuthConfiguration Auth { get; }
    public string ZoneIdentifier { get; }
}