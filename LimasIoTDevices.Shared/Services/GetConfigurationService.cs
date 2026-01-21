using LimasIoTDevices.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace LimasIoTDevices.Shared.Services;

public interface IGetConfigurationService
{
    T Execute<T>(string key);
}

internal class GetConfigurationService : IGetConfigurationService
{
    private readonly IConfiguration _configuration;

    public GetConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T Execute<T>(string key)
    {
        T? value = _configuration.GetValue<T>(key);

        if (value is null || value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
        {
            throw new MessageErrorException($"Config \"{key}\" not defined.");
        }

        return value;
    }
}
