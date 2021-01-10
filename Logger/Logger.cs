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
        private static TextWriter _w;
        static Logger()
        {
            _w = File.AppendText("log.txt");
        }

        public static void Log(string logMessage)
        {
            _w.Write("\r\nLog Entry : ");
            _w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            _w.WriteLine("  :");
            _w.WriteLine($"  :{logMessage}");
            _w.WriteLine("-------------------------------");
        }
    }
}
