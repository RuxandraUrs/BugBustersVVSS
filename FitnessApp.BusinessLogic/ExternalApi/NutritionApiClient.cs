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

        public NutritionApiClient(HttpClient httpClient = null)
        {
            FileLogger.LogDebug("Initializing NutritionApiClient...");

            try
            {
                Env.Load();
                _apiKey = Environment.GetEnvironmentVariable("NINJA_API_KEY");

                if (string.IsNullOrEmpty(_apiKey) && httpClient != null)
                {
                    FileLogger.LogWarning("NINJA_API_KEY not found in environment, using TEST_API_KEY for testing");
                    _apiKey = "TEST_API_KEY";
                }

                if (string.IsNullOrEmpty(_apiKey))
                {
                    FileLogger.LogError("NINJA_API_KEY is missing from environment variables");
                    throw new Exception("API Key missing");
                }

                _httpClient = httpClient ?? new HttpClient();
                if (!_httpClient.DefaultRequestHeaders.Contains("X-Api-Key"))
                {
                    _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
                }

                FileLogger.LogInfo("✓ NutritionApiClient initialized successfully");
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Error initializing NutritionApiClient", ex);
                throw;
            }
        }

        // Fetch food information based on a query string
        public async Task<NinjaFoodItem> GetFoodInfoAsync(string query)
        {
            FileLogger.LogDebug($"NutritionApiClient: Fetching food info for query '{query}'");

            FileLogger.LogDebug($"NutritionApiClient: Fetching food info for query '{query}'");

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
                        FileLogger.LogInfo($" Food info retrieved successfully for '{query}': {foods[0].Name} ({foods[0].Calories} cal)");
                        return foods[0];
                    }
                    else
                    {
                        FileLogger.LogWarning($" No food info found for query '{query}'");
                        return null;
                    }
                }
                else
                {
                    FileLogger.LogWarning($"API returned error status {response.StatusCode} for query '{query}'");
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                FileLogger.LogError($"Network error while fetching food info for query '{query}'", httpEx);
                Console.WriteLine($"Connection error: {httpEx.Message}");
                return null;
            }
            catch (JsonException jsonEx)
            {
                FileLogger.LogError($"JSON parsing error for query '{query}'", jsonEx);
                Console.WriteLine($"JSON error: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Unexpected error fetching food info for query '{query}'", ex);
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }
    }
}