namespace MyDDNS.Registrar.Cloudflare.Configuration;

public class CloudflareDomainConfiguration
{
    public CloudflareDomainConfiguration(string apiToken, string zoneIdentifier, string recordName,
        bool proxied, int ttl)
    {
        ApiToken = apiToken;
        ZoneIdentifier = zoneIdentifier;
        RecordName = recordName;
        Proxied = proxied;
        Ttl = ttl;
    }

    public string ApiToken { get; set; }
    public string ZoneIdentifier { get; set; }
    public string RecordName { get; set; }
    public bool Proxied { get; set; }
    public int Ttl { get; set; }
}