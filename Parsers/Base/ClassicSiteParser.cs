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
        public override async Task<List<Apteka>> ParseSite(Stopwatch sw)
        {
            await PrepareInits();
            UpdateProgress(0);
            List<Apteka> result = new List<Apteka>();
            List<Task<List<Apteka>>> tasks = new List<Task<List<Apteka>>>();
            foreach (var data in _fileData)
            {
                var task = new Task<List<Apteka>>(() => AddProduct(new IdsData(data)).Result);
                tasks.Add(task);
                task.Start();
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                result.AddRange(item);
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
