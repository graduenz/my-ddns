using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

/// <summary>
/// Basic response body that comes from the PATCH DNS record on Cloudflare API.
/// </summary>
public class PatchDnsRecordResponse : CloudflareResponse<CloudflareDnsRecord>;