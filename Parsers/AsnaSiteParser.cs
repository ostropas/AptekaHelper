using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public class AsnaSiteParser : ClassicSiteParser
    {
        public override string Name => "Асна";

        protected override string _siteUrl => "https://www.asna.ru/api/v1.0";
        private (string, string) _authToken;
       
        private Dictionary<string, (int, int)> _citiesDistricts = new Dictionary<string, (int, int)>()
        {
            ["Волгоград"] = (15, 176),
            ["Волжский"] = (15, 962),
            ["Астрахань"] = (7, 130)
        };

        private Dictionary<int, PharmacyAttributes> _pharmacies;

        protected override async Task<List<Apteka>> AddProduct(IdsData data)
        {
            var rnd = new Random();
            await Task.Delay(rnd.Next(2000, 4000));
            var cityId = _citiesDistricts[_city];
            var productResponse = await GetRequest<ProductResponse>($"https://www.asna.ru/api/v1.0/prices/?region={cityId.Item1}&city={cityId.Item2}&product[{data.Id}]={data.Count}",
                true, 
                _authToken);

            var res = productResponse.data.Where(p => _pharmacies.ContainsKey(p.attributes.pharmacie)).Select(x => new Apteka(data.ProductName,
                _pharmacies[x.attributes.pharmacie].address,
                _pharmacies[x.attributes.pharmacie].address,
                x.attributes.offers[0].quantity.count.ToString(),
                "Асна", data.Id)).ToList();

            if (res.Count == 0)
            {
                throw new Exception();
            }

            return res;
        }

        protected override async Task PrepareInits()
        {
            var city = _citiesDistricts[_city];
            var tokenResponse = await GetRequest<TokenResponse>(
                "https://www.asna.ru/api/v1/connect/token/?clientId=26f275ef-d9ae-426b-963b-a8d6150fa5ca&clientSecret=D4mwTem-xlU^&grantType=client_credentials",
                true);
            _authToken = ("Authorization", $"Bearer {tokenResponse.access_token}");

            var addreses =
                await GetRequest<PharmacyResponse>($"https://www.asna.ru/api/v1.0/pharmacies/?region={city.Item1}&city={city.Item2}", true,
                    headers:_authToken);
            
            _pharmacies = addreses.data.ToDictionary(x => x.id, x => x.attributes);
        }

        [Serializable]
        public class TokenResponse
        {
            public string access_token { get; set; }
        }
        
        [Serializable]
        public class PharmacyResponse
        {
            public List<Pharmacy> data { get; set; }
        }

        [Serializable]
        public class Pharmacy
        {
            public int id { get; set; }
            public PharmacyAttributes attributes { get; set; }
        }

        [Serializable]
        public class PharmacyAttributes
        {
            public string name { get; set; }
            public string address { get; set; }
        }

        [Serializable]
        public class ProductResponse
        {
            public List<Product> data { get; set; }
        }

        [Serializable]
        public class Product
        {
            public ProductAttributes attributes { get; set; }
        }
        
        
        [Serializable]
        public class ProductAttributes
        {
            // id
            public int pharmacie { get; set; }
            public List<ProductOffer> offers { get; set; }
        }

        [Serializable]
        public class ProductOffer
        {
            public Quantity quantity { get; set; }
        }

        [Serializable]
        public class Quantity
        {
            public int count { get; set; }
        }
    }
}
