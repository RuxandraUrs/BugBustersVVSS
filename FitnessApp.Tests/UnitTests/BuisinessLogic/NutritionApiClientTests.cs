using NUnit.Framework;
using Moq;
using Moq.Protected; 
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.ExternalApi;
using System;

namespace FitnessApp.Tests.UnitTests.BusinessLogic
{
    [TestFixture]
    public class NutritionApiClientTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private NutritionApiClient _apiClient;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://api.api-ninjas.com");

            _apiClient = new NutritionApiClient(httpClient);
        }

        [Test]
        public async Task GetFoodInfoAsync_ValidResponse_ReturnsCorrectData()
        {
            string jsonResponse = "[{ 'name': 'apple', 'calories': 52.0, 'serving_size_g': 100.0, 'protein_g': 0.3, 'fat_total_g': 0.2, 'carbohydrates_total_g': 13.8 }]";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var result = await _apiClient.GetFoodInfoAsync("apple");

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("apple"));
            Assert.That(result.Calories, Is.EqualTo(52.0));
        }

        [Test]
        public async Task GetFoodInfoAsync_ApiError404_ReturnsNull()
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _apiClient.GetFoodInfoAsync("unknown_food");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetFoodInfoAsync_NetworkException_ReturnsNull()
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("No internet connection"));

            var result = await _apiClient.GetFoodInfoAsync("apple");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetFoodInfoAsync_EmptyListReturned_ReturnsNull()
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var result = await _apiClient.GetFoodInfoAsync("stone");

            Assert.IsNull(result);
        }
    }
}