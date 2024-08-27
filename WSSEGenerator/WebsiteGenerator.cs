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
        private IEnumerable<Recipe> recipes;
        private string proteinOptions;
        private string cuisineOptions;

        public WebsiteGenerator(Settings settings) 
        {
            this.settings = settings; 
        }

        public async Task Generate()
        {
            // Overall Algorithm:

            // Get data from sheets
            // Get home page template
            // fill out with data and output
            // for each recipe
            // get template file
            // Fill out template using the data
            // output

            // For more fun, add home page with random buttons, and add filters to both random and list page.

            await GetRecipeDataFromSheets();
            createFilterOptionLists();

            CreateHomePage();
            CreateDetailPages();
            CreateIndexPage();
            CreateScriptFile();
            CreateStyleFile();
        }

        private async Task<IEnumerable<Recipe>> GetRecipeDataFromSheets()
        {
            // Get value range from google spreadsheet
            // convert to Recipe list

            ValueRange response = await GetValueRangeFromSheets();

            var recipes = ConvertValueRangeToRecipes(response);
            this.recipes = recipes;
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

        private void CreateHomePage()
        {
            string fileName = "home.html";
            string homeTemplate = GetTemplateFile(fileName);

            string recipeListHtml = "";
            foreach (var recipe in this.recipes)
            {
                recipeListHtml += $"<li data-protein=\"{recipe.Protein}\"><a href=\"{recipe.Link}\">{recipe.Name}</a></li>\n";
            }

            var html = homeTemplate.Replace("{{ListGoesHere}}", recipeListHtml);
            html = html.Replace("{{ProteinOptionListGoesHere}}", this.proteinOptions);

            OutputFile(fileName, html);
        }

        private void CreateIndexPage()
        {
            string fileName = "index.html";
            string homeTemplate = GetTemplateFile(fileName);

            var html = homeTemplate.Replace("{{CuisineOptionList}}", this.cuisineOptions);
            html = html.Replace("{{ProteinOptionList}}", this.proteinOptions);

            OutputFile(fileName, html);
        }

        private void createFilterOptionLists()
        {
            this.proteinOptions = GetProteinOptionHtmlFromRecipes();
            this.cuisineOptions = GetCuisineOptionHtmlFromRecipes();
        }

        private string GetProteinOptionHtmlFromRecipes()
        {
            string filterOptionList = "";
            List<string> protienNames = new List<string> { "Choose a protein" };
            protienNames.AddRange(this.recipes.Select(x => x.Protein).Where(y => !string.IsNullOrEmpty(y)).Distinct().ToList());
            foreach (var proteinOption in protienNames)
            {
                filterOptionList += $"<option value=\"{proteinOption}\">{proteinOption}</option>";
            }
            return filterOptionList;
        }

        private string GetCuisineOptionHtmlFromRecipes()
        {
            string filterOptionList = "";
            List<string> cuisineNames =
            [
                "Choose a cuisine",
                .. this.recipes.Select(x => x.Cuisine).Where(y => !string.IsNullOrEmpty(y)).Distinct().ToList(),
            ];
            foreach (var cuisineName in cuisineNames)
            {
                filterOptionList += $"<option value=\"{cuisineName}\">{cuisineName}</option>";
            }
            return filterOptionList;
        }

        private void CreateDetailPages()
        {
            string detailTemplate = GetTemplateFile("recipe-detail.html");

            foreach (var recipe in this.recipes)
            {
                var html = detailTemplate.Replace("{{Name}}", recipe.Name);
                html = html.Replace("{{Cuisine}}", recipe.Cuisine);
                html = html.Replace("{{Protein}}", recipe.Protein);
                html = html.Replace("{{CookingTimeInMinutes}}", recipe.CookingTimeInMinutes);

                if (recipe.IsOnlineRecipe)
                {
                    html = html.Replace("{{LinkOrBook}}", $"<div>Link: <a href=\"{recipe.LinkOrBook}\">{recipe.LinkOrBook}</a></div>");
                }
                else 
                {
                    html = html.Replace("{{LinkOrBook}}", $"<div>Cookbook: {recipe.LinkOrBook}</div>");
                }

                OutputFile(recipe.Link, html);
            }
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

        private void OutputFile(string filename, string contents)
        {
            try
            {
                // Create output folder if it does not exist
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"Output/{filename}");
                FileInfo file = new FileInfo(outputPath);
                file.Directory.Create();
                File.WriteAllText(file.FullName, contents);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to write {filename} output file.");
                throw;
            }
        }

        private void CreateScriptFile() 
        {
            string fileName = "script.js";
            var templateScript = GetTemplateFile(fileName);

            var recipesAsJavascriptList = recipes.Select(x => $"{{ Name: \"{x.Name}\",\r\nLinkOrBook: \"{x.LinkOrBook}\", \r\nCuisine: \"{x.Cuisine}\", \r\nProtein: \"{x.Protein}\", \r\nCookingTimeInMinutes: {(string.IsNullOrEmpty(x.CookingTimeInMinutes) ? 0 : x.CookingTimeInMinutes)}, \r\nLink: \"{x.Link}\", \r\nIsOnlineRecipe: {(x.IsOnlineRecipe ? "true" : "false")}}}");
            var recipesAsJavascript  = string.Join(",\n", recipesAsJavascriptList);

            var script = templateScript.Replace("RECIPES = []", $"RECIPES = [{recipesAsJavascript}]");

            OutputFile(fileName, script);
        }

        private void CreateStyleFile()
        {
            var fromPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates/style.css");
            var toPath = Path.Combine(Directory.GetCurrentDirectory(), "Output/style.css");
            FileInfo file = new FileInfo(fromPath);
            file.CopyTo(toPath, true);
        }
    }
}
