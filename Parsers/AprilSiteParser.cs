using AptekaHelper.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public class AprilSiteParser : SeleniumLongSiteParser
    {
        public override string Name => "April";

        protected override string _siteUrl => "https://apteka-april.ru";

        protected override async Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data)
        {
            var task = new Task<List<Apteka>>(() => FindProduct(driver, data));
            task.Start();
            return await task;
        }

        private List<Apteka> FindProduct(IWebDriver driver, IdsData data)
        {
            driver.Navigate().GoToUrl(GetAbsolutePath($"product/{data.Id}"));

            if (driver.WaitElement(By.ClassName("no-product-in-city"), 1))
                return new List<Apteka>();

            var selection = driver.FindElement(By.ClassName("buy"));
            var buyButton = selection.FindElement(By.TagName("button"));
            buyButton.Click();

            driver.WaitElement(By.ClassName("quantity"), 10);

            driver.Navigate().GoToUrl(GetAbsolutePath("basket"));


            var quantity = driver.FindElement(By.ClassName("quantity"), 10);

            var input = quantity.FindElement(By.TagName("input"));
            input.SendKeys(Keys.Backspace);
            input.SendKeys(data.Count.ToString());
            input.SendKeys(Keys.Enter);

            driver.WaitCondition(drv => 
            {
                var selectionList = driver.FindElement(By.ClassName("product-list"));
                var title = selectionList.FindElement(By.TagName("h1"));
                return ExpectedConditions.TextToBePresentInElement(title, $"В корзине {data.Count} товаров");
            }, 10);

            driver.Navigate().GoToUrl(GetAbsolutePath("checkout"));

            var pharmacies = driver.FindElements(By.ClassName("pharmacy"), 10);

            List<Apteka> res = pharmacies.Select(x =>
            {
                var pharmacyName = x.FindElement(By.ClassName("pharmacy-name"));
                var address = pharmacyName.FindElements(By.TagName("span")).Last().Text;
                var badge = x.FindElement(By.ClassName("badge")).Text;

                var countLeft = 0;

                if (badge == "Все")
                {
                    countLeft = int.Parse(data.Count);
                }
                else if (badge == "Под заказ")
                {
                    countLeft = 0;
                }
                else
                {
                    countLeft = int.Parse(badge.Substring(0, 1));
                }

                return new Apteka(data.ProductName, "Апрель", address, countLeft.ToString());
            }).ToList();


            return res;
        }

        protected override void ClearBasket(IWebDriver driver)
        {
            try
            {
                var badge = driver.FindElement(By.ClassName("q-badge"));
                driver.Navigate().GoToUrl(GetAbsolutePath("basket"));

                var selection = driver.FindElement(By.ClassName("c-basket-summary"), 10);
                var li = selection.FindElements(By.TagName("li")).ElementAt(1);
                li.Click();

                driver.WaitCondition(drv =>
                {
                    var selectionElements = driver.FindElement(By.ClassName("product-list"));
                    var h1 = selectionElements.FindElement(By.TagName("h1"));
                    return ExpectedConditions.TextToBePresentInElement(h1, "В корзине нет товаров");
                }, 10);
            }
            catch (Exception)
            {
            }
        }

        protected override void Login(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(GetAbsolutePath("login"));
            IWebElement number = driver.FindElement(By.ClassName("q-edit-phone"));
            var input = number.FindElement(By.TagName("input"));
            input.Click();
            input.SendKeys("9370886609");

            var passwordField = driver.FindElements(By.ClassName("q-edit"))
             .Where(x => x.GetProperty("placeholder").ToString()
             .Contains("Пароль"))
             .First();
            passwordField.Click();
            passwordField.SendKeys("Jd3oQ98P");

            var enterButton = driver.FindElement(By.ClassName("primary"));
            enterButton.Click();
        }

        protected override void SetCity(IWebDriver driver, string city)
        {
            var selection = driver.FindElement(By.ClassName("c-select-city-desktop"));

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var button = wait.Until(drv => drv.FindElement(By.TagName("button")));
            driver.WaitToBeClickable(button);
            button.Click();

            var openedSelection = driver.FindElement(By.ClassName("opened"));
            var inputCity = openedSelection.FindElement(By.TagName("input"));
            inputCity.SendKeys(city);
            driver.WaitCondition(drv =>
            {
                var firstCity = openedSelection.FindElement(By.ClassName("name"));
                return ExpectedConditions.TextToBePresentInElement(firstCity, city);
            }, 10);
            var cityElement = openedSelection.FindElement(By.ClassName("name"));
            driver.WaitToBeClickable(cityElement);
            cityElement.Click();
            Thread.Sleep(1000);
        }
    }
}
