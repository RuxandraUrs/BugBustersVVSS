using System;

namespace FitnessApp.BusinessLogic.Interfaces
{
    public interface ITrackerService
    {
        int CalculateRemainingCalories(string username, int consumed, int burned);
        bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories);
        string GetDailyProgressStatus(string username, int consumed);
    }
}
