using Microsoft.AspNetCore.Mvc;
using ErrorHandling;
namespace APIWithControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExceptionController : Controller
{

    private readonly ILogger<ExceptionController> _logger;

    public ExceptionController(ILogger<ExceptionController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "ErrorIndex")]
    public IActionResult Index()
    {
        return View();
    }

    // GET api/Exception/throw
    [HttpGet("throw")]
    public ActionResult<string> ThrowException()
    {
        _logger.LogInformation("Throwing an exception");
        throw new Exception("This is a test exception");
    }

    // GET api/Exception/crash
    [HttpGet("throwUnhandled")]
    public ActionResult<string> ThrowUnhandleException()
    {
        _logger.LogInformation("Throwing an unhandle exception");
        throw new SpecialUnhandleException("This is a unhandle exception");
    }


    //https://stackoverflow.com/questions/46304066/how-to-crash-an-asp-net-core-application-for-all-users
    // GET api/Exception/crash
    [HttpGet("crash")]
    public ActionResult<string> StackOverflow()
    {
        _logger.LogInformation("creating stackoverflow");
        // https://stackoverflow.com/questions/46304066/how-to-crash-an-asp-net-core-application-for-all-users
        var foo = new StackOverflower();
        System.Console.WriteLine(foo.MyText);
        return "StackOverflow";
    }
    public class StackOverflower
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private string m_MyText;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string MyText
        {
            get { return MyText; }
            set { this.m_MyText = value; }
        }
    }

    // GET api/Exception/exit
    [HttpGet("exit")]
    public ActionResult<string> Crash()
    {
        _logger.LogInformation("Crashing the application");
        try {
            Environment.Exit(1);
        } catch (Exception e) {
            _logger.LogError(e, "An error occurred");
        }
        return "Crashed the application";
    }
}
