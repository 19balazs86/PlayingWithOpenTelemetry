using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name, CancellationToken ct)
    {
        Activity activity = Activity.Current!;

        //_logger.LogInformation("Activity.Id: {id}", activity.Id);

        string? myBaggage = activity.GetBaggageItem("MyBaggage");

        await Task.Delay(Random.Shared.Next(100, 250));

        HttpStatusCode selectedStatusCode = Dummy.GetRandomStatusCode();

        _logger.LogInformation("Say hello to: {name}. BaggageItem: '{myBaggage}'. StatusCode: {code}", name, myBaggage, selectedStatusCode);

        activity.SetTag("SelectedStatusCode", selectedStatusCode.ToString());

        HelloMetrics.IncrementCounter(name, (int)selectedStatusCode);

        // --> Return OK
        if (selectedStatusCode == HttpStatusCode.OK)
            return Ok($"Hello {name}!");

        // --> Delay
        if (selectedStatusCode == HttpStatusCode.RequestTimeout)
        {
            try
            {
                // If your method do not accept token in the argument, you can check it here beforehand.
                ct.ThrowIfCancellationRequested();

                await Task.Delay(5000, ct);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("The operation was canceled");

                //activity.RecordException(ex); // This will raise an event
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);

                return NoContent();
            }
        }

        // --> Throw Exception
        if (selectedStatusCode == HttpStatusCode.InternalServerError)
            throw new Exception("Throw exception for test purpose.");

        // --> Other returns
        return StatusCode((int)selectedStatusCode, $"Selected status code: {selectedStatusCode}");
    }
}
