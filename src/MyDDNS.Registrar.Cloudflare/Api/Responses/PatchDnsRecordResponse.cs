using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

public class PatchDnsRecordResponse : CloudflareDnsRecord
{
    public bool Success { get; set; }
}