using Microsoft.Extensions.Configuration;
using WSSEGenerator;
using WSSEGenerator.Models;

var config = new ConfigurationManager();

config.Sources.Clear();

config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
Settings settings = config.Get<Settings>();



if (settings == null)
{
    throw new Exception("Could not bind configuration. Ensure appsettings.json is present and correct.");
}

await new WebsiteGenerator(settings).Generate();





