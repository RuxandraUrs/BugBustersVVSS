using NUnit.Framework;
using FitnessApp.BusinessLogic.Services;

namespace FitnessApp.Tests.UnitTests
{
    [TestFixture]
    public class CalculatorTests
    {
        private FitnessCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new FitnessCalculator();
        }

        [Test]
        public void CalculateBMI_ValidInputs_ReturnsCorrectValue()
        {
            double weight = 80; // kg
            double height = 180; // cm

            double result = _calculator.CalculateBMI(weight, height);

            // 80 / (1.8 * 1.8) = 24.69
            Assert.AreEqual(24.69, result);
        }

        [Test]
        public void CalculateBMI_ZeroHeight_ReturnsZero()
        {
            double result = _calculator.CalculateBMI(70, 0);
            Assert.AreEqual(0, result);
        }
    }
}