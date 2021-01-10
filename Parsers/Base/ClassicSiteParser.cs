using AptekaHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class ClassicSiteParser : BaseSiteParser
    {
        public override async Task<List<Apteka>> ParseSite(Stopwatch sw)
        {
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

        protected abstract Task<List<Apteka>> AddProduct(IdsData data);
    }
}
