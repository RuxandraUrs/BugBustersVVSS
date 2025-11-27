using DotNetEnv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FitnessApp.BusinessLogic.ExternalApi
{
    public class NutritionApiClient
    {
        private string _apiKey;
        private const string BaseUrl = "https://api.api-ninjas.com/v1/nutrition?query=";
        private readonly HttpClient _httpClient;

        public NutritionApiClient()
        {
            Env.Load();

            _apiKey = Environment.GetEnvironmentVariable("NINJA_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("API Key missing");
            }
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        }

        // Fetch food information based on a query string
        public async Task<NinjaFoodItem> GetFoodInfoAsync(string query)
        {
            try
            {
                string url = BaseUrl + query;

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var foods = JsonConvert.DeserializeObject<List<NinjaFoodItem>>(jsonResponse);

                    if (foods != null && foods.Count > 0)
                    {
                        return foods[0];
                    }
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }

            return null; 
        }
    }
}