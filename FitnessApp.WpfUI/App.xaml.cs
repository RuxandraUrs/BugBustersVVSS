using System;
using System.Windows;
using DotNetEnv; 
using FitnessApp.DataAccess;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.BusinessLogic.Domains;

namespace FitnessApp.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Env.Load();
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Couldn’t find DB_CONNECTION_STRING in the .env file!");
                Shutdown(); 
                return;
            }

            try
            {
                var dbConnection = new DbConnection(connectionString);
                var userRepo = new UserRepository(dbConnection);
                var userDomain = new UserDomain(userRepo);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error at startup: {ex.Message}");
            }
        }
    }

}
