using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

/// <summary>
/// Basic response body that comes from the PATCH DNS record on Cloudflare API.
/// </summary>
public class PatchDnsRecordResponse
{
    /// <summary>
    /// The updated DNS record.
    /// </summary>
    public CloudflareDnsRecord? Result { get; set; }
    /// <summary>
    /// Tells if the request was successful or not.
    /// </summary>
    public bool Success { get; set; }
}