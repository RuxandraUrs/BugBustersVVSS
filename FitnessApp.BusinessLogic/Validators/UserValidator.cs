using System;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;

namespace FitnessApp.BusinessLogic.Validators
{
    public class UserValidator
    {
        public bool ValidateRegistration(User user, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (user == null)
            {
                errorMessage = "User cannot be null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Length <= 4)
            {
                errorMessage = "Username must be longer than 4 characters.";
                return false;
            }

            if (string.IsNullOrEmpty(user.PasswordHash) || user.PasswordHash.Length < 6)
            {
                errorMessage = "Password is too short (minimum 6 characters).";
                return false;
            }

            int age = CalculateAge(user.BirthDate);
            if (age < 12)
            {
                errorMessage = "You must be at least 12 years old to use the application.";
                return false;
            }

            if (user.WeightKg <= 0 || user.HeightCm <= 0)
            {
                errorMessage = "Weight and height must be positive values.";
                return false;
            }

            return true;
        }

        public bool ValidateLogin(IUserRepository userRepository, string name, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            User user = userRepository.GetUserByUsername(name);

            if (user == null)
            {
                errorMessage = "User wasn't found.";
                return false;
            }

            if (string.IsNullOrEmpty(password) || password != user.PasswordHash)
            {
                errorMessage = "Password is incorrect.";
                return false;
            }

            return true;
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}