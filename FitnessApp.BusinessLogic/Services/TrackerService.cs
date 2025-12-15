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
            FileLogger.LogInfo("TrackerService initialized successfully");
        }

        public int CalculateRemainingCalories(string username, int consumed, int burned)
        {
            FileLogger.LogDebug($"TrackerService: Calculating remaining calories for '{username}' (Consumed: {consumed}, Burned: {burned})");
            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("CalculateRemainingCalories called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("CalculateRemainingCalories called with empty username");
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                }

                if (consumed < 0)
                {
                    FileLogger.LogWarning($"CalculateRemainingCalories: Invalid consumed calories ({consumed}) for user '{username}'");
                    throw new ArgumentException("Consumed calories cannot be negative.", nameof(consumed));
                }

                if (burned < 0)
                {
                    FileLogger.LogWarning($"CalculateRemainingCalories: Invalid burned calories ({burned}) for user '{username}'");
                    throw new ArgumentException("Burned calories cannot be negative.", nameof(burned));
                }

                int remaining = _userDomain.CalculateRemainingCalories(username, consumed, burned);

                FileLogger.LogInfo($"✓ TrackerService: Remaining calories calculated for '{username}': {remaining} cal");

                return remaining;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"TrackerService: Error calculating remaining calories for '{username}'", ex);
                throw;
            }
        }
        public bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories)
        {
            FileLogger.LogDebug($"TrackerService: Checking budget for '{username}' (Current: {currentTotal}, New: {newFoodCalories}, Burned: {burnedToday})");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("WouldExceedBudget called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("WouldExceedBudget called with empty username");
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                }

                if (burnedToday < 0)
                {
                    FileLogger.LogWarning($"WouldExceedBudget: Invalid burned calories ({burnedToday}) for user '{username}'");
                    throw new ArgumentException("Burned calories cannot be negative.", nameof(burnedToday));
                }

                if (currentTotal < 0)
                {
                    FileLogger.LogWarning($"WouldExceedBudget: Invalid current total ({currentTotal}) for user '{username}'");
                    throw new ArgumentException("Current total cannot be negative.", nameof(currentTotal));
                }

                if (newFoodCalories < 0)
                {
                    FileLogger.LogWarning($"WouldExceedBudget: Invalid new food calories ({newFoodCalories}) for user '{username}'");
                    throw new ArgumentException("New food calories cannot be negative.", nameof(newFoodCalories));
                }

                bool wouldExceed = _userDomain.WouldExceedBudget(username, burnedToday, currentTotal, newFoodCalories);

             
                FileLogger.LogInfo($"TrackerService: Budget check completed for '{username}': {(wouldExceed ? "WOULD EXCEED" : "Within limit")}");

                return wouldExceed;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"TrackerService: Error checking budget for '{username}'", ex);
                throw;
            }
        }

        public string GetDailyProgressStatus(string username, int consumed)
        {
            FileLogger.LogDebug($"TrackerService: Getting daily progress status for '{username}' (Consumed: {consumed})");

            try
            {
                if (username == null)
                {
                    FileLogger.LogWarning("GetDailyProgressStatus called with null username");
                    throw new ArgumentNullException(nameof(username), "Username cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    FileLogger.LogWarning("GetDailyProgressStatus called with empty username");
                    throw new ArgumentException("Username cannot be empty.", nameof(username));
                }

                if (consumed < 0)
                {
                    FileLogger.LogWarning($"GetDailyProgressStatus: Invalid consumed calories ({consumed}) for user '{username}'");
                    throw new ArgumentException("Consumed calories cannot be negative.", nameof(consumed));
                }

                string status = _userDomain.GetDailyProgressStatus(username, consumed);

                FileLogger.LogInfo($" TrackerService: Daily progress status retrieved for '{username}'");

                return status;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"TrackerService: Error getting daily progress for '{username}'", ex);
                throw;
            }
        }
    }
}