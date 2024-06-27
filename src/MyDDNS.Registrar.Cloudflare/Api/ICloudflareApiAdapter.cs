using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

public interface ICloudflareApiAdapter
{
    Task<GetDnsRecordsResponse?> GetDnsRecordsAsync(string zoneIdentifier, string recordName,
        CancellationToken cancellationToken = default);

    Task<PatchDnsRecordResponse?> PatchDnsRecordAsync(string zoneIdentifier, string recordId,
        PatchDnsRecordRequest payload, CancellationToken cancellationToken = default);
}