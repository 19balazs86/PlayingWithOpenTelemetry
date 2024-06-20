using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace ClientConsoleApp;

public static class HelloClientExtensions
{
    private static readonly Uri _webApiUri = new Uri("http://localhost:5000");

    public static IServiceCollection AddHelloClient(this IServiceCollection services)
    {
        services
            .AddHttpClient<IHelloClient, HelloClient>(httpClient => httpClient.BaseAddress = _webApiUri)
            .AddResilienceHandler("user-pipeline", configureResilienceHandler);

        return services;
    }

    private static void configureResilienceHandler(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder)
    {
        // --> Define option: Retry
        var retryOptions = new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 2,
            Delay            = TimeSpan.FromMilliseconds(500),
            BackoffType      = DelayBackoffType.Constant,
        };

        // --> Configure: Pipeline
        pipelineBuilder
            .AddTimeout(TimeSpan.FromSeconds(5)) // Total timeout for the request execution
            .AddRetry(retryOptions)
            .AddTimeout(TimeSpan.FromMilliseconds(500)); // Timeout per each request attempt
    }
}
