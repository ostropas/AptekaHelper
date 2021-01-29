using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;

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

        public static void LogScreenShot(int pos, IWebDriver driver)
        {
            var vcs = new VerticalCombineDecorator(new ScreenshotMaker());
            var screen = driver.TakeScreenshot(vcs);
            var time = DateTime.Now.ToLocalTime().ToString();
            time = time.Replace(':', '_');
            var finalPath = Path.Combine(_path, $"{pos}_{time}_Image.png");

            using (Image image = Image.FromStream(new MemoryStream(screen)))
            {
                image.Save(finalPath, ImageFormat.Png);
            }
        }
    }

    public class Log {
        public string Data { get; set; }
        public string FullData { get; set; }
    }
}
