﻿using AptekaHelper.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
            driver.WaitToBeClickable(buyButton);
            buyButton.Click();

            driver.WaitElement(By.ClassName("quantity"), 10);

            driver.Navigate().GoToUrl(GetAbsolutePath("basket"));


            var quantity = driver.FindElement(By.ClassName("quantity"), 10);

            var input = quantity.FindElement(By.TagName("input"));
            input.SendKeys(Keys.Backspace);
            input.SendKeys(data.Count.ToString());
            input.SendKeys(Keys.Enter);

            var cartButton = driver.FindElement(By.ClassName("q-button"), 10);
            driver.WaitToBeClickable(cartButton);

            //var qBadge = driver.FindElement(By.ClassName("q-badge"), 10);

            //Thread.Sleep(500);

            //driver.WaitCondition(drv => 
            //{
            //    var text = qBadge.Text;
            //    return ExpectedConditions.TextToBePresentInElement(qBadge, data.Count);
            //}, 10);

            //Thread.Sleep(500);

            driver.Navigate().GoToUrl(GetAbsolutePath("checkout"));

            try
            {
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
            catch (Exception e)
            {
                return new List<Apteka>();
            }
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

        protected override bool SetCity(IWebDriver driver, string city)
        {
            try
            {
                var selection = driver.FindElement(By.ClassName("c-select-city-desktop"), 10);

                var button = driver.FindElement(By.TagName("button"), 10);
                driver.WaitToBeClickable(button);
                button.Click();

                Thread.Sleep(1000);
                var openedSelection = driver.FindElement(By.ClassName("opened"), 10);
                var inputCity = openedSelection.FindElement(By.TagName("input"));
                inputCity.SendKeys(city);
                driver.WaitCondition(drv =>
                {
                    var firstCity = openedSelection.FindElement(By.ClassName("name"));
                    return ExpectedConditions.TextToBePresentInElement(firstCity, city);
                }, 10);
                Thread.Sleep(1000);
                var cityElement = openedSelection.FindElement(By.ClassName("name"));
                driver.WaitToBeClickable(cityElement);
                cityElement.Click();
                Thread.Sleep(1000);
                var newCity = driver.FindElement(By.ClassName("c-select-city-desktop"), 10);
                var text = newCity.Text;
                if (text != city)
                    throw new Exception("Incorrect city");

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
