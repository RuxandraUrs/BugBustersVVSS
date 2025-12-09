using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.Validators;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.Models;

namespace FitnessApp.BusinessLoic.Domains
{
    public class UserDomain
    {
        private IUserRepository userRepository { get; set; }
        private readonly UserValidator _validator;

        public UserDomain(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            _validator = new UserValidator();
        }

        #region Authentication
        public void RegisterUser(User user)
        {
            if (!_validator.Validate(user, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var existingUser = userRepository.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            userRepository.AddUser(user);
        }

        public User Login(string username, string password)
        {
            var user = userRepository.GetUserByUsername(username);

            if (user == null || user.PasswordHash != password)
            {
                return null;
            }

            return user;
        }
        #endregion

        #region Fitness Calculator
        public double CalculateBMI(string username)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null) return 0;

            double weightKg = user.WeightKg;
            double heightCm = user.HeightCm;
            if (heightCm <= 0 || weightKg <= 0) return 0;

            double heightM = heightCm / 100.0;
            double bmi = weightKg / (heightM * heightM);

            return Math.Round(bmi, 2);
        }

        public double CalculateBMR(string username)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null) return 0;

            double weightKg = user.WeightKg;
            double heightCm = user.HeightCm;
            int age = DateTime.Now.Year - user.BirthDate.Year;
            string gender = user.Gender;

            if (weightKg <= 0 || heightCm <= 0 || age <= 0) return 0;

            double bmr = (10 * weightKg) + (6.25 * heightCm) - (5 * age);

            if (gender != null && gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
            {
                bmr += 5;
            }
            else
            {
                bmr -= 161;
            }

            return Math.Round(bmr, 0);
        }
        #endregion

        #region Tracker
        public int CalculateRemainingCalories(string username, int consumed, int burned)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null) throw new ArgumentException("User not found.");

            return (user.DailyCalorieGoal + burned) - consumed;
        }
        public bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null) throw new ArgumentException("User not found.");
            int dailyGoal = user.DailyCalorieGoal;
            int limit = dailyGoal + burnedToday;

            return (currentTotal + newFoodCalories) > limit;
        }

        public string GetDailyProgressStatus(string username, int consumed)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null) throw new ArgumentException("User not found.");
            int dailyGoal = user.DailyCalorieGoal;

            double percentage = (double)consumed / dailyGoal * 100;

            if (percentage >= 100)
            {
                return "Warning: You have exceeded your daily calorie limit!";
            }
            else if (percentage >= 85)
            {
                return "Careful: You are close to your daily limit.";
            }
            else
            {
                return $"You are doing great! {dailyGoal - consumed} calories remaining.";
            }
        }
        #endregion
    }
}
