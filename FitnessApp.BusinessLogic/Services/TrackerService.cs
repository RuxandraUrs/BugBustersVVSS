using FitnessApp.BusinessLogic.Interfaces;
using System;

namespace FitnessApp.BusinessLogic.Services
{
    public class TrackerService : ITrackerService
    {
        public int CalculateRemainingCalories(int dailyGoal, int consumed, int burned)
        {
            return (dailyGoal + burned) - consumed;
        }
        public bool WouldExceedBudget(int dailyGoal, int burnedToday,int currentTotal, int newFoodCalories)
        {
            int limit = dailyGoal + burnedToday; 

            return (currentTotal + newFoodCalories) > limit;
        }

        public string GetDailyProgressStatus(int dailyGoal, int consumed)
        {
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
    }
}