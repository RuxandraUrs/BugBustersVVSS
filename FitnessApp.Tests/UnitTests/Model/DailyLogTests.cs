using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.UnitTests.Model
{
    [TestFixture]
    public class DailyLogTests
    {
        #region Constructor Tests
        [Test]
        public void Constructor_Default_ShouldInitializeCollections()
        {
            var log = new DailyLog();

            Assert.IsNotNull(log.Meals);
            Assert.IsNotNull(log.Workouts);
            Assert.AreEqual(0, log.Meals.Count);
            Assert.AreEqual(0, log.Workouts.Count);
        }


        [Test]
        public void Constructor_WithParameters_ShouldSetProperties()
        {
            var userId = 1;
            var logDate = new DateTime(2024, 12, 10);

            var log = new DailyLog(userId, logDate);

            Assert.AreEqual(userId, log.UserId);
            Assert.AreEqual(logDate.Date, log.LogDate);
            Assert.IsNotNull(log.Meals);
            Assert.IsNotNull(log.Workouts);
        }

        [Test]
        public void Constructor_InvalidUserId_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new DailyLog(0, DateTime.Today));
            Assert.That(ex.Message, Does.Contain("User ID must be positive"));
        }

        [Test]
        public void Constructor_FutureDate_ShouldThrowArgumentException()
        {
            var futureDate = DateTime.Today.AddDays(1);
            var ex = Assert.Throws<ArgumentException>(() => new DailyLog(1, futureDate));
            Assert.That(ex.Message, Does.Contain("cannot be in the future"));
        }


        [Test]
        public void Constructor_TodayDate_ShouldSucceed()
        {
            Assert.DoesNotThrow(() => new DailyLog(1, DateTime.Today));
        }

        [Test]
        public void Constructor_PastDate_ShouldSucceed()
        {
            var pastDate = DateTime.Today.AddDays(-5);
            var log = new DailyLog(1, pastDate);
            Assert.AreEqual(pastDate.Date, log.LogDate);
        }
        #endregion

        #region AddMeal Tests
        [Test]
        public void AddMeal_ValidMeal_ShouldAddToCollection()
        {
            var log = new DailyLog(1, DateTime.Today);
            var meal = new Meal("Apple", 95, 150, MealType.Snack);

            log.AddMeal(meal);

            Assert.AreEqual(1, log.Meals.Count);
            Assert.AreEqual(meal, log.Meals[0]);
        }

        [Test]
        public void AddMeal_NullMeal_ShouldThrowArgumentNullException()
        {
            var log = new DailyLog(1, DateTime.Today);

            var ex = Assert.Throws<ArgumentNullException>(() => log.AddMeal(null));
            Assert.That(ex.ParamName, Is.EqualTo("meal"));
        }

        [Test]
        public void AddMeal_MultipleMeals_ShouldAddAllToCollection()
        {
            var log = new DailyLog(1, DateTime.Today);
            var meal1 = new Meal("Apple", 95, 150, MealType.Snack);
            var meal2 = new Meal("Banana", 105, 120, MealType.Snack);

            log.AddMeal(meal1);
            log.AddMeal(meal2);

            Assert.AreEqual(2, log.Meals.Count);
        }


        [Test]
        public void ddMeal_ShouldUpdateTotalCalories()
        {
            var log = new DailyLog(1, DateTime.Today);
            var meal = new Meal("Apple", 95, 150, MealType.Snack);

            log.AddMeal(meal);

            Assert.AreEqual(95, log.TotalCaloriesConsumed);
        }

        [Test]
        public void AddMeal_ShouldUpdateLastModifiedAt()
        {
            var log = new DailyLog(1, DateTime.Today);
            var meal = new Meal("Apple", 95, 150, MealType.Snack);

            log.AddMeal(meal);

            Assert.IsNotNull(log.LastModifiedAt);
            Assert.LessOrEqual((DateTime.Now - log.LastModifiedAt.Value).TotalSeconds, 1);
        }
        

        #region AddWorkout Tests
        [Test]
        public void AddWorkout_ValidWorkout_ShouldAddToCollection()
        {
            var log = new DailyLog(1, DateTime.Today);
            var workout = new Workout("Running", 30, 300);

            log.AddWorkout(workout);

            Assert.AreEqual(1, log.Workouts.Count);
            Assert.AreEqual(workout, log.Workouts[0]);
        }
        #endregion

        [Test]
        public void AddWorkout_NullWorkout_ShouldThrowArgumentNullException()
        {
            var log = new DailyLog(1, DateTime.Today);

            var ex = Assert.Throws<ArgumentNullException>(() => log.AddWorkout(null));
            Assert.That(ex.ParamName, Is.EqualTo("workout"));
        }

        [Test]
        public void AddWorkout_MultipleWorkouts_ShouldAddAllToCollection()
        {
            var log = new DailyLog(1, DateTime.Today);
            var workout1 = new Workout("Running", 30, 300);
            var workout2 = new Workout("Cycling", 45, 400);

            log.AddWorkout(workout1);
            log.AddWorkout(workout2);

            Assert.AreEqual(2, log.Workouts.Count);
        }

        [Test]
        public void AddWorkout_ShouldUpdateTotalCaloriesBurned()
        {
            var log = new DailyLog(1, DateTime.Today);
            var workout = new Workout("Running", 30, 300);

            log.AddWorkout(workout);

            Assert.AreEqual(300, log.TotalCaloriesBurned);
        }

        [Test]
        public void AddWorkout_ShouldUpdateLastModifiedAt()
        {
            var log = new DailyLog(1, DateTime.Today);
            var workout = new Workout("Running", 30, 300);

            log.AddWorkout(workout);

            Assert.IsNotNull(log.LastModifiedAt);
        }
        #endregion

        #region GetMealsByType Tests
        [Test]
        public void GetMealsByType_BreakfastOnly_ShouldReturnBreakfastMeals()
        {
            var log = new DailyLog(1, DateTime.Today);
            log.AddMeal(new Meal("Oatmeal", 150, 100, MealType.Breakfast));
            log.AddMeal(new Meal("Lunch", 500, 400, MealType.Lunch));
            log.AddMeal(new Meal("Eggs", 200, 150, MealType.Breakfast));

            var breakfastMeals = log.GetMealsByType(MealType.Breakfast);

            Assert.AreEqual(2, breakfastMeals.Count);
            Assert.IsTrue(breakfastMeals.All(m => m.Type == MealType.Breakfast));
        }

        [Test]
        public void GetMealsByType_NoMatchingType_ShouldReturnEmptyList()
        {
            var log = new DailyLog(1, DateTime.Today);
            log.AddMeal(new Meal("Lunch", 500, 400, MealType.Lunch));

            var breakfastMeals = log.GetMealsByType(MealType.Breakfast);

            Assert.AreEqual(0, breakfastMeals.Count);
        }

        #endregion

        #region IsComplete Tests
        [Test]
        public void IsComplete_WithMeals_ShouldReturnTrue()
        {
            var log = new DailyLog(1, DateTime.Today);
            log.AddMeal(new Meal("Apple", 95, 150, MealType.Snack));

            Assert.IsTrue(log.IsComplete());
        }

        [Test]
        public void IsComplete_NoMeals_ShouldReturnFalse()
        {
            var log = new DailyLog(1, DateTime.Today);

            Assert.IsFalse(log.IsComplete());
        }

        #endregion


        #region GetSummary Tests
        [Test]
        public void GetSummary_ShouldReturnFomattedString()
        {
            var log = new DailyLog(1, new DateTime(2024, 12, 10));
            log.AddMeal(new Meal("Lunch", 500, 400, MealType.Lunch));
            log.AddWorkout(new Workout("Running", 30, 300));

            var summary = log.GetSummary();

            Assert.That(summary, Does.Contain("2024-12-10"));
            Assert.That(summary, Does.Contain("Meals: 1"));
            Assert.That(summary, Does.Contain("Workouts: 1"));
            Assert.That(summary, Does.Contain("Consumed: 500"));
            Assert.That(summary, Does.Contain("Burned: 300"));
            Assert.That(summary, Does.Contain("Net: 200"));
        }

        [Test]
        public void ToString_ShouldReturnSummary()
        {
            var log = new DailyLog(1, DateTime.Today);

            var toString = log.ToString();
            var summary = log.GetSummary();

            Assert.AreEqual(summary, toString);
        }
        #endregion

    }
}
