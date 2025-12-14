using NUnit.Framework;
using Moq;
using FitnessApp.BusinessLogic.Services;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.Models;
using System;

namespace FitnessApp.Tests.UnitTests.BuisinessLogic
{
    [TestFixture]
    public class AuthenticationTests
    {
        private Mock<IUserDomain> _mockUserDomain;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _mockUserDomain = new Mock<IUserDomain>();
            _authService = new AuthenticationService(_mockUserDomain.Object);
        }

        //verifies that registering a new, valid user calls the AddUser method in the domain layer.
        [Test]
        public void RegisterUser_ValidUser_CallsAddUserInRepository()
        {

            var newUser = new User("ValidUser", "test@email.com", 70, 175, 2000);
            newUser.PasswordHash = "safePssword1234";
            newUser.BirthDate = new DateTime(2000, 1, 1);

            _authService.RegisterUser(newUser);

            _mockUserDomain.Verify(d => d.RegisterUser(newUser), Times.Once);
        }
        //ensures that attempting to register an existing username throws an InvalidOperationException.
        [Test]
        public void RegisterUser_DomainThrowsException_ServiceBubblesException()
        {
            var newUser = new User("DuplicateUser", "test@email.com", 70, 175, 2000);

            _mockUserDomain.Setup(d => d.RegisterUser(It.IsAny<User>()))
                           .Throws(new InvalidOperationException("Username already exists."));

            var ex = Assert.Throws<InvalidOperationException>(() => _authService.RegisterUser(newUser));

            Assert.That(ex.Message, Is.EqualTo("Username already exists."));
        }
    }
}