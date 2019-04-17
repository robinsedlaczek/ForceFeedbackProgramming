using System;
using System.Diagnostics;
using System.IO;
using ForceFeedback.Core.adapters;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class Log_tests
    {
        private readonly string TEST_LOG_PATH = "logtest-" + Guid.NewGuid().ToString();
        
        
        [Fact]
        public void Append()
        {
            if (Directory.Exists(TEST_LOG_PATH)) Directory.Delete(TEST_LOG_PATH, true);
            Directory.CreateDirectory(TEST_LOG_PATH);

            var sut = new Log(TEST_LOG_PATH);
            sut.Append("foo");

            var result = File.ReadAllLines(Path.Combine(TEST_LOG_PATH,".forcefeedbackprogramming.log"));
            Assert.Equal(3, result.Length);
            Assert.Equal("foo", result[1]);
        }
        
        
        [Fact]
        public void Try_no_error() {
            if (Directory.Exists(TEST_LOG_PATH)) Directory.Delete(TEST_LOG_PATH, true);
            Directory.CreateDirectory(TEST_LOG_PATH);
    
            var sut = new Log(TEST_LOG_PATH);
            sut.Try(
                () => Console.WriteLine("No exception!"),
                null);
    
            Assert.False(File.Exists(Path.Combine(TEST_LOG_PATH,".forcefeedbackprogramming.log")));
        }
        
        
        [Fact]
        public void Try_with_error() {
            if (Directory.Exists(TEST_LOG_PATH)) Directory.Delete(TEST_LOG_PATH, true);
            Directory.CreateDirectory(TEST_LOG_PATH);

            var result = false;
            var sut = new Log(TEST_LOG_PATH);
            sut.Try(
                () => throw new ApplicationException(),
                () => result = true);
    
            Assert.True(result);
            var logText = File.ReadAllText(Path.Combine(TEST_LOG_PATH,".forcefeedbackprogramming.log"));
            Debug.WriteLine($"<<<LOG<<<\n{logText}>>>>>>");
            Assert.True(logText.IndexOf("ApplicationException") >  0);
        }

        
        [DebugFact()] // Run manually in debug mode; this is to avoid automatic global folder "pollution".
        public void Try_with_global_log_folder()
        {
            var GLOBAL_LOG_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logFilename = Path.Combine(GLOBAL_LOG_PATH, ".forcefeedbackprogramming.log");
            DeleteLogfile();

            try
            {
                var sut = new Log(GLOBAL_LOG_PATH);
                sut.Append("foo");

                var logText = File.ReadAllText(logFilename);
                Assert.True(logText.IndexOf("foo") > 0);
            }
            finally
            {
                DeleteLogfile();       
            }

            
            void DeleteLogfile() { if (File.Exists(logFilename)) File.Delete(logFilename); }
        }
    }
}
