using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AptekaHelper.Extensions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;
using UnidecodeSharpFork;

namespace AptekaHelper.Parsers
{
    public class AsnaSiteParser : SeleniumLongSiteParser
    {
        public override string Name => "Asna";

        protected override string _siteUrl => _correctedUrl;
        private string _correctedUrl = "https://www.asna.ru";

        protected override async Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data)
        {
            var task = new Task<List<Apteka>>(() => FindProduct(driver, data));
            task.Start();
            return await task;
        }

        private List<Apteka> FindProduct(IWebDriver driver, IdsData data)
        {
            driver.Navigate().GoToUrl(GetAbsolutePath($"cards/{data.Id}"));
            var plusButton = driver.FindElement(By.ClassName("js-product__btn-plus"), 10);
            driver.WaitToBeClickable(plusButton);
            for (int i = 0; i < int.Parse(data.Count) - 1; i++)
            {
                plusButton.Click();
                Thread.Sleep(200);
            }
            Thread.Sleep(1000);
            var addButton = driver.FindElement(By.ClassName("js-product-add-to-cart"), 10);
            driver.WaitToBeClickable(addButton);
            addButton.Click();


            driver.WaitCondition(drv => ExpectedConditions.TextToBePresentInElement(addButton, "Оформить"), 10);

            addButton.Click();
            //var toCartButton = driver.FindElement(By.ClassName("btn-basket js-product-add-to-cart"))

            //driver.WaitCondition(drv =>
            //{
            //    var miniCart = drv.FindElement(By.ClassName("js-test-miniCartCount"));
            //    return ExpectedConditions.TextToBePresentInElement(miniCart, data.Count);
            //}, 10);

            //driver.Navigate().GoToUrl(GetAbsolutePath("cart/"));
            Thread.Sleep(999999);

            return new List<Apteka>();
        }

        protected override void ClearBasket(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(GetAbsolutePath("cart"));
            try
            {
                var elements = driver.FindElements(By.ClassName("js-test-iconRemove"), 2);
                if (elements != null)
                {
                    foreach (var element in elements)
                    {
                        driver.WaitToBeClickable(element);
                        element.Click();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        protected override bool SetCity(IWebDriver driver, string city)
        {
            _correctedUrl = $"https://{city.ToLower().Unidecode()}.asna.ru".Replace("'", "");
            driver.Navigate().GoToUrl(_siteUrl);
            return true;
        }
    }
}
