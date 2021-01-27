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
        public static void Init(Action<Log> action)
        {
            _logAction = action;
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
    }

    public class Log {
        public string Data { get; set; }
        public string FullData { get; set; }
    }
}
