//using NUnit.Framework;
//using Moq;
//using FitnessApp.BusinessLogic.Services;
//using FitnessApp.DataAccess.Interfaces;
//using FitnessApp.Models;
//using System;

//namespace FitnessApp.Tests.UnitTests.BuisinessLogic
//{
//    [TestFixture]
//    public class AuthenticationTests
//    {
//        private Mock<IUserRepository> _mockRepository;
//        private AuthenticationService _authService;

//        [SetUp]
//        public void Setup()
//        {
//            _mockRepository = new Mock<IUserRepository>();
//            _authService = new AuthenticationService(_mockRepository.Object);
//        }

//        [Test]
//        public void RegisterUser_ValidUser_CallsAddUserInRepository()
//        {
            
//            var newUser = new User("ValidUser", "test@email.com", 70, 175, 2000);
//            newUser.PasswordHash = "safePssword1234"; 
//            newUser.BirthDate = new DateTime(2000, 1, 1);

            
//            _mockRepository.Setup(repo => repo.GetUserByUsername("ValidUser"))
//                           .Returns((User)null);

//            _authService.RegisterUser(newUser);

//            _mockRepository.Verify(repo => repo.AddUser(newUser), Times.Once);
//        }

//        [Test]
//        public void RegisterUser_ExistingUser_ThrowsException()
//        {
//            var existingUser = new User("OldUser", "old@email.com", 80, 180, 2000);
//            existingUser.PasswordHash = "pass123";

//            _mockRepository.Setup(repo => repo.GetUserByUsername("OldUser"))
//                           .Returns(existingUser);

//            var newUserDuplicate = new User("OldUser", "new@email.com", 90, 180, 2000);

//            newUserDuplicate.PasswordHash = "NewPassoword4321";
//            newUserDuplicate.BirthDate = new DateTime(2000, 1, 1);

//            var ex = Assert.Throws<InvalidOperationException>(() => _authService.RegisterUser(newUserDuplicate));

//            Assert.That(ex.Message, Is.EqualTo("Username already exists."));
//        }
//    }
//}