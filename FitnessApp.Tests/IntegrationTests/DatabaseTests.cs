using FitnessApp.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDbConnection = FitnessApp.DataAccess.DbConnection;

namespace FitnessApp.Tests.IntegrationTests
{
    public class DatabaseTests
    {
        private string _testConnectionString;
        private MyDbConnection _dbConnection;

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
                    // Curățăm spațiile și eventualele ghilimele rămase
                    var value = parts[1].Trim().Trim('"');

                    Environment.SetEnvironmentVariable(key, value);
                }
            }

            _testConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(_testConnectionString))
            {
                Assert.Inconclusive("DB_CONNECTION_STRING is not set!");
            }

            _dbConnection = new MyDbConnection(_testConnectionString);
        }
  
        [Test]
        public void Can_Open_And_Close_Connection()
        {
            Assert.IsNotNull(_dbConnection, "_dbConnection object was not initialized.");

            using (var conn = _dbConnection.GetConnection())
            {
                Assert.IsNotNull(conn, "GetConnection returned null.");

                Assert.DoesNotThrow(() => conn.Open());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            }
        }
    }
}
