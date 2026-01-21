using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace LimasIoTDevices.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureHttpResilienceHandler(this IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddResilienceHandler("default", pipeline =>
            {
                pipeline.AddTimeout(TimeSpan.FromSeconds(15));
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    Delay = TimeSpan.FromMilliseconds(500)
                });
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.9,
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(5)
                });
            });
        });
    }
}
