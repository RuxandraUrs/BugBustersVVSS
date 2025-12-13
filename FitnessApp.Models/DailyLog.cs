using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    public class DailyLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LogDate { get; set; }

        public List<Meal> Meals { get; set; }
        public List<Workout> Workouts { get; set; }

        public int TotalCaloriesConsumed { get; private set; }
        public int TotalCaloriesBurned { get; private set; }
        public int NetCalories { get; private set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        public DailyLog()
        {
            Meals = new List<Meal>();
            Workouts = new List<Workout>();
            CreatedAt = DateTime.Now;
        }

        public DailyLog(int userId, DateTime logDate) : this()
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive.", nameof(userId));

            if (logDate.Date > DateTime.Today)
                throw new ArgumentException("Log date cannot be in the future.", nameof(logDate));

            UserId = userId;
            LogDate = logDate.Date;
        }

        public void CalculateTotals()
        {
            TotalCaloriesConsumed = 0;
            TotalCaloriesBurned = 0;

            if (Meals != null)
            {
                foreach (var meal in Meals)
                {
                    TotalCaloriesConsumed += (int)meal.Calories;
                }
            }

            if (Workouts != null)
            {
                foreach (var workout in Workouts)
                {
                    TotalCaloriesBurned += workout.CaloriesBurned;
                }
            }

            NetCalories = TotalCaloriesConsumed - TotalCaloriesBurned;
            LastModifiedAt = DateTime.Now;
        }

        public void AddMeal(Meal meal)
        {
            if (meal == null)
                throw new ArgumentNullException(nameof(meal), "Meal cannot be null.");

            Meals.Add(meal);
            CalculateTotals();
        }

        public void AddWorkout(Workout workout)
        {
            if (workout == null)
                throw new ArgumentNullException(nameof(workout), "Workout cannot be null.");

            Workouts.Add(workout);
            CalculateTotals();
        }

        public bool RemoveMeal(int mealId)
        {
            var meal = Meals.Find(m => m.Id == mealId);
            if (meal != null)
            {
                bool removed = Meals.Remove(meal);
                if (removed)
                {
                    CalculateTotals();
                }
                return removed;
            }
            return false;
        }


        public bool RemoveWorkout(int workoutId)
        {
            var workout = Workouts.Find(w => w.Id == workoutId);
            if (workout != null)
            {
                bool removed = Workouts.Remove(workout);
                if (removed)
                {
                    CalculateTotals();
                }
                return removed;
            }
            return false;
        }

        public List<Meal> GetMealsByType(Models.Enums.MealType mealType)
        {
            return Meals.FindAll(m => m.Type == mealType);
        }

        /// <summary>
        /// Gets total calories for a specific meal type
        /// </summary>
        public double GetCaloriesByMealType(Models.Enums.MealType mealType)
        {
            var mealsOfType = GetMealsByType(mealType);
            double total = 0;
            foreach (var meal in mealsOfType)
            {
                total += meal.Calories;
            }
            return total;
        }

        public bool IsComplete()
        {
            return Meals != null && Meals.Count > 0;
        }
        public string GetSummary()
        {
            return $"Date: {LogDate:yyyy-MM-dd} | Meals: {Meals.Count} | Workouts: {Workouts.Count} | " +
                   $"Consumed: {TotalCaloriesConsumed} cal | Burned: {TotalCaloriesBurned} cal | " +
                   $"Net: {NetCalories} cal";
        }

        public override string ToString()
        {
            return GetSummary();
        }

    }
}
