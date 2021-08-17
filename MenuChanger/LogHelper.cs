using System;

namespace MenuChanger
{
    public static class LogHelper
    {
        public static event Action<string> OnLog;

        public static void Log(string message = "")
        {
            OnLog?.Invoke(message);
        }

        public static void Log<T>(T t) where T : struct
        {
            Log(t.ToString());
        }

        public static void Log(object message)
        {
            Log(message?.ToString() ?? "null");
        }
    }
}
