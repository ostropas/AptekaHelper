using AptekaHelper.Parsers;
using CsQuery;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public class VolgofarmSiteParser : ClassicSiteParser
    {
        public override string Name => "Волгофарм";

        protected override string _siteUrl => "http://volgofarm.ru";

        protected override async Task<List<Apteka>> AddProduct(IdsData data)
        {
            var baseAddress = new Uri(_siteUrl);
            var cookieContainer = new CookieContainer();
            string res = "";
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new System.Net.Cookie($"basket[{data.Id}]", data.Count));
                    var result = await client.GetAsync("/step2.html");
                    result.EnsureSuccessStatusCode();
                    res = await result.Content.ReadAsStringAsync();
                }
            }
            var resData = Parse(res, data.ProductName, data.Id);

            var allDataIsZero = resData.All(x => x.Count == "0");
            if (allDataIsZero)
            {
                throw new Exception();
            }
            return resData;
        }

        private List<Apteka> Parse(string sitePage, string productTitle, string productID)
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

                content.Add(new Apteka(productTitle, titles[i], address, count, "Волгофарм", productID));
            }

            return content;
        }
    }
}
