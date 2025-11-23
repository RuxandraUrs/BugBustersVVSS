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

        [Test]
        public void User_SetUsername_ValidName_ShouldSetProperty()
        {
            var user = new User();
            string validName = "andreea_v";

            user.SetUsername(validName);

            Assert.AreEqual(validName, user.Username);
        }


        [Test]
        public void User_SetUsername_TooShort_ShouldThrowException()
        {
            var user = new User();
            string shortName = "ana";

            var ex = Assert.Throws<ArgumentException>(
                () => user.SetUsername(shortName)
                );
            Assert.That(ex.Message, Is.EqualTo("Username must be at least 4 characters long!"));
        }


        [Test]
        public void User_SetEmail_InvalidFormat_ShouldThrowException()
        {
            var user = new User();
            Assert.Throws<ArgumentException>(() => user.SetEmail("invalid_email.com"));
        }

        [Test]
        public void User_SetWeight_Negative_ShouldThrowException()
        {
            var user = new User();
            Assert.Throws<ArgumentException>(() => user.SetWeight(-5));
        }

        [Test]
        public void Meal_Constructor_NegativeCalories_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Meal("Pizza", -200, 100, MealType.Lunch));
        }


        [Test]
        public void Workout_Constructor_ValidData_ShouldCreateObject()
        {
            var workout = new Workout("Running", 60, 500);

            Assert.AreEqual("Running", workout.ActivityName);
            Assert.AreEqual(60, workout.DurationMinutes);
        }

        [Test]
        public void Workout_Constructor_NegativeDuration_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Workout("Running", -10, 100));
            Assert.That(ex.Message, Does.Contain("Training duration must be positive."));
        }

        [Test]
        public void Workout_Constructor_ZeroDuration_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Workout("Walking", 0, 50));
        }

        [Test]
        public void Workout_Constructor_EmptyName_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Workout("", 30, 100));
        }

        [Test]
        public void Workout_Constructor_TooLongDuration_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Workout("Marathon", 500, 4000));
        }

    }
}
