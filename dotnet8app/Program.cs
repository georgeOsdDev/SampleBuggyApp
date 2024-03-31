using System.Net;
using ErrorHandling;
using Microsoft.AspNetCore.Diagnostics;

// https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.threadpool.setminthreads?view=net-8.0
// https://qiita.com/1stship/items/dd816cd56605c5ff7ee6
int minWorker, minIOC;
// Get the current settings.
ThreadPool.GetMinThreads(out minWorker, out minIOC);
// Change the minimum number of worker threads to four, but
// keep the old setting for minimum asynchronous I/O
// completion threads.
if (ThreadPool.SetMinThreads(100, minIOC))
{
    // The minimum number of threads was set successfully.
}
else
{
    // The minimum number of threads was not changed.
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(logging => {
});

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();
app.UseHttpLogging();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is SpecialUnhandleException)
        {
            throw new Exception("This is a unhandled exception");
        }

        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;;
        context.Response.ContentType = "text/html";

        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
        await context.Response.WriteAsync("ERROR!<br><br>\r\n");
        await context.Response.WriteAsync(
                                        "<a href=\"/\">Home</a><br>\r\n");
        await context.Response.WriteAsync("</body></html>\r\n");
        await context.Response.WriteAsync(new string(' ', 512));
    });
});
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();
