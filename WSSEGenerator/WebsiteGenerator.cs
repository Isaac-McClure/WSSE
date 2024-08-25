namespace WSSEGenerator
{
    internal class WebsiteGenerator
    {
        private string GoogleApiKey;
        public WebsiteGenerator(string googleApiKey) 
        {
            this.GoogleApiKey = googleApiKey;
        }

        public void Generate()
        {

            // See https://aka.ms/new-console-template for more information
            Console.WriteLine($"API Key: {this.GoogleApiKey}");

            // Overall Algorithm:

            // Get data from sheets
            // Get home page template
            // fill out with data and output
            // for each recipe
            // get template file
            // Fill out template using the data
            // output

           // IEnumerable<Recipe> GetRecipeDataFromSheets()
           // {
           //
           // }
        }
    }
}
