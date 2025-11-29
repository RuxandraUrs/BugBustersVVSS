using FitnessApp.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.IntegrationTests
{
    public class DatabaseTests
    {
        private string _testConnectionString;
        private DataAccess.DbConnection _dbConnection;

        [OneTimeSetUp] 
        public void GlobalSetup()
        {
            _testConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"); //begging of connection string in .env file

            if (string.IsNullOrEmpty(_testConnectionString))
            {
                Assert.Inconclusive("DB_CONNECTION_STRING is not set!");
            }
        }
  
        [Test]
        public void Can_Open_And_Close_Connection()
        {
            using (var conn = _dbConnection.GetConnection())
            {
                Assert.DoesNotThrow(() => conn.Open());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            }
        }
    }
}
