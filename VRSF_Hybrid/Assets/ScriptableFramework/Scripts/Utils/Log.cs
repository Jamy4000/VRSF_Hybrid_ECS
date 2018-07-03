using System.Diagnostics;
using System.IO;

namespace ScriptableFramework.Util
{
    public static class Log
    {
        public static string LogFile;
        public static Stopwatch StartTime = Stopwatch.StartNew();

        private static int _writers = 0;
        private static object lockObj = new object();
        private static string _logFile;

        public static void Write(int text, bool timestamp = true)
        {
            Write(text.ToString(), timestamp);
        }
        public static void Write(float text, bool timestamp = true)
        {
            Write(text.ToString(), timestamp);
        }
        public static void Write(decimal text, bool timestamp = true)
        {
            Write(text.ToString(), timestamp);
        }

        public static void Write(string text, bool timestamp = true)
        {
            while (_writers > 0)
            {
            }
            lock (lockObj)
            {
                _writers++;
                if (string.IsNullOrEmpty(_logFile))
                {
                    _logFile = LogFile;
                    if (string.IsNullOrEmpty(_logFile))
                    {
                        _writers--;
                        return;
                    }
                }
                if (!Directory.Exists(Path.GetDirectoryName(_logFile)))
                {
                    _writers--;
                    return;
                }
                File.AppendAllText(_logFile, GetCurTimeStamp() + ": " + text + "\r\n");
                _writers--;
            }
        }

        static string GetCurTimeStamp()
        {
            var elapsed = StartTime.Elapsed;
            return string.Format("{0}:{1}:{2}", elapsed.Minutes, elapsed.Seconds, elapsed.Ticks);
        }
    }
}
