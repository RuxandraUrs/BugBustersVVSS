using System;
using System.IO;

namespace FitnessApp.BusinessLogic.Helpers
{
    public static class FileLogger
    {
        private static readonly string LogDirectory = "Logs";
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "application_log.txt");
        private static readonly object LockObject = new object();

        static FileLogger()
        {
           if (!Directory.Exists(LogDirectory))
           {
              Directory.CreateDirectory(LogDirectory);
           }
           
        }

        public static void LogInfo(string message)
        {
            WriteLog("INFO", message, null);
        }

        public static void LogWarning(string message)
        {
            WriteLog("WARNING", message, null);
        }

        public static void LogError(string message, Exception ex = null)
        {
            WriteLog("ERROR", message, ex);
        }

        public static void LogDebug(string message)
        {
            WriteLog("DEBUG", message, null);

        }

        private static void WriteLog(string level, string message, Exception ex)
        {
            try
            {
                lock (LockObject)
                {
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine("========================================================================");
                        writer.WriteLine($"[{level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine($"Message: {message}");

                        if (ex != null)
                        {
                            writer.WriteLine($"Exception Type: {ex.GetType().Name}");
                            writer.WriteLine($"Exception Message: {ex.Message}");
                            writer.WriteLine($"Stack Trace:\n{ex.StackTrace}");

                            if (ex.InnerException != null)
                            {
                                writer.WriteLine($"\nInner Exception: {ex.InnerException.GetType().Name}");
                                writer.WriteLine($"Inner Message: {ex.InnerException.Message}");
                            }
                        }

                        writer.WriteLine("========================================================================");
                        writer.WriteLine();
                    }
                }
            }
            catch
            {
                
            }
        }

        public static void ClearLogs()
        {
            try
            {
                lock (LockObject)
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch
            {
               
            }
        }

        public static string GetLogContents()
        {
            try
            {
                lock (LockObject)
                {
                    if (File.Exists(LogFilePath))
                    {
                        return File.ReadAllText(LogFilePath);
                    }
                }
            }
            catch
            {
                
            }

            return string.Empty;
        }
    }
}