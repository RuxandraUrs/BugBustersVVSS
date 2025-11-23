using NUnit.Framework;
using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System.IO;
using FitnessApp.BusinessLoic.Helpers;
namespace FitnessApp.Tests.UnitTests
{
    [TestFixture]
    public class ModelsTests
    {
        [Test]
        public void Meal_Constructor_ShouldSetPropertiesCorrectly()
        {
            string food = "Apple";
            double cal = 95;
            double qty = 150;
            MealType type = MealType.Snack;

            Meal meal = new Meal(food, cal, qty, type);

           
            Assert.AreEqual(food, meal.FoodName);
            Assert.AreEqual(cal, meal.Calories);
            Assert.AreEqual(type, meal.Type);
        }

        [Test]
        public void Logger_LogError_ShouldCreateFile()
        {
            string testFile = "application_log.txt";
            if (File.Exists(testFile)) File.Delete(testFile);

            FileLogger.LogError("Test Error Message");
            Assert.IsTrue(File.Exists(testFile));
        }

        [Test]
        public void Workout_SetDuration_ShouldStoreValue()
        {
            Workout w = new Workout("Running", 30, 300);
            Assert.AreEqual(30, w.DurationMinutes);
        }

    }
}
