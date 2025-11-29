using FitnessApp.DataAccess.Interfaces;
using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnection _dbConnection;

        // Constructor Injection
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
                HeightCm = reader.GetDouble(reader.GetOrdinal("HeightCm")),
                WeightKg = reader.GetDouble(reader.GetOrdinal("WeightKg")),
                DailyCalorieGoal = reader.GetInt32(reader.GetOrdinal("DailyCalorieGoal"))
            }
        }

        public void AddUser(User user) //should add a parameter for passwordhash
        {
            using (var conn = _dbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_RegisterUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash); //it should be the parameter of the function(hashpass)
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value); 
                    cmd.Parameters.AddWithValue("@BirthDate", user.BirthDate);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender.ToString());
                    cmd.Parameters.AddWithValue("@HeightCm", user.HeightCm);
                    cmd.Parameters.AddWithValue("@WeightKg", user.WeightKg);
                    cmd.Parameters.AddWithValue("@DailyCalorieGoal", user.DailyCalorieGoal);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUserByUsername(string username)
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
                            return MapUserFromReader(reader);
                    }
                }
            }

            return null; //user not found
        }
    }
}
