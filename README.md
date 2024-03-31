# Sample buggy app for .NET Core diagnostic scenario

Deployed at https://toshida-dotnet-buggyapp.azurewebsites.net/

# APIs

## [Slow Request](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/)
- Sleep `n` sec [/api/SlowRequest/sleep/{n}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/sleep/1)
- Wait external dependency `n` sec  [/api/SlowRequest/waitExternalDependency/{n}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/waitExternalDependency/1)
- Wait external dependency `n` sec in async [/api/SlowRequest/waitExternalDependencyAsync/{n}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/waitExternalDependencyAsync/1)
- Wait `n` sec [/api/SlowRequest/wait/{n}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/wait/1)
- Wait `n` sec in async [/api/SlowRequest/waitAsync/{n}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/waitAsync/1)

## [Exception](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Exceptions/)
- Throw exception [/api/Exception/throw}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Exception/throw)
- Throw unhandle exception [/api/Exception/throwUnhandled}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Exception/throw)
- Crash application [/api/Exception/crash}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Exception/crash)
- Exit application [/api/Exception/exit}](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Exception/exit)

## [Locked Request](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Lock)
- Do deadlock [/api/Lock/deadlock](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Lock/deadlock)
- Wait single semaphore [/api/Lock/semaphore](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Lock/semaphore)
- Wait unavilable semaphore [/api/Lock/semaphoreUnavailble](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Lock/semaphoreUnavailble)

## [Metric spikes](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Spike)
- CPU spike `n` sec [/api/Spike/memspike/{n}](/api/Spike/cpuspike/1)
- Memory spike `n` sec [/api/Spike/memspike/{n}](/api/Spike/memspike/1)
- Leak memory[/api/Spike/memleak](https://toshida-dotnet-buggyapp.azurewebsites.net/api/Spike/memleak)

# Build

`cd dotnet8app && dotnet publish -c Release -o ./bin/Publish`

# Diagnostic

Under writing...

# References

- https://learn.microsoft.com/en-us/troubleshoot/developer/webapps/aspnetcore/practice-troubleshoot-linux/lab-1-1-reproduce-troubleshoot
- https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak
