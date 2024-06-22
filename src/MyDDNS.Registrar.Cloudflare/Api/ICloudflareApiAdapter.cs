using MyDDNS.Registrar.Cloudflare.Api.Models;
using MyDDNS.Registrar.Cloudflare.Api.Requests;
using MyDDNS.Registrar.Cloudflare.Api.Responses;

namespace MyDDNS.Registrar.Cloudflare.Api;

public interface ICloudflareApiAdapter
{
    Task<List<CloudflareDnsRecord>> GetDnsRecordsAsync(string zoneIdentifier, string recordName);

    Task<PatchDnsRecordResponse> PatchDnsRecord(string zoneIdentifier, string recordId, PatchDnsRecordRequest payload);
}