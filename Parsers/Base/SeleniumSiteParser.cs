using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class SeleniumSiteParser : BaseSiteParser, IDisposable
    {
        protected IWebDriver _webDriver;

        private static bool _consoleAllocated;

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        protected void InitWebDriver()
        {
            if (!_consoleAllocated)
                AllocConsole();

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--window-size=1920,1080");
            options.AddArguments("--start-maximized");
            if (!_showBrowser)
                options.AddArguments("--headless");

            _webDriver = new ChromeDriver(options);
        }

        protected void CloseWebDriver()
        {
            _webDriver.Quit();
        }

        public void Dispose()
        {
            if (_webDriver != null)
                _webDriver.Quit();
        }
    }
}
