using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace APIWithControllers.Controllers;

// This code is based on the following sample:
// https://github.com/dotnet/samples/blob/main/core/diagnostics/DiagnosticScenarios/Controllers/DiagnosticScenarios.cs

[ApiController]
[Route("api/[controller]")]
public class LockController : Controller
{

    private readonly ILogger<LockController> _logger;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static SemaphoreSlim semaphore;
    private static SemaphoreSlim semaphore2;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public LockController(ILogger<LockController> logger)
    {
        _logger = logger;
        semaphore = new SemaphoreSlim(1, 1);
        semaphore2 = new SemaphoreSlim(0, 1);
    }

    [HttpGet(Name = "LockIndex")]
    public IActionResult Index()
    {
        return View();
    }

    // GET api/lock/deadlock
    [HttpGet("deadlock")]
    public ActionResult<string> Deadlock()
    {
        object lock1 = new object();
        object lock2 = new object();
        var task1 = Task.Run(() =>
        {
            lock (lock1)
            {
                Thread.Sleep(1000);
                lock (lock2)
                {
                    //
                }
            }
        });

        var task2 = Task.Run(() =>
        {
            lock (lock2)
            {
                Thread.Sleep(1000);
                lock (lock1)
                {
                    //
                }
            }
        });

        Task.WaitAll(task1, task2);
        return "deadlock";
    }

    // GET api/lock/semaphore
    [HttpGet("semaphore")]
    public ActionResult<string> Semaphore()
    {
        _logger.LogInformation("Wait single semaphore");
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        semaphore.Wait();
        Thread.Sleep(10000);
        semaphore.Release();
        string endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        watch.Stop();
        return "Waited single semaphore. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }

        // GET api/lock/semaphoreUnavailble
    [HttpGet("semaphoreUnavailble")]
    public ActionResult<string> SemaphoreUnavailable()
    {
        _logger.LogInformation("Wait unavailable semaphore forever");
        var watch = new Stopwatch();
        watch.Start();
        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        semaphore2.Wait();
        Thread.Sleep(10000);
        semaphore2.Release();
        string endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        watch.Stop();
        return "Waited single semaphore. Started at " + startTime + " and ended at " + endTime + " and took " + watch.ElapsedMilliseconds + " ms";
    }

}
