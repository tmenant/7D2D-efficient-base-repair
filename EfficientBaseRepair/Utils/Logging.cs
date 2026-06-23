using System.Linq;
using System.Reflection;


public enum LoggingLevel : byte
{
    DEBUG,
    INFO,
    WARNING,
    ERROR,
    NONE,
}


public class Logging
{
    public class Logger
    {
        public LoggingLevel loggingLevel = LoggingLevel.DEBUG;

        public string loggerName;

        public Logger(string name, LoggingLevel level = LoggingLevel.DEBUG)
        {
            loggerName = name;
            loggingLevel = level;
        }

        private string ObjectsToString(object[] objects)
        {
            return string.Join(" ", objects.Select(obj => obj?.ToString()));
        }

        public void Debug(params object[] objects)
        {
            if (loggingLevel > LoggingLevel.DEBUG)
                return;

            Log.Out($"[{loggerName}] {ObjectsToString(objects)}");
        }

        public void Info(params object[] objects)
        {
            if (loggingLevel > LoggingLevel.INFO)
                return;

            Log.Out($"[{loggerName}] {ObjectsToString(objects)}");
        }

        public void Warning(params object[] objects)
        {
            if (loggingLevel > LoggingLevel.WARNING)
                return;

            Log.Warning($"[{loggerName}] {ObjectsToString(objects)}");
        }

        public void Error(params object[] objects)
        {
            if (loggingLevel > LoggingLevel.ERROR)
                return;

            Log.Error($"[{loggerName}] {ObjectsToString(objects)}");
        }
    }

    public static readonly LoggingLevel loggingLevel = LoggingLevel.DEBUG;

    private static readonly Logger root = new Logger("Unknown", loggingLevel);

    public static Logger CreateLogger(string name, LoggingLevel level = LoggingLevel.DEBUG)
    {
        return new Logger(name, level);
    }

    public static Logger CreateLogger<T>(LoggingLevel level = LoggingLevel.DEBUG)
    {
        return new Logger(
            name: $"{Assembly.GetCallingAssembly().GetName().Name}.{typeof(T).Name}",
            level: level
        );
    }

    public static void Debug(params object[] objects)
    {
        root.loggerName = Assembly.GetCallingAssembly().GetName().Name;
        root.Debug(objects);
    }

    public static void Info(params object[] objects)
    {
        root.loggerName = Assembly.GetCallingAssembly().GetName().Name;
        root.Info(objects);
    }

    public static void Warning(params object[] objects)
    {
        root.loggerName = Assembly.GetCallingAssembly().GetName().Name;
        root.Warning(objects);
    }

    public static void Error(params object[] objects)
    {
        root.loggerName = Assembly.GetCallingAssembly().GetName().Name;
        root.Error(objects);
    }

}
