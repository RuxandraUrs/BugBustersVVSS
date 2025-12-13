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
       // List<DailyLog> GetHistory(int userId, DateTime startDate, DateTime endDate);
    }
}
