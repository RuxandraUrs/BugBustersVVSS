using System;
using FitnessApp.BusinessLogic.Interfaces;
using FitnessApp.BusinessLogic.Domains;

namespace FitnessApp.BusinessLogic.Services
{
    public class TrackerService : ITrackerService
    {
        private UserDomain _userDomain { get; set; }

        public TrackerService(UserDomain userDomain)
        {
            _userDomain = userDomain;
        }

        public int CalculateRemainingCalories(string username, int consumed, int burned)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            return _userDomain.CalculateRemainingCalories(username, consumed, burned);
        }
        public bool WouldExceedBudget(string username, int burnedToday,int currentTotal, int newFoodCalories)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            return _userDomain.WouldExceedBudget(username, burnedToday, currentTotal, newFoodCalories);
        }

        public string GetDailyProgressStatus(string username, int consumed)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            return _userDomain.GetDailyProgressStatus(username, consumed);
        }
    }
}