using System;

namespace KappaUtility.Common.Misc
{
    internal class Logger
    {
        public enum LogLevel
        {
            Error,
            Info,
            Warn
        }

        public static bool Send(string str, Exception ex, LogLevel level = LogLevel.Info)
        {
            var date = DateTime.Now.ToString("[H:mm:ss - ") + "KappaUtility";
            string text;
            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    text = date + " Info] ";
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    text = date + " Warn] ";
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    text = date + " Error] ";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    text = date + " Info] ";
                    break;
            }

            Console.WriteLine(text + str);
            if (ex != null)
                Console.WriteLine(ex);
            Console.ResetColor();
            return true;
        }

        public static bool Send(string str, LogLevel level = LogLevel.Info)
        {
            var date = DateTime.Now.ToString("[H:mm:ss - ") + "KappaUtility";
            string text;
            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    text = date + " Info] ";
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    text = date + " Warn] ";
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    text = date + " Error] ";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    text = date + " Info] ";
                    break;
            }

            Console.WriteLine(text + str);
            Console.ResetColor();
            return true;
        }
    }
}
