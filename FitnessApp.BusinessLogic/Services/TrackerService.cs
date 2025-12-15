using System;
using FitnessApp.BusinessLogic.Interfaces;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLogic.Helpers;

namespace FitnessApp.BusinessLogic.Services
{
    public class TrackerService : ITrackerService
    {
        private IUserDomain _userDomain { get; set; }

        public TrackerService(IUserDomain userDomain)
        {
            _userDomain = userDomain ?? throw new ArgumentNullException(nameof(userDomain));
        }

        public int CalculateRemainingCalories(string username, int consumed, int burned)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));

                if (consumed < 0)
                    throw new ArgumentException("Consumed calories cannot be negative.", nameof(consumed));

                if (burned < 0)
                    throw new ArgumentException("Burned calories cannot be negative.", nameof(burned));

                return _userDomain.CalculateRemainingCalories(username, consumed, burned);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error calculating remaining calories for user {username}", ex);
                throw;
            }
        }
        public bool WouldExceedBudget(string username, int burnedToday,int currentTotal, int newFoodCalories)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));

                if (burnedToday < 0)
                    throw new ArgumentException("Burned calories cannot be negative.", nameof(burnedToday));

                if (currentTotal < 0)
                    throw new ArgumentException("Current total cannot be negative.", nameof(currentTotal));

                if (newFoodCalories < 0)
                    throw new ArgumentException("New food calories cannot be negative.", nameof(newFoodCalories));

                return _userDomain.WouldExceedBudget(username, burnedToday, currentTotal, newFoodCalories);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error checking budget for user {username}", ex);
                throw;
            }
        }

        public string GetDailyProgressStatus(string username, int consumed)
        {
            try
            {
                if (username == null)
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty.", nameof(username));

                if (consumed < 0)
                    throw new ArgumentException("Consumed calories cannot be negative.", nameof(consumed));

                return _userDomain.GetDailyProgressStatus(username, consumed);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error getting daily progress for user {username}", ex);
                throw;
            }
        }
    }
}