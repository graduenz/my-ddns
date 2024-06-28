namespace MyDDNS.Registrar.Cloudflare.Api.Requests;

/// <summary>
/// Basic request body that goes on the PATCH DNS record on Cloudflare API.
/// </summary>
public class PatchDnsRecordRequest
{
    /// <summary>
    /// Record name.
    /// </summary>
    /// <example><c>test.com</c></example>
    public string? Name { get; set; }
    /// <summary>
    /// Record type.
    /// </summary>
    /// <example><c>A</c></example>
    public string? Type { get; set; }
    /// <summary>
    /// Record content (IP address).
    /// </summary>
    /// <example><c>10.10.10.10</c></example>
    public string? Content { get; set; }
    /// <summary>
    /// Record is proxied or not.
    /// </summary>
    /// <example><c>false</c></example>
    public bool Proxied { get; set; }
    /// <summary>
    /// Record TTL.
    /// </summary>
    /// <example><c>3600</c></example>
    public int Ttl { get; set; }
}