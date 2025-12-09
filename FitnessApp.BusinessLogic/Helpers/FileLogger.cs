using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BusinessLogic.Helpers
{
    public static class FileLogger
    {
        private static readonly string filePath = "application_log.txt";

        public static void LogError(string message, Exception ex = null)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine($"Date and Time : {DateTime.Now}");
                    writer.WriteLine($"Message      : {message}");

                    if (ex != null)
                    {
                        writer.WriteLine($"Error type  :{ex.GetType().Name}");
                        writer.WriteLine($"Details     :{ex.Message}");
                        writer.WriteLine($"Stack Trace : {ex.StackTrace}");
                    }
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }
    }
}
