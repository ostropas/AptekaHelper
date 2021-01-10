using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class SeleniumSiteParser : BaseSiteParser, IDisposable
    {
        protected IWebDriver _webDriver;

        protected void InitWebDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--window-size=1920,1080");
            options.AddArguments("--start-maximized");
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
