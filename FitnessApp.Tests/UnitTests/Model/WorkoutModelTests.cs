using NUnit.Framework;
using FitnessApp.Models;
using System;
namespace FitnessApp.Tests.UnitTests.Model
{
    [TestFixture]
    public class WorkoutModelTests
    {
        [Test]
        public void Constructor_ValidParameters_ShouldCreateWorkout()
        {
            var workout = new Workout("Running", 30, 300);

            Assert.AreEqual("Running", workout.ActivityName);
            Assert.AreEqual(30, workout.DurationMinutes);
            Assert.AreEqual(300, workout.CaloriesBurned);
        }

        [Test]
        public void Constructor_NullActivityName_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout(null, 30, 300));
            Assert.That(ex.Message, Does.Contain("Activity name is required"));
        }

        [Test]
        public void Constructor_EmptyActivityName_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("", 30, 300));
        }

        [Test]
        public void Constructor_WhitespaceActivityName_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("   ", 30, 300));
        }

        [Test]
        public void Constructor_ZeroDuration_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("Running", 0, 300));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        public void Constructor_NegativeDuration_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("Running", -10, 300));
        }

        [Test]
        public void Constructor_DurationOver8Hours_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("Marathon", 500, 4000));
            Assert.That(ex.Message, Does.Contain("suspiciously high"));
        }

        [Test]
        public void Constructor_Exactly480Minutes_ShouldPass()
        {
            Assert.DoesNotThrow(() => new Workout("Ultra Marathon", 480, 4000));
        }

        [Test]
        public void Constructor_NegativeCalories_ShouldThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Workout("Running", 30, -100));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        public void Constructor_ZeroCalories_ShouldPass()
        {
            Assert.DoesNotThrow(() => new Workout("Stretching", 10, 0));
        }

        [Test]
        public void Constructor_ShortDuration_ShouldPass()
        {
            var workout = new Workout("Quick Walk", 5, 25);
            Assert.AreEqual(5, workout.DurationMinutes);
        }

        [Test]
        public void Constructor_LongActivityName_ShouldPass()
        {
            var longName = "High Intensity Interval Training with Weights and Cardio";
            Assert.DoesNotThrow(() => new Workout(longName, 45, 500));
        }

    }
}
