using System;
using FitnessApp.BusinessLogic.Interfaces;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;

namespace FitnessApp.BusinessLogic.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDomain _userDomain;

        // for now we will mock this dependency bcs we don't have a real DB 
        public AuthenticationService(UserDomain userDomain)
        {
            _userDomain = userDomain;
        }

        public void RegisterUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            _userDomain.RegisterUser(user);
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            return _userDomain.Login(username, password);
        }
    }
}