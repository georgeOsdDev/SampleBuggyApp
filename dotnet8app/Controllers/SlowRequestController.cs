using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace APIWithControllers.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SlowRequestController : Controller
{

    private readonly ILogger<SlowRequestController> _logger;
    private readonly HttpClient httpClient;

    public SlowRequestController( IHttpClientFactory httpClientFactory,ILogger<SlowRequestController> logger)
    {
        httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    [HttpGet(Name = "SlowRequestIndex")]
    public IActionResult Index()
    {
        return View();
    }

    // GET api/SlowRequest/sleep/{seconds}
    [HttpGet("sleep/{seconds:int}")]
    public ActionResult<string> WaitSleep([FromRoute] int seconds)
    {
        _logger.LogInformation("Sleeping for {seconds} seconds", seconds);
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        System.Threading.Thread.Sleep(seconds * 1000);
        string endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        watch.Stop();
        return "Slept for " + seconds + " seconds. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }



    // GET api/SlowRequest/waitExternalDependency/{seconds}
    [HttpGet("waitExternalDependency/{seconds:int}")]
    public ActionResult<string> WaitExternalDependency([FromRoute] int seconds)
    {
        _logger.LogInformation("Call slow external api which take {seconds} seconds", seconds);
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        string endTime = ExecuteExternalCall(seconds).Result;
        watch.Stop();
        return "Waited for " + seconds + " seconds. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }


    async Task<string> ExecuteExternalCall(int seconds)
    {
        await httpClient.GetAsync("https://httpstat.us/200?sleep=" + seconds* 1000);
        string endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        return DateTime.Now.ToString("HH:mm:ss.fff");
    }
    // GET api/SlowRequest/waitExternalDependencyAsync/{seconds}
    [HttpGet("waitExternalDependencyAsync/{seconds:int}")]
    public async Task<ActionResult<string>> waitExternalDependencyAsync([FromRoute] int seconds)
    {
        _logger.LogInformation("Call slow external api which take {seconds} seconds in async", seconds);
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        string endTime = await ExecuteExternalCall(seconds);
        watch.Stop();
        return "Waited for " + seconds + " seconds. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }

    async Task<string> Wait(int seconds)
    {
        await Task.Delay(seconds* 1000);
        return DateTime.Now.ToString("HH:mm:ss.fff");
    }

    [HttpGet("wait/{seconds:int}")]
    public ActionResult<string> waitResult([FromRoute] int seconds)
    {
        _logger.LogInformation("Wait Task.Delay({seconds})", seconds);
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        string endTime = Wait(seconds).Result;
        watch.Stop();
        return "Waited for " + seconds + " seconds. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }
    [HttpGet("waitAsync/{seconds:int}")]
    public async Task<ActionResult<string>> waitAsyncResult([FromRoute] int seconds)
    {
        _logger.LogInformation("await Task.Delay({seconds}) in async", seconds);
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        string endTime = await Wait(seconds);
        watch.Stop();
        return "Waited for " + seconds + " seconds. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }
}
