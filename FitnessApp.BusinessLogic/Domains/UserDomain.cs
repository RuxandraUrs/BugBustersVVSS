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
            FileLogger.LogInfo("UserDomain initialized successfully");

        }

        #region Authentication
        public void RegisterUser(User user)
        {
            FileLogger.LogInfo($"Attempting to register user: {user?.Username}");
            try
            {
                if (!_validator.ValidateRegistration(user, out string errorMessage))
                {
                    FileLogger.LogWarning($"User validation failed for '{user?.Username}': {errorMessage}");
                    throw new ArgumentException(errorMessage);
                }

                var existingUser = userRepository.GetUserByUsername(user.Username);
                if (existingUser != null)
                {
                    FileLogger.LogWarning($"Registration failed: Username '{user.Username}' already exists");
                    throw new InvalidOperationException("Username already exists.");
                }

                userRepository.AddUser(user);
                FileLogger.LogInfo($" User registered successfully: {user.Username}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error during user registration for username: {user?.Username}", ex);
                throw;
            }
        }

        public User Login(string username, string password)
        {
            FileLogger.LogInfo($"Login attempt for username: {username}");
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    FileLogger.LogWarning($"Login failed: Empty username or password.");
                    throw new ArgumentException("Username and password are required.");
                }

                var user = userRepository.GetUserByUsername(username);

                if (user == null || user.PasswordHash != password)
                {
                    FileLogger.LogWarning($"Login failed: User '{username}' not found");
                    return null;
                }

                FileLogger.LogInfo($" User logged in successfully: {username}");
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

            FileLogger.LogDebug($"Calculating BMI for user: {username}");
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogWarning($"BMI calculation failed: User '{username}' not found");
                    return 0;
                }

                double weightKg = user.WeightKg;
                double heightCm = user.HeightCm;

                if (heightCm <= 0 || weightKg <= 0)
                {
                    FileLogger.LogWarning($"BMI calculation failed: Invalid height ({heightCm}cm) or weight ({weightKg}kg) for user '{username}'");
                    return 0;
                }

                double heightM = heightCm / 100.0;
                double bmi = weightKg / (heightM * heightM);

                FileLogger.LogInfo($" BMI calculated for user '{username}': {Math.Round(bmi, 2)} (Weight: {weightKg}kg, Height: {heightCm}cm)");
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
            FileLogger.LogDebug($"Calculating BMR for user: {username}");
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogWarning($"BMR calculation failed: User '{username}' not found");
                    return 0;
                }

                double weightKg = user.WeightKg;
                double heightCm = user.HeightCm;
                int age = DateTime.Now.Year - user.BirthDate.Year;
                string gender = user.Gender;

                if (weightKg <= 0 || heightCm <= 0 || age <= 0)
                {
                    FileLogger.LogWarning($"BMR calculation failed: Invalid data for user '{username}' (Weight: {weightKg}kg, Height: {heightCm}cm, Age: {age})");
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

                FileLogger.LogInfo($" BMR calculated for user '{username}': {Math.Round(bmr, 0)} cal/day (Age: {age}, Gender: {gender})");
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

            FileLogger.LogDebug($"Calculating remaining calories for '{username}' (Consumed: {consumed}, Burned: {burned})");
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogWarning($"Calculate remaining calories failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int remaining = (user.DailyCalorieGoal + burned) - consumed;
                FileLogger.LogInfo($"✓ Remaining calories for '{username}': {remaining} cal (Goal: {user.DailyCalorieGoal}, Consumed: {consumed}, Burned: {burned})");
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

            FileLogger.LogDebug($"Checking budget for '{username}' (Current: {currentTotal}, Adding: {newFoodCalories}, Burned: {burnedToday})");
            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogWarning($"Budget check failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int dailyGoal = user.DailyCalorieGoal;
                int limit = dailyGoal + burnedToday;
                int newTotal = currentTotal + newFoodCalories;
                bool wouldExceed = newTotal > limit;


                if (wouldExceed)
                {
                    FileLogger.LogWarning($"Budget check for '{username}': Adding {newFoodCalories} cal would EXCEED limit! ({newTotal}/{limit} cal)");
                }
                else
                {
                    FileLogger.LogInfo($"Budget check for '{username}': Within limit ({newTotal}/{limit} cal)");
                }

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
            FileLogger.LogDebug($"Getting daily progress status for '{username}' (Consumed: {consumed})");

            try
            {
                var user = userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    FileLogger.LogWarning($"Progress status failed: User '{username}' not found");
                    throw new ArgumentException("User not found.");
                }

                int dailyGoal = user.DailyCalorieGoal;
                double percentage = (double)consumed / dailyGoal * 100;

                string status;
                if (percentage >= 100)
                {
                    status = "Warning: You have exceeded your daily calorie limit!";
                    FileLogger.LogWarning($"User '{username}' EXCEEDED daily limit: {consumed}/{dailyGoal} cal ({percentage:F1}%)");
                }
                else if (percentage >= 85)
                {
                    status = "Careful: You are close to your daily limit.";
                    FileLogger.LogWarning($"User '{username}' approaching limit: {consumed}/{dailyGoal} cal ({percentage:F1}%)");
                }
                else
                {
                    status = $"You are doing great! {dailyGoal - consumed} calories remaining.";
                    FileLogger.LogInfo($"✓ User '{username}' on track: {consumed}/{dailyGoal} cal ({percentage:F1}%)");
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