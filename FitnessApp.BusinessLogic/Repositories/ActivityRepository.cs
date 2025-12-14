using FitnessApp.BusinessLogic.Helpers;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DataAccess.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly DbConnection _dbConnection;

        public ActivityRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private Meal MapMealFromReader(SqlDataReader reader)
        {
            return new Meal
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                LogId = reader.GetInt32(reader.GetOrdinal("LogId")),
                FoodName = reader.GetString(reader.GetOrdinal("FoodName")), 

                Calories = reader.GetDouble(reader.GetOrdinal("TotalCalories")), 
                QuantityGrams = reader.GetDouble(reader.GetOrdinal("QuantityGrams")), 

                Type = (MealType)Enum.Parse(typeof(MealType), reader.GetString(reader.GetOrdinal("MealType")))
            };
        }

        private Workout MapWorkoutFromReader(SqlDataReader reader)
        {
            return new Workout
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ActivityName = reader.GetString(reader.GetOrdinal("ActivityName")),
                DurationMinutes = (int)reader.GetDouble(reader.GetOrdinal("DurationMinutes")),
                CaloriesBurned = (int)reader.GetDouble(reader.GetOrdinal("CaloriesBurned")) // Cast la int daca in DB e float/decimal
            };
        }
        public void AddMeal(Meal meal, int userId, DateTime logDate)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_AddMeal", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LogDate", logDate.Date);
                        cmd.Parameters.AddWithValue("@FoodName", meal.FoodName);
                        cmd.Parameters.AddWithValue("@QuantityGrams", meal.QuantityGrams);
                        cmd.Parameters.AddWithValue("@TotalCalories", meal.Calories);
                        cmd.Parameters.AddWithValue("@MealType", meal.Type.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }
                FileLogger.LogError($"Meal added to database: {meal.FoodName} for user {userId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error adding meal to database for user {userId}", ex);
                throw;
            }
        }

        public void AddWorkout(Workout workout, int userId, DateTime logDate)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_AddWorkout", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LogDate", logDate.Date);
                        cmd.Parameters.AddWithValue("@ActivityName", workout.ActivityName);
                        cmd.Parameters.AddWithValue("@DurationMinutes", workout.DurationMinutes);
                        cmd.Parameters.AddWithValue("@CaloriesBurned", workout.CaloriesBurned);

                        cmd.ExecuteNonQuery();
                    }
                }
                FileLogger.LogError($"Workout added to database: {workout.ActivityName} for user {userId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error adding workout to database for user {userId}", ex);
                throw;
            }
        }

        public DailyLog GetDailyLog(int userId, DateTime date)
        {
            var dailyLog = new DailyLog(userId, date);

            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();

                    string sqlMeals = "SELECT * FROM Meals WHERE UserId = @UserId AND CAST(LogDate AS DATE) = @LogDate"; //To Do: stored procedure

                    using (var cmd = new SqlCommand(sqlMeals, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LogDate", date.Date);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dailyLog.Meals.Add(MapMealFromReader(reader));
                            }
                        }
                    }

                    string sqlWorkouts = "SELECT * FROM Workouts WHERE UserId = @UserId AND CAST(LogDate AS DATE) = @LogDate"; //To Do: stored procedure

                    using (var cmd = new SqlCommand(sqlWorkouts, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LogDate", date.Date);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dailyLog.Workouts.Add(MapWorkoutFromReader(reader));
                            }
                        }
                    }
                }
                dailyLog.CalculateTotals();
                return dailyLog;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error retrieving DailyLog for user {userId} on {date}", ex);
                throw;
            }
        }

        //TO DO: GetHistory function!
        //public List<DailyLog> GetHistory(int userId, DateTime startDate, DateTime endDate)
        //{
        //    var historyList = new List<DailyHistoryItem>(); //TO DO: DailyHistoryItem

        //    try
        //    {
        //        using (var conn = _dbConnection.GetConnection())
        //        {
        //            conn.Open();
                  
        //            using (var cmd = new SqlCommand("sp_GetHistory", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

                       
        //                cmd.Parameters.AddWithValue("@UserId", userId);
        //                cmd.Parameters.AddWithValue("@StartDate", startDate);
        //                cmd.Parameters.AddWithValue("@EndDate", endDate);

        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var item = new DailyHistoryItem
        //                        {
        //                            LogDate = reader.GetDateTime(reader.GetOrdinal("LogDate")),

        //                            TotalCaloriesIn = reader.GetDouble(reader.GetOrdinal("TotalCaloriesIn")),
        //                            TotalCaloriesBurned = reader.GetDouble(reader.GetOrdinal("TotalCaloriesBurned")),

        //                            CalorieDifference = reader.GetDouble(reader.GetOrdinal("CalorieDifference"))
        //                        };

        //                        historyList.Add(item);
        //                    }
        //                }
        //            }
        //        }

        //        return historyList;
        //    }
        //    catch (Exception ex)
        //    {
        //        FileLogger.LogError($"Error getting history for user {userId} between {startDate} and {endDate}", ex);
        //        throw;
        //    }
        //}
    }
}
