using System;
using DotNetEnv;

namespace FitnessApp.BusinessLogic.ExternalApi
{
    public class NutritionApiClient
    {
        private string _apiKey;
        private const string BaseUrl = "https://api.api-ninjas.com/v1/nutrition?query=";

        public NutritionApiClient()
        {
            Env.Load();

            _apiKey = Environment.GetEnvironmentVariable("NINJA_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("API Key missing");
            }
        }

        public void GetFoodCalories(string foodName)
        {
            Console.WriteLine($"Folosesc cheia: {_apiKey}");
            ///TO DO: Implementation to call the external API for fetching food calories
        }
    }
}