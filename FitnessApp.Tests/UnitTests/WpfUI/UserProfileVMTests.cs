using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.Models;
using FitnessApp.WpfUI.ViewModels;
using Moq;

namespace FitnessApp.Tests.UnitTests.WpfUI
{
    [TestFixture]
    public class UserProfileVMTests
    {
        private User CreateTestUser()
        {
            return new User
            {
                Username = "TestUser",
                BirthDate = new DateTime(2000, 1, 1),
                HeightCm = 180,
                WeightKg = 80,
                TargetWeightKg = 75
            };
        }

        [Test]
        public void Constructor_ShouldInitializeCommands()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            var vm = new UserProfileVM(user, domainMock.Object);

            Assert.NotNull(vm.UpdateAccountCommand);
            Assert.NotNull(vm.DeleteAccountCommand);
            Assert.NotNull(vm.LogOutCommand);
        }

        [Test]
        public void Username_AndWelcomeMessage_ShouldBeCorrect()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            var vm = new UserProfileVM(user, domainMock.Object);

            Assert.AreEqual("TestUser", vm.Username);
            Assert.AreEqual("Welcome, TestUser!", vm.WelcomeMessage);
        }

        [Test]
        public void Age_ShouldBeCalculatedCorrectly()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            var vm = new UserProfileVM(user, domainMock.Object);

            var expectedAge = DateTime.Today.Year - 2000;
            if (new DateTime(2000, 1, 1) > DateTime.Today.AddYears(-expectedAge))
                expectedAge--;

            Assert.AreEqual(expectedAge, vm.Age);
        }

        [Test]
        public void UserMeasurements_ShouldReturnCorrectValues()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            var vm = new UserProfileVM(user, domainMock.Object);

            Assert.AreEqual(180, vm.Height);
            Assert.AreEqual(80, vm.CurrentWeight);
            Assert.AreEqual(75, vm.TargetWeight);
        }

        [Test]
        public void UserBMI_ShouldCallCalculateBMI()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            domainMock
                .Setup(d => d.CalculateBMI("TestUser"))
                .Returns(24.7);

            var vm = new UserProfileVM(user, domainMock.Object);

            var bmi = vm.UserBMI;

            Assert.AreEqual(24.7, bmi);
            domainMock.Verify(d => d.CalculateBMI("TestUser"), Times.Once);
        }


        [Test]
        public void UserBMR_ShouldCallCalculateBMR()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            domainMock
                .Setup(d => d.CalculateBMR("TestUser"))
                .Returns(1700);

            var vm = new UserProfileVM(user, domainMock.Object);

            var bmr = vm.UserBMR;

            Assert.AreEqual(1700, bmr);
            domainMock.Verify(d => d.CalculateBMR("TestUser"), Times.Once);
        }

        [Test]
        public void DeleteAccount_WhenDeleteThrows_ShouldSetErrorMessage()
        {
            var user = CreateTestUser();
            var domainMock = new Mock<IUserDomain>();

            domainMock
                .Setup(d => d.DeleteUser(It.IsAny<User>()))
                .Throws(new InvalidOperationException("Delete failed"));

            var vm = new UserProfileVM(user, domainMock.Object);

            try
            {
                vm.DeleteAccount();
            }
            catch { }

            Assert.AreEqual("Delete failed", vm.ErrorMessage);
        }

    }
}
