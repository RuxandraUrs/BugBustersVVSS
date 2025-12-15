using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.Helpers;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.Models;

namespace FitnessApp.BusinessLogic.Domains
{
    public class UserDomain : IUserDomain
    {
        private IUserRepository userRepository { get; set; }
        private readonly UserValidator _validator;

        public void AddUser(User user)
        {
            userRepository.AddUser(user);
        }

        public User GetUserByUsername(string username)
        {
            return userRepository.GetUserByUsername(username);
        }
        public UserDomain(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            _validator = new UserValidator();
        }

        #region Authentication
        public void RegisterUser(User user)
        {
            try
            {
                if (!_validator.Validate(user, out string errorMessage))
                {
                    FileLogger.LogError($"User validation failed: {errorMessage}");
                    throw new ArgumentException(errorMessage);
                }

                var existingUser = userRepository.GetUserByUsername(user.Username);
                if (existingUser != null)
                {
                    FileLogger.LogError($"Registration failed: Username '{user.Username}' already exists");
                    throw new InvalidOperationException("Username already exists.");
                }

                userRepository.AddUser(user);
                FileLogger.LogError($"User registered successfully: {user.Username}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error during user registration for username: {user?.Username}", ex);
                throw;
            }
        }

        public User Login(string username, string password)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);

                if (user == null || user.PasswordHash != password)
                {
                    FileLogger.LogError($"Login failed for username: {username}");
                    return null;
                }

                FileLogger.LogError($"User logged in successfully: {username}");
                return user;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error during login for username: {username}", ex);
                throw;
            }
        }
        #endregion

        #region Fitness Calculator
        public double CalculateBMI(string username)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogError($"BMI calculation failed: User '{username}' not found");
                    return 0;
                }

                double weightKg = user.WeightKg;
                double heightCm = user.HeightCm;

                if (heightCm <= 0 || weightKg <= 0)
                {
                    FileLogger.LogError($"BMI calculation failed: Invalid height or weight for user '{username}'");
                    return 0;
                }

                double heightM = heightCm / 100.0;
                double bmi = weightKg / (heightM * heightM);

                FileLogger.LogError($"BMI calculated for user '{username}': {Math.Round(bmi, 2)}");
                return Math.Round(bmi, 2);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating BMI for user: {username}", ex);
                throw;
            }
        }

        public double CalculateBMR(string username)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogError($"BMR calculation failed: User '{username}' not found");
                    return 0;
                }

                double weightKg = user.WeightKg;
                double heightCm = user.HeightCm;
                int age = DateTime.Now.Year - user.BirthDate.Year;
                string gender = user.Gender;

                if (weightKg <= 0 || heightCm <= 0 || age <= 0)
                {
                    FileLogger.LogError($"BMR calculation failed: Invalid data for user '{username}'");
                    return 0;
                }

                double bmr = (10 * weightKg) + (6.25 * heightCm) - (5 * age);

                if (gender != null && gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
                {
                    bmr += 5;
                }
                else
                {
                    bmr -= 161;
                }

                FileLogger.LogError($"BMR calculated for user '{username}': {Math.Round(bmr, 0)}");
                return Math.Round(bmr, 0);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating BMR for user: {username}", ex);
                throw;
            }
        }
        #endregion

        #region Tracker
        public int CalculateRemainingCalories(string username, int consumed, int burned)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogError($"Calculate remaining calories failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int remaining = (user.DailyCalorieGoal + burned) - consumed;
                FileLogger.LogError($"Remaining calories calculated for '{username}': {remaining}");
                return remaining;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating remaining calories for user: {username}", ex);
                throw;
            }
        }
        public bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogError($"Budget check failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int dailyGoal = user.DailyCalorieGoal;
                int limit = dailyGoal + burnedToday;
                bool wouldExceed = (currentTotal + newFoodCalories) > limit;

                FileLogger.LogError($"Budget check for '{username}': Would exceed = {wouldExceed}");
                return wouldExceed;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error checking budget for user: {username}", ex);
                throw;
            }
        }

        public string GetDailyProgressStatus(string username, int consumed)
        {
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogError($"Progress status failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int dailyGoal = user.DailyCalorieGoal;
                double percentage = (double)consumed / dailyGoal * 100;

                string status;
                if (percentage >= 100)
                {
                    status = "Warning: You have exceeded your daily calorie limit!";
                }
                else if (percentage >= 85)
                {
                    status = "Careful: You are close to your daily limit.";
                }
                else
                {
                    status = $"You are doing great! {dailyGoal - consumed} calories remaining.";
                }

                FileLogger.LogError($"Progress status for '{username}': {status}");
                return status;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error getting daily progress for user: {username}", ex);
                throw;
            }
        }
        #endregion
    }
}
