using FitnessApp.BusinessLogic.Domains;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.UnitTests.BuisinessLogic
{
    [TestFixture]
    public class UserDomainTests
    {
        private Mock<IUserRepository> _mockRepo;
        private UserDomain _userDomain;

        private static User CreateValidUser()
        {
            return new User
            {
                Username = "TestUser",
                PasswordHash = "password123",
                Email = "validtest@gmail.com",
                WeightKg = 70,
                HeightCm = 175,
                BirthDate = DateTime.Now.AddYears(-20),
                TargetWeightKg = 68,
                DailyCalorieGoal = 2000,
                Gender = "Male"
            };
        }

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userDomain = new UserDomain(_mockRepo.Object);
        }

        #region Register User
        [Test]
        public void RegisterUser_ValidUser_CallsAddUser()
        {
            var user = CreateValidUser();
            _mockRepo.Setup(r => r.GetUserByUsername(user.Username)).Returns((User)null);

            _userDomain.RegisterUser(user);

            _mockRepo.Verify(r => r.AddUser(user), Times.Once);
        }

        [Test]
        public void RegisterUser_DuplicateUsername_ThrowsInvalidOperationException()
        {
            var user = CreateValidUser();
            _mockRepo.Setup(r => r.GetUserByUsername(user.Username)).Returns(new User());

            var ex = Assert.Throws<InvalidOperationException>(() => _userDomain.RegisterUser(user));
            Assert.That(ex.Message, Does.Contain("already exists"));
        }

        [Test]
        public void RegisterUser_InvalidUser_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => _userDomain.RegisterUser(user));
            _mockRepo.Verify(r => r.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void RegisterUser_RepositoryThrowsError_RethrowsException()
        {
            var user = CreateValidUser();
            _mockRepo.Setup(r => r.GetUserByUsername(It.IsAny<string>())).Throws(new Exception("DB Error"));

            var ex = Assert.Throws<Exception>(() => _userDomain.RegisterUser(user));
            Assert.That(ex.Message, Is.EqualTo("DB Error"));
        }
        #endregion

        #region Login User
        [Test]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var user = CreateValidUser();
            _mockRepo.Setup(r => r.GetUserByUsername("TestUser")).Returns(user);

            var result = _userDomain.Login("TestUser", "password123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("TestUser"));
        }

        [Test]
        public void Login_UserNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("GhostUser")).Returns((User)null);

            var result = _userDomain.Login("GhostUser", "pass");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Login_WrongPassword_ReturnsNull()
        {
            var user = CreateValidUser();
            _mockRepo.Setup(r => r.GetUserByUsername("TestUser")).Returns(user);

            var result = _userDomain.Login("TestUser", "WRONG_PASS");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Login_RepositoryError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername(It.IsAny<string>())).Throws(new Exception("Connection failed!"));

            Assert.Throws<Exception>(() => _userDomain.Login("U", "P"));
        }
        #endregion

        #region Fitness Calculator BMI
        [Test]
        public void CalculateBMI_ValidUser_ReturnsCorrectValue()
        {
            var user = new User
            {
                WeightKg = 80,
                HeightCm = 200
            };
            _mockRepo.Setup(r => r.GetUserByUsername("TallGuy")).Returns(user);

            double bmi = _userDomain.CalculateBMI("TallGuy");

            Assert.That(bmi, Is.EqualTo(20.0).Within(0.1));
        }

        [Test]
        public void CalculateBMI_UserNotFound_ReturnsZero()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("Ghost")).Returns((User)null);
            Assert.That(_userDomain.CalculateBMI("Ghost"), Is.EqualTo(0));
        }

        [Test]
        public void CalculateBMI_ZeroHeight_ReturnsZero()
        {
            var user = new User
            {
                WeightKg = 80,
                HeightCm = 0
            };
            _mockRepo.Setup(r => r.GetUserByUsername("Bug")).Returns(user);

            Assert.That(_userDomain.CalculateBMI("Bug"), Is.EqualTo(0));
        }

        [Test]
        public void CalculateBMI_ZeroWeight_ReturnsZero()
        {
            var user = new User
            {
                WeightKg = 0,
                HeightCm = 180
            };
            _mockRepo.Setup(r => r.GetUserByUsername("Bug")).Returns(user);

            Assert.That(_userDomain.CalculateBMI("Bug"), Is.EqualTo(0));
        }

        [Test]
        public void CalculateBMI_RepoError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername(It.IsAny<string>())).Throws(new Exception("Fail"));
            Assert.Throws<Exception>(() => _userDomain.CalculateBMI("X"));
        }
        #endregion

        #region Fitness Calculator BMR
        [Test]
        public void CalculateBMR_UserNotFound_ReturnsZero()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("Ghost")).Returns((User)null);
            Assert.That(_userDomain.CalculateBMR("Ghost"), Is.EqualTo(0));
        }

        [Test]
        public void CalculateBMR_InvalidData_ReturnsZero()
        {
            var user = new User
            {
                WeightKg = -5
            };
            _mockRepo.Setup(r => r.GetUserByUsername("Invalid")).Returns(user);
            Assert.That(_userDomain.CalculateBMR("Invalid"), Is.EqualTo(0));
        }

        [Test]
        public void CalculateBMR_RepoError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("X")).Throws(new Exception("DB"));

        }
        #endregion

        #region Tracker for Remaining Calories
        [Test]
        public void CalculateRemaining_ValidUser_ReturnsCorrectMath()
        {
            var user = new User { DailyCalorieGoal = 2000 };
            _mockRepo.Setup(r => r.GetUserByUsername("User")).Returns(user);

            int result = _userDomain.CalculateRemainingCalories("User", 1000, 500);
            Assert.That(result, Is.EqualTo(1500));
        }

        [Test]
        public void CalculateRemaining_UserNotFound_ThrowsArgumentException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("Ghost")).Returns((User)null);
            Assert.Throws<ArgumentException>(() => _userDomain.CalculateRemainingCalories("Ghost", 0, 0));
        }

        [Test]
        public void CalculateRemaining_RepoError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("X")).Throws(new Exception());
            Assert.Throws<Exception>(() => _userDomain.CalculateRemainingCalories("X", 0, 0));
        }
        #endregion for c

        #region Tracker for Budget
        [Test]
        public void WouldExceedBudget_UserNotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("Ghost")).Returns((User)null);
            Assert.Throws<ArgumentException>(() => _userDomain.WouldExceedBudget("Ghost", 0, 0, 0));
        }

        [Test]
        public void WouldExceedBudget_RepoError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("X")).Throws(new Exception());
            Assert.Throws<Exception>(() => _userDomain.WouldExceedBudget("X", 0, 0, 0));
        }
        #endregion

        #region Tracker for Daily Progress
        [Test]
        public void GetProgress_OverLimit_ReturnsWarning()
        {
            var user = new User { DailyCalorieGoal = 2000 };
            _mockRepo.Setup(r => r.GetUserByUsername("U")).Returns(user);

            string status = _userDomain.GetDailyProgressStatus("U", 2500);
            Assert.That(status, Does.Contain("Warning"));
        }

        [Test]
        public void GetProgress_NearLimit_ReturnsCareful()
        {
            var user = new User { DailyCalorieGoal = 100 };
            _mockRepo.Setup(r => r.GetUserByUsername("U")).Returns(user);

            string status = _userDomain.GetDailyProgressStatus("U", 90);
            Assert.That(status, Does.Contain("Careful"));
        }

        [Test]
        public void GetProgress_SafeZone_ReturnsGreat()
        {
            var user = new User { DailyCalorieGoal = 100 };
            _mockRepo.Setup(r => r.GetUserByUsername("U")).Returns(user);

            string status = _userDomain.GetDailyProgressStatus("U", 50);
            Assert.That(status, Does.Contain("doing great"));
        }
        [Test]
        public void GetProgress_UserNotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("Ghost")).Returns((User)null);
            Assert.Throws<ArgumentException>(() => _userDomain.GetDailyProgressStatus("Ghost", 0));
        }

        [Test]
        public void GetProgress_RepoError_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetUserByUsername("X")).Throws(new Exception());
            Assert.Throws<Exception>(() => _userDomain.GetDailyProgressStatus("X", 0));
        }
        #endregion
    }
}
