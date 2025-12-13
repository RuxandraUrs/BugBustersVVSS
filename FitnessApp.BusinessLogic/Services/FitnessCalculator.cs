using System;
using System.Data;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLogic.Helpers;

namespace FitnessApp.BusinessLogic.Services
{
    public class FitnessCalculator
    {
        private UserDomain _userDomain { get; set; }

        public FitnessCalculator(UserDomain userDomain)
        {
            _userDomain = userDomain ?? throw new ArgumentNullException(nameof(userDomain));
        }

        ///BMI (Body Mass Index): weight(kg) / height(m)^2
        public double CalculateBMI(string username)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));

                return _userDomain.CalculateBMI(username);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating BMI for user {username}", ex);
                throw;
            }
        }

        /// BMR (Basal Metabolic Rate) : (10 x weight) + (6.25 x height) - (5 x age) + s ; Mifflin-St Jeor formula
        public double CalculateBMR(string username)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));

                return _userDomain.CalculateBMR(username);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating BMR for user {username}", ex);
                throw;
            }
        }

        /// <summary>
        /// Calculates Total Daily Energy Expenditure (TDEE) based on activity level
        /// </summary>
        public double CalculateTDEE(string username, string activityLevel)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(activityLevel))
                    throw new ArgumentException("Activity level cannot be empty.", nameof(activityLevel));

                double bmr = _userDomain.CalculateBMR(username);

                if (bmr == 0)
                    return 0;

                double multiplier;

                switch (activityLevel.ToLower())
                {
                    case "sedentary":
                        multiplier = 1.2;
                        break;
                    case "light":
                        multiplier = 1.375;
                        break;
                    case "moderate":
                        multiplier = 1.55;
                        break;
                    case "active":
                        multiplier = 1.725;
                        break;
                    case "veryactive":
                        multiplier = 1.9;
                        break;
                    default:
                        multiplier = 1.2;
                        break;
                }

                return Math.Round(bmr * multiplier, 0);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating TDEE for user {username}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets BMI category based on BMI value
        /// </summary>
        public string GetBMICategory(double bmi)
        {
            try
            {
                if (bmi <= 0)
                    return "Invalid BMI";

                if (bmi < 18.5)
                    return "Underweight";
                else if (bmi < 25)
                    return "Normal weight";
                else if (bmi < 30)
                    return "Overweight";
                else
                    return "Obese";
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error getting BMI category for BMI {bmi}", ex);
                throw;
            }

        }

        /// <summary>
        /// Calculates recommended daily calorie intake for weight goal
        /// </summary>
        public int CalculateCaloriesForGoal(string username, string goal, string activityLevel)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                double tdee = CalculateTDEE(username, activityLevel);

                if (tdee == 0)
                    return 0;

                int calories;

                switch (goal.ToLower())
                {
                    case "lose":
                        calories = (int)(tdee - 500);
                        break;
                    case "maintain":
                        calories = (int)tdee;
                        break;
                    case "gain":
                        calories = (int)(tdee + 500);
                        break;
                    default:
                        calories = (int)tdee;
                        break;
                }

                return calories;

            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating calories for goal for user {username}", ex);
                throw;
            }
        } 

    }
}