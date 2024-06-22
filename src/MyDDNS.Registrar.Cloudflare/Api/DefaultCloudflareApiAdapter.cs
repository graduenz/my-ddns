using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

public class DefaultCloudflareApiAdapter : ICloudflareApiAdapter
{
    private readonly string _zoneIdentifier;
    private readonly string _authEmail;
    private readonly string _authToken;

    public DefaultCloudflareApiAdapter(string zoneIdentifier, string authEmail, string authToken)
    {
        _zoneIdentifier = zoneIdentifier ?? throw new ArgumentNullException(nameof(zoneIdentifier));
        _authEmail = authEmail ?? throw new ArgumentNullException(nameof(authEmail));
        _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
    }

    public async Task<List<CloudflareDnsRecord>> GetDnsRecordsAsync(string zoneIdentifier, string recordName)
    {
        throw new NotImplementedException();
    }

    public async Task<PatchDnsRecordResponse> PatchDnsRecord(string zoneIdentifier, string recordId,
        PatchDnsRecordRequest payload)
    {
        throw new NotImplementedException();
    }
}