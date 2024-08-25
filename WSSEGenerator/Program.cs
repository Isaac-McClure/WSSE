using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

// See https://aka.ms/new-console-template for more information
Console.WriteLine($"API Key: {settings.GoogleApiKey}");

await host.RunAsync();

