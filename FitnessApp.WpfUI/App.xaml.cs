using DotNetEnv; 
using FitnessApp.BusinessLogic.Domains;
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
            FileLogger.LogInfo("=== APPLICATION STARTING ===");
            FileLogger.LogInfo($"Start Time: {DateTime.Now}");

            try
            {
                FileLogger.LogDebug("Loading environment variables from .env file...");
                Env.Load();

                string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if (string.IsNullOrEmpty(connectionString))
                {
                    FileLogger.LogError("DB_CONNECTION_STRING not found in .env file");
                    MessageBox.Show(
                        "Database connection string not found!\n\nPlease ensure DB_CONNECTION_STRING is set in the .env file.",
                        "Configuration Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    Shutdown();
                    return;
                }

                FileLogger.LogDebug("Connection string loaded successfully");
                FileLogger.LogDebug("Initializing database connection...");

                var dbConnection = new DbConnection(connectionString);

                FileLogger.LogDebug("Initializing repositories...");
                var userRepo = new UserRepository(dbConnection);
                var activityRepo = new ActivityRepository(dbConnection);

                FileLogger.LogDebug("Initializing domain layer...");
                var userDomain = new UserDomain(userRepo);

                FileLogger.LogInfo("All components initialized successfully");
                FileLogger.LogInfo("Application started successfully");
                FileLogger.LogInfo("=== APPLICATION READY ===");

                base.OnStartup(e);
            }
            catch (Exception ex)
            {

                FileLogger.LogError("CRITICAL: Application startup failed", ex);

                MessageBox.Show(
                    $"A critical error occurred during application startup:\n\n{ex.Message}\n\nThe application will now close.\n\nCheck application_log.txt for details.",
                    "Critical Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            FileLogger.LogInfo("=== APPLICATION SHUTTING DOWN ===");
            FileLogger.LogInfo($"Exit Time: {DateTime.Now}");
            FileLogger.LogInfo($"Exit Code: {e.ApplicationExitCode}");

            base.OnExit(e);
        }
    }
}


