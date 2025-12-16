using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DataAccess.Interfaces
{
    public interface IActivityRepository
    {
        void AddMeal(Meal meal, int userId, DateTime logDate);

        void AddWorkout(Workout workout, int userId, DateTime logDate);


        Meal GetMealById(int mealId);
        Workout GetWorkoutById(int workoutId);
        DailyLog GetDailyLog(int userId, DateTime date);
        List<DailyHistory> GetHistory(int userId, DateTime startDate, DateTime endDate);

        void UpdateMeal(Meal meal);
        void UpdateWorkout(Workout workout);

        void DeleteMeal(int mealId);
        void DeleteWorkout(int workoutId);
    }
}