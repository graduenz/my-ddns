namespace MyDDNS.Registrar.Cloudflare.Api.Requests;

public class PatchDnsRecordRequest
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public int Ttl { get; set; }
    public bool Proxied { get; set; }
}