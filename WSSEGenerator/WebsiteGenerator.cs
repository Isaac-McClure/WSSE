using WSSEGenerator.Models;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Sheets.v4;
using System.IO;
using System.Net;
using Google.Apis.Sheets.v4.Data;

namespace WSSEGenerator
{
    internal class WebsiteGenerator
    {
        private Settings settings;
        public WebsiteGenerator(Settings settings) 
        {
            this.settings = settings; 
        }

        public async Task Generate()
        {

            // See https://aka.ms/new-console-template for more information
            Console.WriteLine($"ID: {this.settings.GoogleClientId}");
            Console.WriteLine($"Secret: {this.settings.GoogleClientSecret}");

            // Overall Algorithm:

            // Get data from sheets
            // Get home page template
            // fill out with data and output
            // for each recipe
            // get template file
            // Fill out template using the data
            // output

            var recipes = await GetRecipeDataFromSheets();

            CreateHomePage(recipes);

        }

        private async Task<IEnumerable<Recipe>> GetRecipeDataFromSheets()
        {
            // Get value range from google spreadsheet
            // convert to Recipe list

            ValueRange response = await GetValueRangeFromSheets();

            var recipes = ConvertValueRangeToRecipes(response);
            return recipes;
        }

        private async Task<ValueRange> GetValueRangeFromSheets()
        {
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            new ClientSecrets
                            {
                                ClientId = this.settings.GoogleClientId,
                                ClientSecret = this.settings.GoogleClientSecret
                            },
                            new[] { SheetsService.Scope.SpreadsheetsReadonly },
                            "user",
                            CancellationToken.None,
                            new FileDataStore("Sheets.DataStore"));

            var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.settings.AppName
            });


            // Define request parameters.
            String spreadsheetId = this.settings.SpreadsheetId;
            String range = "Sheet1!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);

            try
            {
                ValueRange response = request.Execute();
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception thrown trying to execute request");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private IEnumerable<Recipe> ConvertValueRangeToRecipes(ValueRange valueRange)
        {
            var recipes = new List<Recipe>();
            var values = valueRange.Values;

            if (values == null) 
            {
                throw new Exception("No values found in spreadsheet.");
            }

            for (int i = 0; i < values.Count; i++)
            {
                var recipeValues = values[i];
                var recipe = new Recipe();

                if (recipeValues.Count > 0) { recipe.Name = recipeValues[0].ToString() ?? ""; }
                if (recipeValues.Count > 1) { recipe.LinkOrBook = recipeValues[1].ToString() ?? ""; }
                if (recipeValues.Count > 2) { recipe.Cuisine = recipeValues[2].ToString() ?? ""; }
                if (recipeValues.Count > 3) { recipe.Protein= recipeValues[3].ToString() ?? ""; }
                if (recipeValues.Count > 4) { recipe.CookingTimeInMinutes = recipeValues[4].ToString() ?? ""; }
                recipe.Link = $"recipe{i}.html";

                recipes.Add(recipe);
            }
            return recipes;
        }

        private void CreateHomePage(IEnumerable<Recipe> recipes) 
        {
            // Read home into memory
            // insert list of recipes into template
            // Write to output

            string homeTemplate = GetTemplateFile("home.html");

            string recipeListHtml = "";
            foreach (var recipe in recipes)
            {

                recipeListHtml += $"<li><a href=\"{recipe.Link}\">{recipe.Name}</a></li>\n";
            }

            var html = homeTemplate.Replace("{{ListGoesHere}}", recipeListHtml);

            OutputHtmlFile("home.html", html);

            Console.WriteLine(homeTemplate);
        }

        private string GetTemplateFile(string filename)
        {
            string template;
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), $"Templates/{filename}");
            try
            {
                StreamReader sr = new StreamReader(templatePath);
                template = sr.ReadToEnd();
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to open {filename} template file.");
                throw;
            }
            return template;
        }

        private void OutputHtmlFile(string filename, string html)
        {
            try
            {
                // Create output folder if it does not exist
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"Output/{filename}");
                System.IO.FileInfo file = new System.IO.FileInfo(outputPath);
                file.Directory.Create(); // If the directory already exists, this method does nothing.
                System.IO.File.WriteAllText(file.FullName, html);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to write {filename} html file.");
                throw;
            }
        }
    }
}
