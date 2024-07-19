using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

/// <summary>
/// Basic response body that comes from the GET DNS records on Cloudflare API.
/// </summary>
public class GetDnsRecordsResponse : CloudflareResponse<List<CloudflareDnsRecord>>;