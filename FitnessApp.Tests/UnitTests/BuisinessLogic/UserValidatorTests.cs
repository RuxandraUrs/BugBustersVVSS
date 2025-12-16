using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using Moq;

namespace FitnessApp.Tests.UnitTests.BuisinessLogic
{
    [TestFixture]
    public class UserValidatorTests
    {
        [Test]
        public void ValidateRegistration_ValidUser_ReturnsTrue()
        {
            var validator = new UserValidator();
            var user = new User
            {
                Username = "ValidUser",
                PasswordHash = "StrongPass123",
                BirthDate = new DateTime(2000, 1, 1),
                WeightKg = 70,
                HeightCm = 175
            };

            bool result = validator.ValidateRegistration(user, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void ValidateRegistration_NullUser_ReturnsFalse()
        {
            var validator = new UserValidator();

            bool result = validator.ValidateRegistration(null, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("User cannot be null.", errorMessage);
        }

        [Test]
        public void ValidateRegistration_ShortUsername_ReturnsFalse()
        {
            var validator = new UserValidator();
            var user = new User { Username = "abc", PasswordHash = "StrongPass123", BirthDate = new DateTime(2000, 1, 1), WeightKg = 70, HeightCm = 175 };

            bool result = validator.ValidateRegistration(user, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Username must be longer than 4 characters.", errorMessage);
        }

        [Test]
        public void ValidateRegistration_ShortPassword_ReturnsFalse()
        {
            var validator = new UserValidator();
            var user = new User { Username = "ValidUser", PasswordHash = "123", BirthDate = new DateTime(2000, 1, 1), WeightKg = 70, HeightCm = 175 };

            bool result = validator.ValidateRegistration(user, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Password is too short (minimum 6 characters).", errorMessage);
        }

        [Test]
        public void ValidateRegistration_UnderageUser_ReturnsFalse()
        {
            var validator = new UserValidator();
            var user = new User { Username = "YoungUser", PasswordHash = "StrongPass123", BirthDate = DateTime.Today.AddYears(-10), WeightKg = 40, HeightCm = 150 };

            bool result = validator.ValidateRegistration(user, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("You must be at least 12 years old to use the application.", errorMessage);
        }

        [Test]
        public void ValidateRegistration_NegativeWeightOrHeight_ReturnsFalse()
        {
            var validator = new UserValidator();
            var user = new User { Username = "ValidUser", PasswordHash = "StrongPass123", BirthDate = new DateTime(2000, 1, 1), WeightKg = -5, HeightCm = 175 };

            bool result = validator.ValidateRegistration(user, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Weight and height must be positive values.", errorMessage);
        }

        [Test]
        public void ValidateLogin_CorrectCredentials_ReturnsTrue()
        {
            var mockRepo = new Mock<IUserRepository>();
            var user = new User { Username = "TestUser", PasswordHash = "Password123" };
            mockRepo.Setup(r => r.GetUserByUsername("TestUser")).Returns(user);

            var validator = new UserValidator();
            bool result = validator.ValidateLogin(mockRepo.Object, "TestUser", "Password123", out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void ValidateLogin_UserNotFound_ReturnsFalse()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetUserByUsername("Unknown")).Returns((User)null);

            var validator = new UserValidator();
            bool result = validator.ValidateLogin(mockRepo.Object, "Unknown", "Password123", out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("User wasn't found.", errorMessage);
        }

        [Test]
        public void ValidateLogin_WrongPassword_ReturnsFalse()
        {
            var mockRepo = new Mock<IUserRepository>();
            var user = new User { Username = "TestUser", PasswordHash = "Password123" };
            mockRepo.Setup(r => r.GetUserByUsername("TestUser")).Returns(user);

            var validator = new UserValidator();
            bool result = validator.ValidateLogin(mockRepo.Object, "TestUser", "WrongPass", out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Password is incorrect.", errorMessage);
        }

    }
}
