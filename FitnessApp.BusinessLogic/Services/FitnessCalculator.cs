using System;
using System.Data;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLogic.Helpers;

namespace FitnessApp.BusinessLogic.Services
{
    public class FitnessCalculator
    {
        private IUserDomain _userDomain { get; set; }

        public FitnessCalculator(IUserDomain userDomain)
        {
            _userDomain = userDomain ?? throw new ArgumentNullException(nameof(userDomain));
            FileLogger.LogInfo("FitnessCalculator initialized successfully");
        }

        ///BMI (Body Mass Index): weight(kg) / height(m)^2
        public double CalculateBMI(string username)
        {
            FileLogger.LogDebug($"FitnessCalculator: Calculating BMI for '{username}'");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("CalculateBMI called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("CalculateBMI called with empty username");
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                }

                double bmi = _userDomain.CalculateBMI(username);

                if (bmi > 0)
                {
                    FileLogger.LogInfo($"FitnessCalculator: BMI calculated for '{username}': {bmi}");
                }

                return bmi;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"FitnessCalculator: Unexpected error calculating BMI for '{username}'", ex);
                throw;
            }
        }

        /// BMR (Basal Metabolic Rate) : (10 x weight) + (6.25 x height) - (5 x age) + s ; Mifflin-St Jeor formula
        public double CalculateBMR(string username)
        {
            FileLogger.LogDebug($"FitnessCalculator: Calculating BMR for '{username}'");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("CalculateBMR called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("CalculateBMR called with empty username");
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                }

                double bmr = _userDomain.CalculateBMR(username);

                if (bmr > 0)
                {
                    FileLogger.LogInfo($" FitnessCalculator: BMR calculated for '{username}': {bmr} cal/day");
                }

                return bmr;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"FitnessCalculator: Unexpected error calculating BMR for '{username}'", ex);
                throw;
            }
        }

        /// <summary>
        /// Calculates Total Daily Energy Expenditure (TDEE) based on activity level
        /// </summary>
        public double CalculateTDEE(string username, string activityLevel)
        {
            FileLogger.LogDebug($"FitnessCalculator: Calculating TDEE for '{username}' with activity level '{activityLevel}'");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("CalculateTDEE called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(activityLevel))
                {
                    FileLogger.LogWarning($"CalculateTDEE called with empty activity level for '{username}'");
                    throw new ArgumentException("Activity level cannot be empty.", nameof(activityLevel));
                }

                double bmr = _userDomain.CalculateBMR(username);

                if (bmr == 0)
                {
                    FileLogger.LogWarning($"TDEE calculation skipped for '{username}': BMR is 0");
                    return 0;
                }

                double multiplier;
                string normalizedActivity = activityLevel.ToLower();

                switch (normalizedActivity)
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
                        FileLogger.LogWarning($"Unknown activity level '{activityLevel}' for user '{username}', defaulting to sedentary");
                        multiplier = 1.2;
                        break;
                }

                double tdee = Math.Round(bmr * multiplier, 0);
                FileLogger.LogInfo($"✓ FitnessCalculator: TDEE calculated for '{username}': {tdee} cal/day (Activity: {activityLevel}, Multiplier: {multiplier})");

                return tdee;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"FitnessCalculator: Error calculating TDEE for '{username}'", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets BMI category based on BMI value
        /// </summary>
        public string GetBMICategory(double bmi)
        {
            FileLogger.LogDebug($"FitnessCalculator: Getting BMI category for BMI value {bmi}");

            try
            {
                if (bmi <= 0)
                {
                    FileLogger.LogWarning($"Invalid BMI value: {bmi}");
                    return "Invalid BMI";
                }

                string category;

                if (bmi < 18.5)
                    category = "Underweight";
                else if (bmi < 25)
                    category = "Normal weight";
                else if (bmi < 30)
                    category = "Overweight";
                else
                    category = "Obese";

                FileLogger.LogInfo($"FitnessCalculator: BMI {bmi} categorized as '{category}'");

                return category;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"FitnessCalculator: Error getting BMI category for BMI {bmi}", ex);
                throw;
            }

        }

        /// <summary>
        /// Calculates recommended daily calorie intake for weight goal
        /// </summary>
        public int CalculateCaloriesForGoal(string username, string goal, string activityLevel)
        {
            FileLogger.LogDebug($"FitnessCalculator: Calculating calories for '{username}' with goal '{goal}' and activity '{activityLevel}'");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("CalculateCaloriesForGoal called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                double tdee = CalculateTDEE(username, activityLevel);

                if (tdee == 0)
                {
                    FileLogger.LogWarning($"Calorie goal calculation skipped for '{username}': TDEE is 0");
                    return 0;
                }

                int calories;
                string normalizedGoal = goal.ToLower();

                switch (normalizedGoal)
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
                        FileLogger.LogWarning($"Unknown goal '{goal}' for user '{username}', defaulting to maintain");
                        calories = (int)tdee;
                        break;
                }

                FileLogger.LogInfo($" FitnessCalculator: Calorie goal calculated for '{username}': {calories} cal/day (Goal: {goal}, TDEE: {tdee})");

                return calories;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"FitnessCalculator: Error calculating calories for goal for '{username}'", ex);
                throw;
            }
        } 

    }
}