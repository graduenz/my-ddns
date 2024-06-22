namespace MyDDNS.Registrar.Cloudflare.Configuration;

public class DnsEntry
{
    public DnsEntry(string name, bool proxied, int ttl)
    {
        Name = name;
        Proxied = proxied;
        Ttl = ttl;
    }

    public string Name { get; set; }
    public bool Proxied { get; set; }
    public int Ttl { get; set; }
}