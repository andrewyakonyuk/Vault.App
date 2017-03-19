using System;

namespace Vault.Shared
{
    public interface ILogger
    {
        /// <summary>
        /// Write message to the log entries
        /// </summary>
        /// <param name="category">The log entry category</param>
        /// <param name="message">The log entry message</param>
        /// <param name="args">Optional log entry arguments</param>
        void Write(LogCategory category, string message, params object[] args);
    }

    public static class LoggerExtensions
    {
        /// <summary>
        /// The shortcut for writing the log entry message into <seealso cref="LogCategory.Info"/> category
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="message">The log entry message</param>
        /// <param name="args">Optional log entry arguments</param>
        public static void WriteInfo(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                return;

            logger.Write(LogCategory.Info, message, args);
        }

        /// <summary>
        /// The shortcut for writing the log entry message into <seealso cref="LogCategory.Warning"/> category
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="message">The log entry message</param>
        /// <param name="args">Optional log entry arguments</param>
        public static void WriteWarning(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                return;

            logger.Write(LogCategory.Warning, message, args);
        }

        /// <summary>
        /// The shortcut for writing the log entry message into <seealso cref="LogCategory.Error"/> category
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="message">The log entry message</param>
        /// <param name="args">Optional log entry arguments</param>
        public static void WriteError(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                return;

            logger.Write(LogCategory.Error, message, args);
        }

        /// <summary>
        /// The shortcut to write the exception into <seealso cref="LogCategory.Error"/> category
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="ex">The exception to write as log entry</param>
        public static void WriteError(this ILogger logger, Exception ex)
        {
            if (logger == null)
                return;

            logger.Write(LogCategory.Error, ex.ToString());
        }

        public static void WriteError(this ILogger logger, Exception ex, string message, params object[] args)
        {
            if (logger == null)
                return;

            logger.Write(LogCategory.Error, string.Join(Environment.NewLine, ex.ToString(), message), args);
        }
    }

    public enum LogCategory
    {
        Info,
        Warning,
        Error
    }

    public sealed class ConsoleLogger : ILogger
    {
        public void Write(LogCategory category, string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;

            switch (category)
            {
                case LogCategory.Info:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                case LogCategory.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;

                case LogCategory.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                default:
                    break;
            }

            Console.WriteLine(message, args);

            Console.ForegroundColor = oldColor;
        }
    }
}