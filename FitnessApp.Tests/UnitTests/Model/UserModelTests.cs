using NUnit.Framework;
using FitnessApp.Models;
using System;


namespace FitnessApp.Tests.UnitTests.Model
{
    [TestFixture]
    public class UserModelTests
    {
        #region Constructor Tests
        [Test]
        public void Constructor_Deafault_ShouldInitializeProperties()
        {
            var user = new User();

            Assert.IsNotNull(user);
            Assert.AreEqual(0, user.Id);
        }

        [Test]
        public void Constructor_WithParameters_ShouldSetProperties()
        {
            var user = new User("testUser", "test@email.com", 70, 175, 65, 2000);

            Assert.AreEqual("testUser", user.Username);
            Assert.AreEqual("test@email.com", user.Email);
            Assert.AreEqual(70, user.WeightKg);
            Assert.AreEqual(175, user.HeightCm);
            Assert.AreEqual(2000, user.DailyCalorieGoal);
        }

        [Test]
        public void Constructor_ShouldSetCreatedAt()
        {
            var user = new User("testuser", "test@email.com", 70, 175, 65, 2000);
            Assert.IsNotNull(user.CreatedAt);
            Assert.LessOrEqual((DateTime.Now - user.CreatedAt).TotalSeconds, 1);
        }
        #endregion

        #region SetUsername Tests
        [Test]
        public void SetUsername_ValidUsername_ShouldSetProperty()
        {
            var user = new User();

            user.SetUsername("validUser");
            Assert.AreEqual("validUser", user.Username);
        }

        [Test]
        public void SetUsername_Null_ShouldThrowArgumentException()
        {
            var user = new User();
            var ex = Assert.Throws<ArgumentException>(() => user.SetUsername(null));
            Assert.AreEqual("Username is mandatory!", ex.Message);
        }


        [Test]
        public void SetUsername_EmptyString_ShouldThrowArgumentException()
        {
            var user = new User();
            var ex = Assert.Throws<ArgumentException>(() => user.SetUsername(""));
            Assert.That(ex.Message, Does.Contain("Username is mandatory!"));
        }

        [Test]
        public void SetUsername_Whitespace_ShouldThrowArgumentException()
        {
            var user = new User();
            var ex = Assert.Throws<ArgumentException>(() => user.SetUsername("   "));
            Assert.That(ex.Message, Does.Contain("Username is mandatory!"));
        }

        [Test]
        public void SetUsername_LessThan4Characters_ShouldThrowArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.SetUsername("abc"));
            Assert.That(ex.Message, Does.Contain("at least 4 characters"));
        }

        [Test]
        public void SetUsername_Exactly4Characters_ShouldPass()
        {
            var user = new User();

            Assert.DoesNotThrow(() => user.SetUsername("abcd"));
            Assert.AreEqual("abcd", user.Username);
        }

        #endregion

        #region SetEmail Tests
        [Test]
        public void SetEmail_ValidEmail_ShouldSetProperty()
        {
            var user = new User();
            user.SetEmail("test@example.com");

            Assert.AreEqual("test@example.com", user.Email);
        }


        [Test]
        public void SetEmail_Null_ShouldThrowArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.SetEmail(null));
            Assert.That(ex.Message, Does.Contain("Email is mandatory"));
        }

        [Test]
        public void SetEmail_WithoutAtSymbol_ShouldThrowArgumentException()
        {
            var user = new User();
            var ex = Assert.Throws<ArgumentException>(() => user.SetEmail("invalidemail.com"));
            Assert.That(ex.Message, Does.Contain("Invalid email format"));
        }

        [Test]
        public void SetEmail_WithoutDot_ShouldThrowArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.SetEmail("invalid@emailcom"));
            Assert.That(ex.Message, Does.Contain("Invalid email format"));
        }

        #endregion

        #region SetWeight Tests
        [Test]
        public void SetWeight_ValidWeight_ShouldSetProperty()
        {
            var user = new User();

            user.SetWeight(70);

            Assert.AreEqual(70, user.WeightKg);
        }

        [Test]
        public void SetWeight_30kg_ShouldThrowArgumentException()
        {
            var user = new User();
            var ex = Assert.Throws<ArgumentException>(() => user.SetWeight(30));
            Assert.That(ex.Message, Does.Contain("between 30 and 300"));
        }

        [Test]
        public void SetWeight_31kg_ShouldPass()
        {
            var user = new User();

            Assert.DoesNotThrow(() => user.SetWeight(31));
        }

        [Test]
        public void SetWeight_300kg_ShouldThrowArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.SetWeight(301));
            Assert.That(ex.Message, Does.Contain("between 30 and 300"));
        }
        #endregion
    }
}