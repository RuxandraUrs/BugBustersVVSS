using FitnessApp.BusinessLogic.Helpers;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
                Id = (int)reader["Id"],
                LogId = (int)reader["LogId"],
                FoodName = reader["FoodName"].ToString(),
                Calories = Convert.ToDouble(reader["TotalCalories"]),
                QuantityGrams = Convert.ToDouble(reader["QuantityGrams"]),
                Type = (MealType)Enum.Parse(typeof(MealType), reader["MealType"].ToString())
            };
        }

        private Workout MapWorkoutFromReader(SqlDataReader reader)
        {
            return new Workout
            {
                Id = (int)reader["Id"],
                LogId = (int)reader["LogId"],
                ActivityName = reader["ActivityName"].ToString(),
                DurationMinutes = (int)Convert.ToDouble(reader["DurationMinutes"]),
                CaloriesBurned = Convert.ToInt32(reader["CaloriesBurned"])
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

                FileLogger.LogInfo($"Meal added to database: {meal.FoodName} for user {userId}");
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

                FileLogger.LogInfo($"Workout added to database: {workout.ActivityName} for user {userId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error adding workout to database for user {userId}", ex);
                throw;
            }
        }

        public Meal GetMealById(int mealId)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT * FROM Meals WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", mealId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapMealFromReader(reader);
                            }
                        }
                    }
                }

                FileLogger.LogWarning($"Meal with ID {mealId} not found.");
                return null;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at finding the meal with ID: {mealId}", ex);
                throw;
            }
        }

        public Workout GetWorkoutById(int workoutId)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT * FROM Workouts WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", workoutId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapWorkoutFromReader(reader);
                            }
                        }
                    }
                }

                FileLogger.LogWarning($"Workout with ID {workoutId} not found.");
                return null;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at finding the workout with ID: {workoutId}", ex);
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

                    using (var cmd = new SqlCommand("sp_GetMealsByDate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

                    using (var cmd = new SqlCommand("sp_GetWorkoutsByDate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

                FileLogger.LogInfo($"DailyLog retrieved for User {userId} on {date:d}. Items: {dailyLog.Meals.Count + dailyLog.Workouts.Count}");
                return dailyLog;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error retrieving DailyLog for user {userId} on {date}", ex);
                throw;
            }
        }

        public List<DailyHistory> GetHistory(int userId, DateTime startDate, DateTime endDate)
        {
            var historyList = new List<DailyHistory>();
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_GetHistory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new DailyHistory
                                {
                                    LogDate = reader.GetDateTime(reader.GetOrdinal("LogDate")),
                                    TotalCaloriesIn = reader.GetDouble(reader.GetOrdinal("TotalCaloriesIn")),
                                    TotalCaloriesBurned = reader.GetDouble(reader.GetOrdinal("TotalCaloriesBurned")),
                                    CalorieDifference = reader.GetDouble(reader.GetOrdinal("CalorieDifference"))
                                };
                                historyList.Add(item);
                            }
                        }
                    }
                }

                FileLogger.LogInfo($"History retrieved for user {userId}. Count: {historyList.Count}");
                return historyList;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error getting history for user {userId} between {startDate} and {endDate}", ex);
                throw;
            }
        }

        public void UpdateMeal(Meal meal)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_UpdateMeal", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", meal.Id);
                        cmd.Parameters.AddWithValue("@FoodName", meal.FoodName);
                        cmd.Parameters.AddWithValue("@Quantity", meal.QuantityGrams);
                        cmd.Parameters.AddWithValue("@Calories", meal.Calories);
                        cmd.Parameters.AddWithValue("@MealType", meal.Type.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                FileLogger.LogInfo($"Meal updated: ID {meal.Id}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at updating the meal with ID: {meal.Id}", ex);
                throw;
            }
        }

        public void UpdateWorkout(Workout workout)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_UpdateWorkout", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", workout.Id);
                        cmd.Parameters.AddWithValue("@ActivityName", workout.ActivityName);
                        cmd.Parameters.AddWithValue("@DurationMinutes", workout.DurationMinutes);
                        cmd.Parameters.AddWithValue("@CaloriesBurned", workout.CaloriesBurned);

                        cmd.ExecuteNonQuery();
                    }
                }

                FileLogger.LogInfo($"Workout updated: ID {workout.Id}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at updating workout with ID: {workout.Id}", ex);
                throw;
            }
        }

        public void DeleteMeal(int mealId)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("DELETE FROM Meals WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", mealId);
                        cmd.ExecuteNonQuery();
                    }
                }

                FileLogger.LogInfo($"Meal deleted: ID {mealId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at deleting meal with ID: {mealId}", ex);
                throw;
            }
        }

        public void DeleteWorkout(int workoutId)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("DELETE FROM Workouts WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", workoutId);
                        cmd.ExecuteNonQuery();
                    }
                }

                FileLogger.LogInfo($"Workout deleted: ID {workoutId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error at deleting workout with ID: {workoutId}", ex);
                throw;
            }
        }
    }
}