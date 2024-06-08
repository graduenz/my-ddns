namespace MyDDNS.Core.Dns;

public interface IDnsUpdater
{
    Task UpdateDnsAsync(IDnsConfiguration configuration);
}