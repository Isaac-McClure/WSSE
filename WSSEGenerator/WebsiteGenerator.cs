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

        public void Generate()
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

            var recipes = GetRecipeDataFromSheets();

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

        IEnumerable<Recipe> ConvertValueRangeToRecipes(ValueRange valueRange)
        {
            var recipes = new List<Recipe>();
            var values = valueRange.Values;
            foreach(var recipeValues in values) {
                var recipe = new Recipe {
                    Name = recipeValues[0].ToString() ?? "",
                    LinkOrBook = recipeValues[1].ToString() ?? "",
                    Cuisine = recipeValues[2].ToString() ?? "",
                    Protein = recipeValues[3].ToString() ?? "",
                    CookingTimeInMinutes = recipeValues[4].ToString() ?? "",
                };

                recipes.Add(recipe);
            }
            return recipes;
        }
    }
}
