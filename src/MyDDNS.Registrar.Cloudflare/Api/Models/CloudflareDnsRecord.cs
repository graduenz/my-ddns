namespace MyDDNS.Registrar.Cloudflare.Api.Models;

/// <summary>
/// Basic model of the DNS record on Cloudflare API.
/// </summary>
public class CloudflareDnsRecord
{
    /// <summary>
    /// Record ID.
    /// </summary>
    /// <example><c>27c4f7acb73349d1a57a36d7cf3c8b2a</c></example>
    public string? Id { get; set; }
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
}