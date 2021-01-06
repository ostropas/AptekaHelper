﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesctopAptekaHelper.Parsers
{
    public class AprilSiteParser : BaseSiteParser
    {
        protected override string _outPutFileName => "april.csv";

        protected override List<Apteka> AddProduct(IWebDriver driver, IdsData data)
        {
            driver.Navigate().GoToUrl($"https://apteka-april.ru/product/{data.Id}");
            var selection = driver.FindElement(By.ClassName("buy"));
            var buyButton = selection.FindElement(By.TagName("button"));
            buyButton.Click();

            Utils.WebWait(() => driver.FindElement(By.ClassName("quantity")));
            driver.Navigate().GoToUrl("https://apteka-april.ru/basket");

            Utils.WebWait(() => driver.FindElement(By.ClassName("quantity")));

            var quantity = driver.FindElement(By.ClassName("quantity"));
            var input = quantity.FindElement(By.TagName("input"));
            input.SendKeys(Keys.Backspace);
            input.SendKeys(data.Count.ToString());
            input.SendKeys(Keys.Enter);

            Utils.WebWait(() =>
            {
                var selectionList = driver.FindElement(By.ClassName("product-list"));
                var title = selectionList.FindElement(By.TagName("h1"));
                return title.Text == $"В корзине {data.Count} товаров";
            });

            driver.Navigate().GoToUrl("https://apteka-april.ru/checkout");

            Utils.WebWait(() => driver.FindElement(By.ClassName("pharmacy")));

            var pharmacies = driver.FindElements(By.ClassName("pharmacy"));

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
                driver.Navigate().GoToUrl("https://apteka-april.ru/basket");
                WebWait(() => driver.FindElement(By.ClassName("c-basket-summary")));
                var selection = driver.FindElement(By.ClassName("c-basket-summary"));
                var li = selection.FindElements(By.TagName("li")).ElementAt(1);
                li.Click();
                WebWait(() =>
                {
                    var selectionElements = driver.FindElement(By.ClassName("product-list"));
                    var h1 = selectionElements.FindElement(By.TagName("h1"));
                    return h1.Text == "В корзине нет товаров";
                });
            }
            catch (Exception e)
            {
                var m = 0;
            }
        }

        protected override IWebDriver InitWebDriver()
        {
            IWebDriver driver = new ChromeDriver();
            return driver;
        }

        protected override void Login(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://apteka-april.ru/login");
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
            var button = selection.FindElement(By.TagName("button"));
            button.Click();
            var openedSelection = driver.FindElement(By.ClassName("opened"));
            var inputCity = openedSelection.FindElement(By.TagName("input"));
            inputCity.SendKeys(city);
            var firstCity = openedSelection.FindElement(By.ClassName("name"));
            firstCity.Click();
        }
    }
}
