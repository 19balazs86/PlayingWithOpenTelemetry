using System.Diagnostics;

namespace ClientConsoleApp;

public interface IHelloClient
{
    Task SayHello(string helloTo);
}

public sealed class HelloClient : IHelloClient
{
    private readonly HttpClient _httpClient;

    public HelloClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SayHello(string helloTo)
    {
        using Activity activity = Program.ClientActivities.StartActivity("SayHello")!;

        activity.SetBaggage("MyBaggage", "baggage-value");

        HttpResponseMessage? response = null;

        try
        {
            response = await _httpClient.GetAsync($"Hello/{helloTo}");
        }
        catch (Exception ex)
        {
            // NotFound and BadRequest does not throw exeption

            //activity.RecordException(ex); // This will raise an event

            activity.SetStatus(ActivityStatusCode.Error, ex.Message);

            Console.WriteLine(ex.Message);

            return;
        }

        string responseString = await response.Content.ReadAsStringAsync();

        response.Dispose();

        var asc = response.IsSuccessStatusCode ? ActivityStatusCode.Ok : ActivityStatusCode.Error;

        activity.SetStatus(asc, responseString);

        Console.WriteLine(responseString);
    }
}
