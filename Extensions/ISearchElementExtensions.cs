using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace AptekaHelper.Extensions
{
    public static class ISearchElementExtensions
    {
        public static bool ElementExist(this ISearchContext context, By by)
        {
            var elements = context.FindElements(by);
            return elements.Count > 0;
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static ReadOnlyCollection<IWebElement> FindElements(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => (drv.FindElements(by).Count > 0) ? drv.FindElements(by) : null);
            }
            return driver.FindElements(by);
        }

        public static bool WaitElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            try
            {
                var wdw = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wdw.Until(ExpectedConditions.ElementExists(by));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void WaitCondition<TResult>(this IWebDriver driver, Func<IWebDriver, TResult> condition, int timeoutInSeconds)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(condition);
        }

        public static void WaitToBeClickable(this IWebDriver driver, IWebElement element, int timeoutInSeconds = 10)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(ExpectedConditions.ElementToBeClickable(element));
        }
    }
}
