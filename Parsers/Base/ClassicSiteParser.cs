using AptekaHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class ClassicSiteParser : BaseSiteParser
    {
        public override async Task<List<Apteka>> ParseSite()
        {
            try
            {
                await PrepareInits();
            }
            catch (Exception e)
            {
                Logger.Logger.Log($"Не удается настроить парсер: сеть: {Name}, город: {_city}", e);
            }
            UpdateProgress(0);
            List<Apteka> result = new List<Apteka>();
            foreach (var (data, index) in _fileData.Select((x, i) => (x, i)))
            {
                try
                {
                    var rnd = new Random();
                    result.AddRange(await AddProduct(new IdsData(data)));
                    UpdateProgress((float)(index + 1) / _fileData.Count);
                }
                catch (Exception e)
                {
                    Logger.Logger.Log($"Не удается распознать товар: {data.ProductName}, id: {data.Id} сеть: {Name}, город: {_city}", e);
                }
            }

            UpdateProgress(1);
            return result;
        }

        protected async Task<T> GetRequest<T>(string path, bool overrideBaseUrl = false, params (string, string)[] headers)
        
        {
            string res = "";

            using (var client = new HttpClient())
            {
                if (!overrideBaseUrl)
                {
                    client.BaseAddress = new Uri(_siteUrl);
                    client.DefaultRequestHeaders.Host = client.BaseAddress.Host;
                }
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Item1, header.Item2);
                }

                var result = await client.GetAsync(path);
                res = await result.Content.ReadAsStringAsync();
            }
            

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(res);
        }

        protected abstract Task<List<Apteka>> AddProduct(IdsData data);

        protected virtual Task PrepareInits()
        {
            return Task.CompletedTask;
        }
    }
}
