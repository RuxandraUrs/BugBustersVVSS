using FitnessApp.BusinessLogic.Helpers;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FitnessApp.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnection _dbConnection;

        public UserRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                Gender = reader.GetString(reader.GetOrdinal("Gender")),
                HeightCm = reader.GetDouble(reader.GetOrdinal("HeightCm")),
                WeightKg = reader.GetDouble(reader.GetOrdinal("WeightKg")),
                TargetWeightKg = reader.GetDouble(reader.GetOrdinal("TargetWeightKg")),
                DailyCalorieGoal = reader.GetInt32(reader.GetOrdinal("DailyCalorieGoal"))
            };
        }

        public void AddUser(User user)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_RegisterUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                        cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BirthDate", user.BirthDate);
                        cmd.Parameters.AddWithValue("@Gender", user.Gender.ToString());
                        cmd.Parameters.AddWithValue("@HeightCm", user.HeightCm);
                        cmd.Parameters.AddWithValue("@WeightKg", user.WeightKg);
                        cmd.Parameters.AddWithValue("@TargetWeightKg", user.TargetWeightKg);
                        cmd.Parameters.AddWithValue("@DailyCalorieGoal", user.DailyCalorieGoal);

                        cmd.ExecuteNonQuery();
                    }
                }

                FileLogger.LogInfo($"User added to database: {user.Username}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error adding user to database: {user?.Username}", ex);
                throw;
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_Login", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = MapUserFromReader(reader);

                                FileLogger.LogInfo($"User retrieved from database: {username}");
                                return user;
                            }
                        }
                    }
                }


                FileLogger.LogWarning($"User not found in database: {username}");
                return null;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error retrieving user from database: {username}", ex);
                throw;
            }
        }

        public void UpdateUserStats(User user)
        {

            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_UpdateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", user.Id);
                        cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@HeightCm", user.HeightCm);
                        cmd.Parameters.AddWithValue("@WeightKg", user.WeightKg);
                        cmd.Parameters.AddWithValue("@TargetWeightKg", user.TargetWeightKg);
                        cmd.Parameters.AddWithValue("@DailyCalorieGoal", user.DailyCalorieGoal);

                        cmd.ExecuteNonQuery();
                    }
                }
                FileLogger.LogInfo($"User stats updated for ID: {user.Id}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error updating stats for user ID: {user.Id}", ex);
                throw;
            }
        }

        public void DeleteUser(int userId)
        {

            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", userId);

                        cmd.ExecuteNonQuery();
                    }
                }
                FileLogger.LogInfo($"User deleted successfully. ID: {userId}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"Error deleting user with ID: {userId}", ex);
                throw;
            }
        }
    }
}