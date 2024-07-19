namespace MyDDNS.Registrar.Cloudflare.Api.Models;

/// <summary>
/// Basic model of an error on Cloudflare API.
/// </summary>
public class CloudflareError
{
    /// <summary>
    ///  Error code.
    /// </summary>
    public int Code { get; set; }
    /// <summary>
    /// Error message.
    /// </summary>
    public string? Message { get; set; }
}