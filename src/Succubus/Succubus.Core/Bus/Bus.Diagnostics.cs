using System;
using System.Diagnostics;
using System.IO;

namespace Succubus.Core
{
    public partial class Bus
    {

        private ulong sentMessages = 0;
        private ulong receivedMessages = 0;

        public Diagnose GetDiagnose()
        {
            lock (this)
                lock (synchronizationContexts)
                {
                    return new Diagnose()
                    {
                        NumberOfItemsForTimeout = timeoutHandler.NumberOfItemsForTimeout(),
                        SynchronizationContexts = synchronizationContexts.Count,
                        SentMessages = sentMessages,
                        ReceivedMessages = receivedMessages
                    };
                }
        }

        public string Name
        {
            get;
            set;
        }


        public TextWriter LogWriter
        {
            get;
            set;
        }

        public LogLevel LogLevel { get; set; }

        
        void Log(LogLevel level, string message, params object[] p) {
            if (LogWriter == null || LogLevel == LogLevel.None || level > LogLevel) return;
            LogWriter.WriteLine("[{0}] {2}: {1}", level, String.Format(message, p), Name);
            LogWriter.Flush();
        }

        void Info(string message, params object[] p)
        {
            Log(LogLevel.Info, message, p);
        }

        void Warning(string message, params object[] p)
        {
            Log(LogLevel.Warning, message, p);
        }

        void Error(string message, params object[] p)
        {
            Log(LogLevel.Error, message, p);
        }

        void Fatal(string message, params object[] p)
        {
            Log(LogLevel.Fatal, message, p);
        }

        [Conditional("DEBUG")]
        void Trace(string message, params object[] p)
        {
            Log(LogLevel.Trace, message, p);
        }
    }
  
    public enum LogLevel
    {
        Fatal,
        Error,
        Warning,
        Info,
        Trace,
        None
    }
}