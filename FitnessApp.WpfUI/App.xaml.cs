using DotNetEnv; 
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLoic.Helpers;
using FitnessApp.DataAccess;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.Models;
using System;
using System.Windows;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLogic.Helpers;

namespace FitnessApp.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Env.Load();
                string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if (string.IsNullOrEmpty(connectionString))
                {
                    FileLogger.LogError("DB_CONNECTION_STRING not found in .env file"); 
                    MessageBox.Show("Couldn't find DB_CONNECTION_STRING in the .env file!");
                    Shutdown();
                    return;
                }

                var dbConnection = new DbConnection(connectionString);
                var userRepo = new UserRepository(dbConnection);
                var userDomain = new UserDomain(userRepo);
                var activityRepo = new ActivityRepository(dbConnection);

                FileLogger.LogError("Application started successfully"); 
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Critical error at application startup", ex); 
                MessageBox.Show($"Critical error at startup: {ex.Message}");
            }
        }
    }

}
