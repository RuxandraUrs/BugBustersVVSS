using FitnessApp.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.IntegrationTests
{
    public class DatabaseTests
    {
        [Test]
        public void DbConnection_CanOpenConnection()
        {
            string connectionString = "Server=...";
            var dbConnection = new DbConnection(connectionString);

            using (var conn = dbConnection.GetConnection())
            {
                // Assert.DoesNotThrow(() => conn.Open());
                conn.Open();
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            }
        }
    }
}
