using NUnit.Framework;
using Moq;
using FitnessApp.BusinessLogic.Services;
using FitnessApp.BusinessLogic.Domains;
using System;

namespace FitnessApp.Tests.UnitTests.BusinessLogic
{
    [TestFixture]
    public class CalculatorTests
    {
        private Mock<IUserDomain> _mockUserDomain;
        private FitnessCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _mockUserDomain = new Mock<IUserDomain>();
            _calculator = new FitnessCalculator(_mockUserDomain.Object);
        }

        //verifies that BMI calculation correctly delegates to the domain layer.
        [Test]
        public void CalculateBMI_ValidUsername_CallsDomain()
        {
            string username = "TestUser";
            _mockUserDomain.Setup(d => d.CalculateBMI(username)).Returns(24.5);

            double result = _calculator.CalculateBMI(username);

            Assert.That(result, Is.EqualTo(24.5));
            _mockUserDomain.Verify(d => d.CalculateBMI(username), Times.Once);
        }

        //ensures that passing a null username throws an ArgumentNullException.
        [Test]
        public void CalculateBMI_NullUsername_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateBMI(null));
        }

        //ensures that passing an empty username throws an ArgumentException.
        [Test]
        public void CalculateBMI_EmptyUsername_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _calculator.CalculateBMI(""));
        }

        //verifies that BMR calculation correctly delegates to the domain layer.
        [Test]
        public void CalculateBMR_ValidUsername_CallsDomain()
        {
            string username = "TestUser";
            _mockUserDomain.Setup(d => d.CalculateBMR(username)).Returns(1500);

            double result = _calculator.CalculateBMR(username);

            Assert.That(result, Is.EqualTo(1500));
            _mockUserDomain.Verify(d => d.CalculateBMR(username), Times.Once);
        }

        //verifies that TDEE is calculated correctly based on different activity levels.
        [TestCase("sedentary", 1.2)]
        [TestCase("light", 1.375)]
        [TestCase("moderate", 1.55)]
        [TestCase("active", 1.725)]
        [TestCase("veryactive", 1.9)]
        public void CalculateTDEE_ActivityLevels_ReturnsCorrectValue(string activity, double multiplier)
        {
            string username = "TestUser";
            double bmr = 2000;
            _mockUserDomain.Setup(d => d.CalculateBMR(username)).Returns(bmr);

            double result = _calculator.CalculateTDEE(username, activity);

            Assert.That(result, Is.EqualTo(bmr * multiplier));
        }

        //checks if an invalid activity level defaults to the sedentary multiplier.
        [Test]
        public void CalculateTDEE_InvalidActivity_ReturnsDefaultMultiplier()
        {
            _mockUserDomain.Setup(d => d.CalculateBMR("User")).Returns(1000);

            double result = _calculator.CalculateTDEE("User", "unknown");

            Assert.That(result, Is.EqualTo(1200));
        }

        //ensures that TDEE returns zero when the underlying BMR is zero.
        [Test]
        public void CalculateTDEE_ZeroBMR_ReturnsZero()
        {
            _mockUserDomain.Setup(d => d.CalculateBMR("User")).Returns(0);

            double result = _calculator.CalculateTDEE("User", "sedentary");

            Assert.That(result, Is.EqualTo(0));
        }

        //verifies that the correct BMI category is returned for various boundary values.
        [TestCase(18.4, "Underweight")]
        [TestCase(18.5, "Normal weight")]
        [TestCase(24.9, "Normal weight")]
        [TestCase(25.0, "Overweight")]
        [TestCase(29.9, "Overweight")]
        [TestCase(30.0, "Obese")]
        public void GetBMICategory_BoundaryValues_ReturnsCorrectString(double bmi, string expected)
        {
            string result = _calculator.GetBMICategory(bmi);
            Assert.That(result, Is.EqualTo(expected));
        }

        //checks if a negative BMI input returns an 'Invalid BMI' message.
        [Test]
        public void GetBMICategory_NegativeValue_ReturnsInvalid()
        {
            string result = _calculator.GetBMICategory(-1);
            Assert.That(result, Is.EqualTo("Invalid BMI"));
        }

        //verifies correct calorie adjustments for different weight goals (lose, gain, maintain).
        [TestCase("lose", -500)]
        [TestCase("gain", 500)]
        [TestCase("maintain", 0)]
        public void CalculateCaloriesForGoal_Goals_ReturnsCorrectAdjustment(string goal, int adjustment)
        {
            string username = "User";
            _mockUserDomain.Setup(d => d.CalculateBMR(username)).Returns(2000);

            int result = _calculator.CalculateCaloriesForGoal(username, goal, "sedentary");

            Assert.That(result, Is.EqualTo(2400 + adjustment));
        }

        //ensures that calorie calculation returns zero when the TDEE is zero.
        [Test]
        public void CalculateCaloriesForGoal_ZeroTDEE_ReturnsZero()
        {
            _mockUserDomain.Setup(d => d.CalculateBMR("User")).Returns(0);

            int result = _calculator.CalculateCaloriesForGoal("User", "lose", "sedentary");

            Assert.That(result, Is.EqualTo(0));
        }
    }
}