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
        private readonly IUserDomain _userDomain;

        // for now we will mock this dependency bcs we don't have a real DB 
        public AuthenticationService(IUserDomain userDomain)
        {
            _userDomain = userDomain ?? throw new ArgumentNullException(nameof(userDomain));
            FileLogger.LogInfo("AuthenticationService initialized successfully");
        }

        public void RegisterUser(User user)
        {
            FileLogger.LogInfo($"AuthenticationService: Starting user registration for '{user?.Username}'");

            try
            {
                if (user == null)
                {
                    FileLogger.LogWarning("RegisterUser called with null user object");
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");
                }

                _userDomain.RegisterUser(user);

                FileLogger.LogInfo($"✓ AuthenticationService: User '{user.Username}' registered successfully");
            }
            catch (ArgumentException ex)
            {
                FileLogger.LogWarning($"AuthenticationService: Registration validation failed for '{user?.Username}': {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                FileLogger.LogWarning($"AuthenticationService: Registration failed for '{user?.Username}': {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"AuthenticationService: Unexpected error during registration for '{user?.Username}'", ex);
                throw;
            }
        }

        public User Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("Login attempt with empty or null username");
                    throw new ArgumentException("Username cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    FileLogger.LogWarning($"Login attempt with empty password for username '{username}'");
                    throw new ArgumentException("Password cannot be empty.");
                }

                var result = _userDomain.Login(username, password);

                if (result == null)
                {
                
                    FileLogger.LogWarning($"AuthenticationService: Login failed for username '{username}' (invalid credentials)");
                }
                else
                {
                    FileLogger.LogInfo($"AuthenticationService: Login successful for username '{username}'");
                }

                return result;
            }
            catch (ArgumentException ex)
            {
                FileLogger.LogWarning($"AuthenticationService: Login validation failed for '{username}': {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"AuthenticationService: Unexpected error during login for '{username}'", ex);
                throw;
            }
        }
    }
}