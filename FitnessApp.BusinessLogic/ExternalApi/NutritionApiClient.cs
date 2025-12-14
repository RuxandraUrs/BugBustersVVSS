using DotNetEnv;
using FitnessApp.BusinessLogic.Helpers;
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
            try
            {
                Env.Load();
                _apiKey = Environment.GetEnvironmentVariable("NINJA_API_KEY");

                if (string.IsNullOrEmpty(_apiKey))
                {
                    FileLogger.LogError("NINJA_API_KEY is missing from environment variables"); // ADD THIS
                    throw new Exception("API Key missing");
                }

                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

                FileLogger.LogError("NutritionApiClient initialized successfully"); // ADD THIS
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Error initializing NutritionApiClient", ex); // ADD THIS
                throw;
            }
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
                        FileLogger.LogError($"Food info retrieved successfully: {query}");
                        return foods[0];
                    }
                    else
                    {
                        FileLogger.LogError($"No food info found for query: {query}"); 
                    }
                }
                else
                {
                    FileLogger.LogError($"API Error: {response.StatusCode} for query: {query}"); 
                    Console.WriteLine($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Connection error for query: {query}", ex); 
                Console.WriteLine($"Connection error: {ex.Message}");
            }

            return null; 
        }
    }
}