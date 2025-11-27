using System;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.BusinessLoic.Interfaces;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;

namespace FitnessApp.BusinessLogic.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository; 
        private readonly UserValidator _validator;

        // for now we will mock this dependency bcs we don't have a real DB 
        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _validator = new UserValidator();
        }

        public void RegisterUser(User user)
        {
            if (!_validator.Validate(user, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var existingUser = _userRepository.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            _userRepository.AddUser(user);
        }

        public User Login(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null || user.PasswordHash != password)
            {
                return null;
            }

            return user;
        }
    }
}