using System;
using FitnessApp.BusinessLogic.Interfaces;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using FitnessApp.BusinessLogic.Helpers;

namespace FitnessApp.BusinessLogic.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDomain _userDomain;

        // for now we will mock this dependency bcs we don't have a real DB 
        public AuthenticationService(UserDomain userDomain)
        {
            _userDomain = userDomain ?? throw new ArgumentNullException(nameof(userDomain));
        }

        public void RegisterUser(User user)
        {
            try
            {
                if (user == null)
                {
                    FileLogger.LogError("RegisterUser called with null user");
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");
                }

                _userDomain.RegisterUser(user);
                FileLogger.LogError($"User registration successful via AuthenticationService: {user.Username}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error in AuthenticationService.RegisterUser for user: {user?.Username}", ex);
                throw;
            }
        }

        public User Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogError("Login attempt with empty username");
                    throw new ArgumentException("Username cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    FileLogger.LogError($"Login attempt with empty password for username: {username}");
                    throw new ArgumentException("Password cannot be empty.");
                }

                var result = _userDomain.Login(username, password);

                if (result == null)
                {
                    FileLogger.LogError($"Login failed in AuthenticationService for username: {username}");
                }
                else
                {
                    FileLogger.LogError($"Login successful in AuthenticationService for username: {username}");
                }

                return result;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error in AuthenticationService.Login for username: {username}", ex);
                throw;
            }
        }
    }
}