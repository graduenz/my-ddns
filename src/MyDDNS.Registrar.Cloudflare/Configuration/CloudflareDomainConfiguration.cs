namespace MyDDNS.Registrar.Cloudflare.Configuration;

/// <summary>
/// A record that contains the DNS updating configuration for a domain on Cloudflare.
/// </summary>
public class CloudflareDomainConfiguration
{
    /// <summary>
    /// Creates the <see cref="CloudflareDomainConfiguration"/>.
    /// </summary>
    /// <param name="apiToken">The API token.</param>
    /// <param name="zoneIdentifier">The zone identifier.</param>
    /// <param name="recordName">The record name.</param>
    /// <param name="proxied">Record is proxied or not.</param>
    /// <param name="ttl">Record TTL.</param>
    public CloudflareDomainConfiguration(string apiToken, string zoneIdentifier, string recordName,
        bool proxied, int ttl)
    {
        ApiToken = apiToken;
        ZoneIdentifier = zoneIdentifier;
        RecordName = recordName;
        Proxied = proxied;
        Ttl = ttl;
    }

    /// <summary>
    /// The API token.
    /// </summary>
    /// <example><c>1234567890abcdef1234567890abcdef12345</c></example>
    public string ApiToken { get; set; }
    /// <summary>
    /// The zone identifier.
    /// </summary>
    /// <example><c>123e4567-e89b-12d3-a456-426614174000</c></example>
    public string ZoneIdentifier { get; set; }
    /// <summary>
    /// The record name.
    /// </summary>
    /// <example><c>test.com</c></example>
    public string RecordName { get; set; }
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