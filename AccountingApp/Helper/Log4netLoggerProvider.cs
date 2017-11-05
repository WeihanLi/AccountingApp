using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using WeihanLi.Common.Helpers;

namespace AccountingApp.Helper
{
    /// <summary>
    /// Log4NetLoggerProvider
    /// learn more https://dotnetthoughts.net/how-to-use-log4net-with-aspnetcore-for-logging/
    /// </summary>
    public class Log4NetLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
            new ConcurrentDictionary<string, Log4NetLogger>();

        public Log4NetLoggerProvider(string log4NetConfigFile)
        {
            XmlConfigurator.ConfigureAndWatch(
                LogManager.CreateRepository(ConfigurationHelper.ApplicationName), 
                new FileInfo(ConfigurationHelper.MapPath(log4NetConfigFile))
                );
        }

        public void Dispose()
        {
            _loggers.Clear();            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new Log4NetLogger(categoryName));
        }
    }

    public class Log4NetLogger : ILogger
    {
        private readonly ILog _logger;

        public Log4NetLogger(string name)
        {
            _logger = LogManager.GetLogger(ConfigurationHelper.ApplicationName, name);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            string message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _logger.Fatal(message);
                        break;

                    case LogLevel.Debug:
                    case LogLevel.Trace:
                        _logger.Debug(message);
                        break;

                    case LogLevel.Error:
                        _logger.Error(message);
                        break;

                    case LogLevel.Information:
                        _logger.Info(message);
                        break;

                    case LogLevel.Warning:
                        _logger.Warn(message);
                        break;

                    default:
                        _logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        _logger.Info(message, exception);
                        break;
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _logger.IsFatalEnabled;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _logger.IsDebugEnabled;

                case LogLevel.Error:
                    return _logger.IsErrorEnabled;

                case LogLevel.Information:
                    return _logger.IsInfoEnabled;

                case LogLevel.Warning:
                    return _logger.IsWarnEnabled;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public static class Log4NetProviderExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            factory.AddProvider(new Log4NetLoggerProvider(log4NetConfigFile));
            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
            => factory.AddLog4Net("log4net.config");
    }
}