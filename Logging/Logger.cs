using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PatternToolbox.Logging
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Warning,
        Error,
        CriticalError,
        CriticalMessage,
        Stub,
        Always
    }

    public interface IExternalLogger
    {
        void LogMessage(LogLevel level, string msg);
    }

    [Serializable]
    public class Logger
    {
        private static Logger _globalLogger = null;
        public static Logger GlobalLogger { get { return Logger._globalLogger; } }

        protected static readonly DateTime startTime = DateTime.Now;
        protected static IExternalLogger? ExternalLogger;

        [Serializable]
        public struct LoggerConfig
        {
            public bool ExceptionOnCriticalError;
            public bool AssertOnCriticalError;
            public bool LogToDiagnosticConsole;
            public bool LogToConsole;
            public bool AssertOnStub;
            public bool CriticalErrorOnNotImplemented;

            public bool FullStackTraceOnAllMessages;

            public bool PartialStackTraceOnAllMessages;
            public bool PartialStackTraceOnNotImplemented;

            public bool TimestampMessagesInMs;

            public LogLevel LowestDisplayableLogLevel;
            public bool SilenceAllLogging;
            //test
            public void SetDefaults()
            {
                ExceptionOnCriticalError = true;
                AssertOnCriticalError = true;
                LogToDiagnosticConsole = true;
                LogToConsole = true;
                AssertOnStub = true;
                CriticalErrorOnNotImplemented = true;

                FullStackTraceOnAllMessages = false;

                PartialStackTraceOnAllMessages = true;
                PartialStackTraceOnNotImplemented = true;

                TimestampMessagesInMs = true;

                LowestDisplayableLogLevel = LogLevel.Trace;
                SilenceAllLogging = false;
            }
        }


        public LoggerConfig Config = new();
        public string SystemName { get; private set; } = String.Empty;


        public static Logger CreateRootLogger()
        {
            LoggerConfig conf = new LoggerConfig();
            conf.SetDefaults();
            return CreateRootLogger(conf);
        }

        public static Logger CreateRootLogger(LoggerConfig config)
        {
            if (GlobalLogger != null)
                throw new Exception("Singleton object is being created more than once");

            Logger._globalLogger = new Logger("RootLogger", config, null);

            return Logger.GlobalLogger!;
        }
        public static Logger CreateLogger(string sytemName)
        {
            return new Logger(sytemName);
        }

        public static Logger CreateLogger<T>()
        {
            return new Logger(typeof(T).Name);
        }

        public static Logger CreateRootLogger(IExternalLogger externLogger, LoggerConfig config)
        {
            if (GlobalLogger != null)
                throw new Exception("Singleton object is being created more than once");

            Logger._globalLogger = new Logger("RootLogger", config, externLogger);

            return Logger.GlobalLogger!;
        }

        private Logger(string systemName, LoggerConfig config, IExternalLogger? externalLogger = null)
        {
            SystemName = systemName;
            Config = config;

            if(ExternalLogger == null)
                ExternalLogger = externalLogger;
        }




        public Logger(string systemName)
        {
            if (GlobalLogger == null)
            {
                CreateRootLogger();
                Logger.GlobalLogger!.Log(LogLevel.Warning, "GlobalLogger was not created manually, using defaults");
            }

            SystemName = systemName;
            Config = Logger.GlobalLogger.Config;
        }

        public Logger(string systemName, LoggerConfig config)
        {
            if (GlobalLogger != null)
                throw new Exception("Create the root logger first!");

            SystemName = systemName;
            Config = config;
        }

        public void SetConfig(LoggerConfig config)
        {
            Config = config;
        }

        public void LogNotImplementedStub(string message = "")
        {
            if (Config.CriticalErrorOnNotImplemented == false && ShouldLogMessage(LogLevel.Stub) == false)
                return;

            string msg = "Not Implemented: ";
            if(message != "")
                msg += "' " + message + " ' :";

            if (Config.PartialStackTraceOnNotImplemented == true && (Config.PartialStackTraceOnAllMessages == false && Config.FullStackTraceOnAllMessages == false))
            {
                StackTrace st = new(true);
                msg += string.Format(" {0} ", st.GetFrame(1));
                msg = msg.TrimEnd();
            }

            Log(LogLevel.Stub, msg);

            if (Config.CriticalErrorOnNotImplemented)
                Log(LogLevel.CriticalError, msg);
        }

        public bool ShouldLogMessage(LogLevel level)
        {
            return Config.SilenceAllLogging == false && level >= Config.LowestDisplayableLogLevel;
        }



        virtual public void Log(LogLevel level, string format, params object[] objs)
        {
            if (ShouldLogMessage(level) == false)
                return;

            string msg = String.Format(format, objs);

            msg = $"[ { SystemName,-20} ]\t{msg}";

            LogMessageInternal(level, msg);
        }

        protected void LogMessageInternal(LogLevel level, string message)
        {
            string msg = message;

            msg = $" [{level,-10}]\t{msg}";

            if (Config.TimestampMessagesInMs == true)
            {
                int ms = (int)(DateTime.Now - startTime).TotalMilliseconds;
                msg = $"[{ms, -4}ms]" + msg;
            }

            if (Config.FullStackTraceOnAllMessages)
            {
                msg += String.Format("\n StackTrace: {0}", Environment.StackTrace);
            }
            else if (Config.PartialStackTraceOnAllMessages)
            {
                System.Diagnostics.StackTrace st = new(true);
                msg += String.Format("\n           StackTrace: {0}", st.GetFrame(3));
            }

            if (Config.LogToConsole)
            {
                Console.WriteLine(msg);
            }

            if (Config.LogToDiagnosticConsole)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }

            if (ExternalLogger != null)
            {
                ExternalLogger.LogMessage(level, msg);
            }

            if (level == LogLevel.Stub)
            {
                if (Config.AssertOnStub == true)
                    System.Diagnostics.Debug.Assert(false, msg);
            }

            if (level == LogLevel.CriticalError)
            {
                if (Config.AssertOnCriticalError)
                    System.Diagnostics.Debug.Assert(false, msg);

                if (Config.ExceptionOnCriticalError)
                    throw new Exception(msg);
            }
        }
    }
}
