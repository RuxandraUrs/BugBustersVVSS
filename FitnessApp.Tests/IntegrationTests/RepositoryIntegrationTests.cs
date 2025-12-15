using FitnessApp.BusinessLogic.Domains;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.Models;
using FitnessApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MyDbConnection = FitnessApp.DataAccess.DbConnection;

namespace FitnessApp.Tests.IntegrationTests
{
    [TestFixture]
    public class RepositoryIntegrationTests
    {
        private string _connectionString;
        private MyDbConnection _dbConnection;
        private UserRepository _userRepo;
        private ActivityRepository _activityRepo;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

            if (File.Exists(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length != 2)
                        continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('"');

                    Environment.SetEnvironmentVariable(key, value);

                }
            }

            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("CRITICAL: Did not find DB_CONNECTION_STRING. Check if .env is set to 'Copy to Output Directory'");
            }
        }

        [SetUp]
        public void Setup()
        {
            _dbConnection = new MyDbConnection(_connectionString);
            _userRepo = new UserRepository(_dbConnection);
            _activityRepo = new ActivityRepository(_dbConnection);
        }

        public void TearDown()
        {
            _dbConnection = null;
            _userRepo = null;
            _activityRepo = null;
        }

        [Test]
        public void AddUser_ValidUser_ShouldInsertIntoDatabase()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User
                {
                    Username = "TestIntegrationUser",
                    PasswordHash = "hash123",
                    Email = "test@integration.com",
                    BirthDate = new DateTime(1995, 5, 20),
                    Gender = "Male",
                    HeightCm = 180,
                    WeightKg = 80,
                    DailyCalorieGoal = 2000
                };

                _userRepo.AddUser(user);
                var retrievedUser = _userRepo.GetUserByUsername("TestIntegrationUser");

                Assert.IsNotNull(retrievedUser, "User was not found in DB after insert!");
                Assert.AreEqual("test@integration.com", retrievedUser.Email);
                Assert.AreEqual(180, retrievedUser.HeightCm);
            }
        }

        [Test]
        public void AddUser_DuplicateUsername_ShouldThrowSqlException()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User { Username = "UniqueUser", PasswordHash = "pass", BirthDate = DateTime.Now, HeightCm = 170, WeightKg = 70, DailyCalorieGoal = 2000 };

                _userRepo.AddUser(user);

                Assert.Throws<SqlException>(() => _userRepo.AddUser(user));
            }
        }

        [Test]
        public void AddMeal_ShouldCreateDailyLogAndMeal()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User 
                {
                    Username = "MealTester",
                    PasswordHash = "pass",
                    BirthDate = DateTime.Now,
                    Gender = "Male",
                    HeightCm = 170,
                    WeightKg = 70,
                    DailyCalorieGoal = 2000
                };
                _userRepo.AddUser(user);
                var dbUser = _userRepo.GetUserByUsername("MealTester");

                var meal = new Meal
                {
                    FoodName = "Test Apple",
                    Calories = 95,
                    QuantityGrams = 150,
                    Type = MealType.Snack
                };

                _activityRepo.AddMeal(meal, dbUser.Id, DateTime.Now);

                var log = _activityRepo.GetDailyLog(dbUser.Id, DateTime.Now);

                //Assert.IsNotNull(log, "DailyLog was not created automatically");
                Assert.AreEqual(1, log.Meals.Count, "Meal was not found in Daily Log");
                Assert.AreEqual("Test Apple", log.Meals[0].FoodName);
                Assert.AreEqual(95, log.Meals[0].Calories);
            }
        }

        [Test]
        public void GetDailyLog_CalculatesTotalsCorrectly()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User { Username = "CalcTester", PasswordHash = "p", BirthDate = DateTime.Now, HeightCm = 180, WeightKg = 80, DailyCalorieGoal = 2000 };
                _userRepo.AddUser(user);
                var dbUser = _userRepo.GetUserByUsername("CalcTester");

                _activityRepo.AddMeal(new Meal { FoodName = "Big Lunch", Calories = 500, QuantityGrams = 200, Type = MealType.Lunch }, dbUser.Id, DateTime.Now);
                _activityRepo.AddWorkout(new Workout { ActivityName = "Running", DurationMinutes = 30, CaloriesBurned = 200 }, dbUser.Id, DateTime.Now);

                var log = _activityRepo.GetDailyLog(dbUser.Id, DateTime.Now);

                Assert.AreEqual(500, log.TotalCaloriesConsumed);
                Assert.AreEqual(200, log.TotalCaloriesBurned);
                Assert.AreEqual(300, log.NetCalories);
            }
        }

        [Test]
        public void UpdateUserStats_ShouldPersistChanges()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User { Username = "Updater", PasswordHash = "p", BirthDate = DateTime.Now, HeightCm = 180, WeightKg = 80, DailyCalorieGoal = 2000 };
                _userRepo.AddUser(user);
                var dbUser = _userRepo.GetUserByUsername("Updater");

                dbUser.WeightKg = 85;
                dbUser.DailyCalorieGoal = 1500;

                _userRepo.UpdateUserStats(dbUser);

                var updatedUser = _userRepo.GetUserByUsername("Updater");

                Assert.AreEqual(85, updatedUser.WeightKg);
                Assert.AreEqual(1500, updatedUser.DailyCalorieGoal);
            }
        }

        [Test]
        public void DeleteUser_ShouldRemoveUserFromDb()
        {
            using (var scope = new TransactionScope())
            {
                var user = new User { Username = "Deletable", PasswordHash = "p", BirthDate = DateTime.Now, HeightCm = 180, WeightKg = 80, DailyCalorieGoal = 2000 };
                _userRepo.AddUser(user);
                var dbUser = _userRepo.GetUserByUsername("Deletable");

                _userRepo.DeleteUser(dbUser.Id);

                var deletedUser = _userRepo.GetUserByUsername("Deletable");
                Assert.IsNull(deletedUser);
            }
        }
    }
}
