using System;
using System.IO;

namespace ForceFeedback.Core.adapters
{
    public class Log
    {
        private readonly string _logPath;
        
        public Log() : this(Environment.GetFolderPath(Environment.SpecialFolder.Personal)) {}
        public Log(string logPath) => _logPath = logPath;
        
        
        public void Try(Action run, Action onFailure)
        {
            try
            {
                run();
            }
            catch (Exception ex)
            {
                Append(ex.Message);
                onFailure();
            }
        }

        public void Append(string message)
        {
            var logFilePath = Path.Combine(_logPath, ".forcefeedbackprogramming.log");
            var entryText = new[] {DateTime.Now.ToString(), message, ""};
            File.AppendAllLines(logFilePath, entryText);
        }
    }
}
