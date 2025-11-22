using System;

namespace FitnessApp.BusinessLoic.Interfaces
{
    public interface ITrackerService
    {
        int CalculateRemainingCalories(int dailyGoal, int consumed, int burned);
        bool WouldExceedBudget(int dailyGoal, int currentTotal, int newFoodCalories);
        string GetDailyProgressStatus(int dailyGoal, int consumed);
    }
}
