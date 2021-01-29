using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Logger
{
    public static class Logger
    {
        private static Action<Log> _logAction;
        private static string _path;
        public static void Init(Action<Log> action)
        {
            _logAction = action;
        }

        public static void SetLogPath(string path)
        {
            _path = path;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void Log(string logMessage, Exception e = null)
        {
            var log = new Log()
            {
                Data = logMessage
            };
            if (e != null)
                log.FullData = e.StackTrace;
            _logAction.Invoke(log);
        }

        public static void LogScreenShot(int pos, Screenshot ss)
        {
            var time = DateTime.Now.ToLocalTime().ToString();
            time = time.Replace(':', '_');
            var finalPath = Path.Combine(_path, $"{pos}_{time}_Image.png");
            ss.SaveAsFile(finalPath,
            ScreenshotImageFormat.Png);
        }
    }

    public class Log {
        public string Data { get; set; }
        public string FullData { get; set; }
    }
}
