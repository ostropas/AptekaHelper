using AptekaHelper.Extensions;
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
        public override string Name => "Апрель";

        protected override string _siteUrl => "https://api.apteka-april.ru";
        
        private Dictionary<string, int> _citiesDistricts = new Dictionary<string, int>()
        {
            ["Волгоград"] = 776,
            ["Волжский"] = 777,
            ["Астрахань"] = 872
        };
        private Dictionary<int, Pharmacy> _pharmacies;

        protected override async Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data)
        {
            var task = new Task<List<Apteka>>(() => FindProduct(driver, data));
            task.Start();
            Random rnd = new Random();
            var wait = rnd.Next(2000, 6000);
            await Task.Delay(wait);
            return await task;
        }

        private List<Apteka> FindProduct(IWebDriver driver, IdsData data)
        {
            var cityId = _citiesDistricts[_city];
            var pharmacies = BrowserGetRequest<List<Product>>($"/pharmacies/stock?productID={{{data.Id}}}&districtID={cityId}");
            return pharmacies.Select(x => new Apteka(data.ProductName,
                _pharmacies[x.pharmacyID].address,
                _pharmacies[x.pharmacyID].address,
                x.count.ToString(),
                "Апрель",
                x.productID.ToString(),
                _city)).ToList();
        }

        protected override void ClearBasket(IWebDriver driver)
        {
        }

        protected override void Login(IWebDriver driver)
        {
        }

        protected override bool SetCity(IWebDriver driver, string city)
        {
            var cityId = _citiesDistricts[_city];
            var addreses = BrowserGetRequest<List<Pharmacy>>($"/pharmacies?districtID={cityId}");
            _pharmacies = addreses.ToDictionary(x => x.ID);
            return true;
        }
        
        
        [Serializable]
        public class Pharmacy
        {
            public int ID { get; set; }
            public string address { get; set; }
        }

        [Serializable]
        public class Product
        {
            public int productID { get; set; }
            public int pharmacyID { get; set; }
            public int count { get; set; }
        }
    }
}
