namespace MyDDNS.Cloudflare.Configuration;

public class DnsEntry
{
    public string? Name { get; set; }
    public bool Proxied { get; set; }
    public int Ttl { get; set; }
}