using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using WSSEGenerator;
using WSSEGenerator.Models;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
Settings settings = builder.Configuration.Get<Settings>();

using IHost host = builder.Build();

if (settings == null)
{
    throw new Exception("Could not bind configuration. Ensure appsettings.json is present and correct.");
}

new WebsiteGenerator(settings.GoogleApiKey).Generate();

await host.RunAsync();




