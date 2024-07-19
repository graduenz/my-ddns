using MyDDNS.Registrar.Cloudflare.Api.Models;

namespace MyDDNS.Registrar.Cloudflare.Api.Responses;

/// <summary>
/// Abstract model for responses that come from Cloudflare API
/// </summary>
/// <typeparam name="TResult">Type of the Result property</typeparam>
public abstract class CloudflareResponse<TResult>
{
    public TResult? Result { get; set; }
    
    /// <summary>
    /// List of errors occurred.
    /// </summary>
    public List<CloudflareError>? Errors { get; set; }
    
    /// <summary>
    /// Tells if the request was successful or not.
    /// </summary>
    public bool Success { get; set; }
}