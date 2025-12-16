using FitnessApp.BusinessLogic.Helpers;
using NUnit.Framework;
using System;
using System.IO;

namespace FitnessApp.Tests.UnitTests
{
    [TestFixture]
    public class FileLoggerTests
    {
        private string _logFilePath = Path.Combine("Logs", "application_log.txt");

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [Test]
        public void LogError_SimpleMessage_ShouldContainMessage()
        {
            string testMessage = "This is a test error";

            FileLogger.LogError(testMessage);

            Assert.IsTrue(File.Exists(_logFilePath), "Fisierul de log nu a fost creat in folderul Logs!");

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain(testMessage));
        }

        [Test]
        public void LogError_WithException_ShouldLogExceptionDetails()
        {
            var exception = new InvalidOperationException("Test exception");

            FileLogger.LogError("Error occurred", exception);

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain("InvalidOperationException"));
            Assert.That(content, Does.Contain("Test exception"));
        }

        [Test]
        public void LogError_WithException_ShouldLogStackTrace()
        {
            var exception = new InvalidOperationException("Test exception");

            FileLogger.LogError("Error occurred", exception);

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain("Stack Trace"));
        }

        [Test]
        public void LogError_MultipleCalls_ShouldAppendToFile()
        {
            FileLogger.LogError("First error");
            FileLogger.LogError("Second error");

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain("First error"));
            Assert.That(content, Does.Contain("Second error"));
        }

        [Test]
        public void LogError_ShouldIncludeTimestamp()
        {
            FileLogger.LogError("Test error");

            var content = File.ReadAllText(_logFilePath);

            string currentYear = DateTime.Now.Year.ToString();
            Assert.That(content, Does.Contain(currentYear));
        }

        [Test]
        public void LogError_NullMessage_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => FileLogger.LogError(null));
        }

        [Test]
        public void LogError_EmptyMessage_ShouldStillCreateFile()
        {
            FileLogger.LogError("");

            Assert.IsTrue(File.Exists(_logFilePath));
        }

        [Test]
        public void LogError_WithNestedExceptions_ShouldLogAll()
        {
            var innerException = new ArgumentException("Inner exception");
            var outerException = new InvalidOperationException("Outer exception", innerException);

            FileLogger.LogError("Error with nested exceptions", outerException);

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain("InvalidOperationException"));
            
            Assert.That(content, Does.Contain("Inner exception"));
        }

        [Test]
        public void LogError_LongMessage_ShouldHandleCorrectly()
        {
            var longMessage = new string('A', 1000);

            Assert.DoesNotThrow(() => FileLogger.LogError(longMessage));
            Assert.IsTrue(File.Exists(_logFilePath));
        }

        [Test]
        public void LogError_SpecialCharacters_ShouldHandleCorrectly()
        {
            string messageWithSpecialChars = "Error: Test@#$%^&*()[]{}";

            FileLogger.LogError(messageWithSpecialChars);

            var content = File.ReadAllText(_logFilePath);
            Assert.That(content, Does.Contain(messageWithSpecialChars));
        }
    }
}