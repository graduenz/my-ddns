using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

/// <summary>
/// Basic response body that comes from the GET DNS records on Cloudflare API.
/// </summary>
public class GetDnsRecordsResponse
{
    /// <summary>
    /// List of DNS records.
    /// </summary>
    public List<CloudflareDnsRecord>? Result { get; set; }
    /// <summary>
    /// Tells if the request was successful or not.
    /// </summary>
    public bool Success { get; set; }
}