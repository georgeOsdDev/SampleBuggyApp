using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace APIWithControllers.Controllers;

// This code is based on the following sample:
// https://github.com/dotnet/samples/blob/main/core/diagnostics/DiagnosticScenarios/Controllers/DiagnosticScenarios.cs

[ApiController]
[Route("api/[controller]")]
public class SpikeController : Controller
{

    private readonly ILogger<SpikeController> _logger;
    private static Processor p = new Processor();

    public SpikeController(ILogger<SpikeController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "SpikeIndex")]
    public IActionResult Index()
    {
        return View();
    }

    // GET api/spike/cpu
    [HttpGet("cpuspike/{seconds:int}")]
    public ActionResult<string> Deadlock([FromRoute] int seconds)
    {
        var watch = new Stopwatch();
        watch.Start();

        while (true)
        {
            watch.Stop();
            if (watch.ElapsedMilliseconds > seconds * 1000)
                break;
            watch.Start();
        }

        return "success:highcpu";
    }

    [HttpGet]
    [Route("memspike/{seconds}")]
    public ActionResult<string> memspike(int seconds)
    {
        var watch = new Stopwatch();
        watch.Start();

        while (true)
        {
            p = new Processor();
            watch.Stop();
            if (watch.ElapsedMilliseconds > seconds * 1000)
                break;
            watch.Start();

            int it = (2000 * 1000);
            for (int i = 0; i < it; i++)
            {
                p.ProcessTransaction(new Item(Guid.NewGuid().ToString()));
            }

            Thread.Sleep(5000); // Sleep for 5 seconds before cleaning up

            // Cleanup
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            p = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // GC
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Thread.Sleep(5000); // Sleep for 5 seconds before spiking memory again
        }
        return "success:memspike";
    }

    [HttpGet]
    [Route("memleak")]
    public ActionResult<string> memleak()
    {
        _logger.LogInformation("Leak memory, current state");
        // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.privatememorysize64?view=net-8.0
        Process currentProcess = Process.GetCurrentProcess();
        string beforeInfo = $@"
        =====================================================================
        Before processing
        Time:                        {DateTime.Now.ToString("HH:mm:ss.fff")}
        Physical memory usage      : {currentProcess.WorkingSet64}
        Base priority              : {currentProcess.BasePriority}
        Priority class             : {currentProcess.PriorityClass}
        User processor time        : {currentProcess.UserProcessorTime}
        Privileged processor time  : {currentProcess.PrivilegedProcessorTime}
        Total processor time       : {currentProcess.TotalProcessorTime}
        Paged system memory size   : {currentProcess.PagedSystemMemorySize64}
        Paged memory size          : {currentProcess.PagedMemorySize64}
        Peak physical memory usage : {currentProcess.PeakPagedMemorySize64}
        Peak paged memory usage    : {currentProcess.PeakVirtualMemorySize64}
        Peak virtual memory usage  : {currentProcess.PeakWorkingSet64}
        =====================================================================
        ";
        _logger.LogInformation(beforeInfo);

        int it = (500 * 1024 * 1024) / 100;
        for (int i = 0; i < it; i++)
        {
            p.ProcessTransaction(new Item(Guid.NewGuid().ToString()));
        }

        Process afterProcess = Process.GetCurrentProcess();
        string afterInfo = $@"
        =====================================================================
        After processing
        Time:                        {DateTime.Now.ToString("HH:mm:ss.fff")}
        Physical memory usage      : {afterProcess.WorkingSet64}
        Base priority              : {afterProcess.BasePriority}
        Priority class             : {afterProcess.PriorityClass}
        User processor time        : {afterProcess.UserProcessorTime}
        Privileged processor time  : {afterProcess.PrivilegedProcessorTime}
        Total processor time       : {afterProcess.TotalProcessorTime}
        Paged system memory size   : {afterProcess.PagedSystemMemorySize64}
        Paged memory size          : {afterProcess.PagedMemorySize64}
        Peak physical memory usage : {afterProcess.PeakPagedMemorySize64}
        Peak paged memory usage    : {afterProcess.PeakVirtualMemorySize64}
        Peak virtual memory usage  : {afterProcess.PeakWorkingSet64}
        =====================================================================
        ";

        return beforeInfo + "\n" + afterInfo;
    }
}

class Item
{
    private string id;

    public Item(string id)
    {
        this.id = id;
    }
}

class ItemMemoryCache
{
    private List<Item> cache = new List<Item>();

    public void AddItem(Item c)
    {
        cache.Add(c);
    }
}

class Processor
{
    private ItemMemoryCache cache = new ItemMemoryCache();

    public void ProcessTransaction(Item item)
    {
        cache.AddItem(item);
    }
}
