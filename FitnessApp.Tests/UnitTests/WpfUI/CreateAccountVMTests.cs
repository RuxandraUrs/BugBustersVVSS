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
    public class CreateAccountVMTests
    {
        [Test]
        public void Constructor_ShouldInitializeCommands()
        {
            var userDomainMock = new Mock<IUserDomain>();
            var vm = new CreateAccountVM(userDomainMock.Object);

            Assert.NotNull(vm.CreateAccountCommand);
            Assert.NotNull(vm.CancelCommand);
        }

        [Test]
        public void CreateAccountCommand_ValidData_CallsRegisterUser()
        {
            var userDomainMock = new Mock<IUserDomain>();
            var vm = new CreateAccountVM(userDomainMock.Object)
            {
                Username = "TestUser",
                Password = "password123",
                Email = "test@test.com",
                BirthDate = new DateTime(2000, 1, 1),
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 75,
                TargetWeightKg = 70,
                DailyCalorieGoal = 2000
            };

            vm.CreateAccountCommand.Execute(null);

            userDomainMock.Verify(
                u => u.RegisterUser(It.Is<User>(user =>
                    user.Username == "TestUser" &&
                    user.PasswordHash == "password123" &&
                    user.Email == "test@test.com" &&
                    user.HeightCm == 180 &&
                    user.WeightKg == 75 &&
                    user.DailyCalorieGoal == 2000
                )),
                Times.Once
            );
        }

        [Test]
        public void CreateAccount_NullNumericValues_ShouldDefaultToZero()
        {
            var userDomainMock = new Mock<IUserDomain>();
            var vm = new CreateAccountVM(userDomainMock.Object)
            {
                Username = "TestUser",
                Password = "password123",
                HeightCm = null,
                WeightKg = null,
                TargetWeightKg = null,
                DailyCalorieGoal = null
            };

            vm.CreateAccountCommand.Execute(null);

            userDomainMock.Verify(u => u.RegisterUser(
                It.Is<User>(user =>
                    user.HeightCm == 0 &&
                    user.WeightKg == 0 &&
                    user.TargetWeightKg == 0 &&
                    user.DailyCalorieGoal == 0
                )
            ));
        }

        [Test]
        public void CreateAccount_WhenRegisterThrows_ShouldSetErrorMessage()
        {
            var userDomainMock = new Mock<IUserDomain>();
            userDomainMock
                .Setup(u => u.RegisterUser(It.IsAny<User>()))
                .Throws(new InvalidOperationException("Username already exists"));

            var vm = new CreateAccountVM(userDomainMock.Object)
            {
                Username = "TestUser",
                Password = "password123"
            };

            vm.CreateAccountCommand.Execute(null);

            Assert.AreEqual("Username already exists", vm.ErrorMessage);
        }

        [Test]
        public void Gender_Set_ShouldRaisePropertyChanged()
        {
            var userDomainMock = new Mock<IUserDomain>();
            var vm = new CreateAccountVM(userDomainMock.Object);

            bool raised = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Gender))
                    raised = true;
            };

            vm.Gender = "Male";

            Assert.IsTrue(raised);
        }

        [Test]
        public void GenderOptions_ShouldContainDefaultValues()
        {
            var vm = new CreateAccountVM(new Mock<IUserDomain>().Object);

            CollectionAssert.AreEqual(
                new[] { "Male", "Female", "Other" },
                vm.GenderOptions
            );
        }

    }
}
