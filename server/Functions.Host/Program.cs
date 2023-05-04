using Infrastructure.Logging;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .UseLogging()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();