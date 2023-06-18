using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace ClientConsoleApp;

public static class HelloClientExtensions
{
    private static readonly Uri _webApiUri = new Uri("http://localhost:5000");

    public static IServiceCollection AddHelloClient(this IServiceCollection services)
    {
        // --> Create: Polly policy.
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
            .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(500));

        AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(500));

        services.AddHttpClient<IHelloClient, HelloClient>(httpClient => httpClient.BaseAddress = _webApiUri)
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy);

        return services;
    }
}
