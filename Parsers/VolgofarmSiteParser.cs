using CsQuery;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DesctopAptekaHelper.Parsers
{
    public class VolgofarmSiteParser : BaseSiteParser
    {
        private string _siteUrl = "http://volgofarm.ru";
        protected override string _outPutFileName => "volgofarm.csv";
        public override string Name => "Волгофарм";
        protected override bool _parallel => true;

        protected override List<Apteka> AddProduct(IWebDriver driver, IdsData data)
        {
            var baseAddress = new Uri(_siteUrl);
            var cookieContainer = new CookieContainer();
            string res = "";
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new System.Net.Cookie($"basket[{data.Id}]", data.Count));
                    var result = client.GetAsync("/step2.html").Result;
                    result.EnsureSuccessStatusCode();
                    res = result.Content.ReadAsStringAsync().Result;
                }
            }
            return Parse(res, data.ProductName);
        }

        private List<Apteka> Parse(string sitePage, string productTitle)
        {
            var content = new List<Apteka>();

            CQ cq = CQ.Create(sitePage);

            var titles = cq[".apt_title"].Select(x => x.Cq().Text()).ToList();
            var addresses = cq[".apt_adress"].Select(x => x.Cq().Text()).ToList();
            var counts = cq[".vnalichii_vsego"].Select(x => x.Cq().Text()).ToList();

            for (int i = 0; i < titles.Count; i++)
            {
                var address = addresses[i];
                if (address.Contains("\r\n"))
                    address = address.Substring(0, address.IndexOf("\r\n"));
                var count = "";
                if (i < counts.Count)
                    count = counts[i];

                if (string.IsNullOrEmpty(count))
                    count = "0";

                content.Add(new Apteka(productTitle, titles[i], address, count));
            }

            return content;
        }

        protected override void ClearBasket(IWebDriver driver)
        {
            //try
            //{
            //    var badge = driver.FindElement(By.ClassName("q-badge"));
            //    driver.Navigate().GoToUrl("https://apteka-april.ru/basket");
            //    WebWait(() => driver.FindElement(By.ClassName("c-basket-summary")));
            //    var selection = driver.FindElement(By.ClassName("c-basket-summary"));
            //    var li = selection.FindElements(By.TagName("li")).ElementAt(1);
            //    li.Click();
            //    WebWait(() =>
            //    {
            //        var selectionElements = driver.FindElement(By.ClassName("product-list"));
            //        var h1 = selectionElements.FindElement(By.TagName("h1"));
            //        return h1.Text == "В корзине нет товаров";
            //    });
            //}
            //catch (Exception e)
            //{
            //    var m = 0;
            //}
        }

        protected override IWebDriver InitWebDriver()
        {
            return null;
            //IWebDriver driver = new ChromeDriver();
            //return driver;
        }

        protected override void Login(IWebDriver driver)
        {
            //driver.Navigate().GoToUrl("https://apteka-april.ru/login");
            //IWebElement number = driver.FindElement(By.ClassName("q-edit-phone"));
            //var input = number.FindElement(By.TagName("input"));
            //input.Click();
            //input.SendKeys("9370886609");

            //var passwordField = driver.FindElements(By.ClassName("q-edit"))
            // .Where(x => x.GetProperty("placeholder").ToString()
            // .Contains("Пароль"))
            // .First();
            //passwordField.Click();
            //passwordField.SendKeys("Jd3oQ98P");

            //var enterButton = driver.FindElement(By.ClassName("primary"));
            //enterButton.Click();
        }

        protected override void SetCity(IWebDriver driver, string city)
        {
            //var selection = driver.FindElement(By.ClassName("c-select-city-desktop"));
            //var button = selection.FindElement(By.TagName("button"));
            //button.Click();
            //var openedSelection = driver.FindElement(By.ClassName("opened"));
            //var inputCity = openedSelection.FindElement(By.TagName("input"));
            //inputCity.SendKeys(city);
            //var firstCity = openedSelection.FindElement(By.ClassName("name"));
            //firstCity.Click();
        }
    }
}
