namespace MyDDNS.Registrar.Cloudflare.Api.Models;

public class CloudflareDnsRecord
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Content { get; set; }
}