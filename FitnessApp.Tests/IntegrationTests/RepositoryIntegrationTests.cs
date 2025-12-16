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
using static System.Formats.Asn1.AsnWriter;
using MyDbConnection = FitnessApp.DataAccess.DbConnection;

namespace FitnessApp.Tests.IntegrationTests
{
    [TestFixture]
    public class RepositoryIntegrationTests
    {
        private string _connectionString;
        private MyDbConnection? _dbConnection;
        private UserRepository? _userRepo;
        private ActivityRepository? _activityRepo;
        private TransactionScope? _scope;

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
            _scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
            _dbConnection = new MyDbConnection(_connectionString);
            _userRepo = new UserRepository(_dbConnection);
            _activityRepo = new ActivityRepository(_dbConnection);
        }

        [TearDown]
        public void TearDown()
        {
            _userRepo = null;
            _activityRepo = null;
            _dbConnection = null;
            _scope?.Dispose();
            _scope = null;
        }

        [Test]
        public void AddUser_ValidUser_ShouldInsertIntoDatabase()
        {
            var user = new User
            {
                Username = "TestIntegrationUser",
                PasswordHash = "paswordhash123",
                Email = "test1@gmail.com",
                BirthDate = new DateTime(1995, 5, 20),
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 80,
                TargetWeightKg = 68,
                DailyCalorieGoal = 2000
            };

            _userRepo.AddUser(user);
            var retrievedUser = _userRepo.GetUserByUsername("TestIntegrationUser");

            Assert.That(retrievedUser, Is.Not.Null, "User was not found in DB after insert!");
            Assert.Multiple(() =>
            {
                Assert.That(retrievedUser.Email, Is.EqualTo("test1@gmail.com"));
                Assert.That(retrievedUser.HeightCm, Is.EqualTo(180));
            });
        }

        [Test]
        public void AddUser_DuplicateUsername_ShouldThrowSqlException()
        {
            var user = new User
            {
                Username = "UniqueUser",
                PasswordHash = "pass",
                Email = "test2@gmail.com",
                BirthDate = new DateTime(2000, 11, 23),
                Gender = "Female",
                HeightCm = 170,
                WeightKg = 70,
                TargetWeightKg = 68,
                DailyCalorieGoal = 2000
            };

            _userRepo.AddUser(user);

            _ = Assert.Throws<SqlException>(() => _userRepo.AddUser(user));
        }

        [Test]
        public void AddMeal_ShouldCreateDailyLogAndMeal()
        {
            var user = new User
            {
                Username = "MealTester",
                PasswordHash = "pass",
                Email = "test3@gmail.com",
                BirthDate = new DateTime(2001, 06, 05),
                Gender = "Male",
                HeightCm = 170,
                WeightKg = 70,
                TargetWeightKg = 68,
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

            Assert.That(log.Meals, Has.Count.EqualTo(1), "Meal was not found in Daily Log");
            Assert.Multiple(() =>
            {
                Assert.That(log.Meals[0].FoodName, Is.EqualTo("Test Apple"));
                Assert.That(log.Meals[0].Calories, Is.EqualTo(95));
            });
        }

        [Test]
        public void GetDailyLog_CalculatesTotalsCorrectly()
        {
            var user = new User
            {
                Username = "CalcTester",
                PasswordHash = "passwordtest4",
                Email = "test4@gmail.com",
                BirthDate = new DateTime(1999, 05, 06),
                Gender = "Female",
                HeightCm = 180,
                WeightKg = 80,
                TargetWeightKg = 75,
                DailyCalorieGoal = 2000
            };
            _userRepo.AddUser(user);
            var dbUser = _userRepo.GetUserByUsername("CalcTester");

            _activityRepo.AddMeal(new Meal
            {
                FoodName = "Big Lunch",
                Calories = 500,
                QuantityGrams = 200,
                Type = MealType.Lunch
            }, dbUser.Id, DateTime.Now);

            _activityRepo.AddWorkout(new Workout
            {
                ActivityName = "Running",
                DurationMinutes = 30,
                CaloriesBurned = 200
            }, dbUser.Id, DateTime.Now);

            var log = _activityRepo.GetDailyLog(dbUser.Id, DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(log.TotalCaloriesConsumed, Is.EqualTo(500));
                Assert.That(log.TotalCaloriesBurned, Is.EqualTo(200));
                Assert.That(log.NetCalories, Is.EqualTo(300));
            });
        }

        [Test]
        public void UpdateUserStats_ShouldPersistChanges()
        {
            var user = new User
            {
                Username = "Updater",
                PasswordHash = "passwordtest5",
                Email = "test5@gmail.com",
                BirthDate = new DateTime(1999, 05, 06),
                Gender = "Female",
                HeightCm = 180,
                WeightKg = 80,
                DailyCalorieGoal = 2000
            };
            _userRepo.AddUser(user);
            var dbUser = _userRepo.GetUserByUsername("Updater");

            dbUser.WeightKg = 85;
            dbUser.DailyCalorieGoal = 1500;

            _userRepo.UpdateUserStats(dbUser);

            var updatedUser = _userRepo.GetUserByUsername("Updater");

            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.WeightKg, Is.EqualTo(85));
                Assert.That(updatedUser.DailyCalorieGoal, Is.EqualTo(1500));
            });
        }

        [Test]
        public void DeleteUser_ShouldRemoveUserFromDb()
        {
            var user = new User
            {
                Username = "Deletable",
                PasswordHash = "passwordtest6",
                Email = "test6@gmail.com",
                BirthDate = new DateTime(1995, 09, 16),
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 80,
                TargetWeightKg = 75,
                DailyCalorieGoal = 2000
            };
            _userRepo.AddUser(user);
            var dbUser = _userRepo.GetUserByUsername("Deletable");

            _userRepo.DeleteUser(dbUser.Id);

            var deletedUser = _userRepo.GetUserByUsername("Deletable");
            Assert.That(deletedUser, Is.Null);
        }
    }
}