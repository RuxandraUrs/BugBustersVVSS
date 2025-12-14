using NUnit.Framework;
using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System;

namespace FitnessApp.Tests.UnitTests.Model
{
    [TestFixture]
    public class MealModelTests
    {
        [Test]
        public void Constructor_ValidParameters_ShouldCreateMeal()
        {
            var meal = new Meal("Apple", 95, 150, MealType.Snack);
            Assert.AreEqual("Apple", meal.FoodName);
            Assert.AreEqual(95, meal.Calories);
            Assert.AreEqual(150, meal.QuantityGrams);
            Assert.AreEqual(MealType.Snack, meal.Type);

        }

        [Test]
        public void Constructor_NullFoodName_ShouldThrowArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                 new Meal(null, 95, 150, MealType.Snack));
            Assert.That(ex.Message, Does.Contain("Food Name is mandatory"));
        }

        [Test]
        public void Constructor_EmptyFoodName_ShouldThrowArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new Meal("", 95, 150, MealType.Snack));
        }

        [Test]
        public void Constructor_WhitespaceFoodName_ShouldThrowArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new Meal("   ", 95, 150, MealType.Snack));
        }

        [Test]
        public void Constructor_NegativeCalories_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            new Meal("Apple", -50, 150, MealType.Snack));
            Assert.That(ex.Message, Does.Contain("Calories cannot be negative"));
        }

        [Test]
        public void Constructor_ZeroCalories_ShouldPass()
        {
            Assert.DoesNotThrow(() => new Meal("Water", 0, 250, MealType.Snack));
        }

        [Test]
        public void Constructor_ZeroQuantity_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Meal("Apple", 95, 0, MealType.Snack));
            Assert.That(ex.Message, Does.Contain("Quantity must be positive"));
        }

        [Test]
        public void Constructor_NegativeQuantity_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Meal("Apple", 95, -100, MealType.Snack));
        }

        [Test]
        public void Constructor_BreakfastType_ShouldSetCorrectly()
        {
            var meal = new Meal("Oatmeal", 150, 100, MealType.Breakfast);
            Assert.AreEqual(MealType.Breakfast, meal.Type);
        }

        [Test]
        public void Constructor_LunchType_ShouldSetCorrectly()
        {
            var meal = new Meal("Salad", 300, 250, MealType.Lunch);
            Assert.AreEqual(MealType.Lunch, meal.Type);
        }

        [Test]
        public void Constructor_DinnerType_ShouldSetCorrectly()
        {
            var meal = new Meal("Pasta", 500, 400, MealType.Dinner);
            Assert.AreEqual(MealType.Dinner, meal.Type);
        }

        [Test]
        public void Constructor_VeryHighCalories_ShouldPass()
        {
            Assert.DoesNotThrow(() => new Meal("Pizza", 2000, 500, MealType.Dinner));
        }

    }
}
