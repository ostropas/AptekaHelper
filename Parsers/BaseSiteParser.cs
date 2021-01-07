using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesctopAptekaHelper
{
    public abstract class BaseSiteParser
    {
        private List<IdsData> _fileData;
        private string _city;
        protected virtual bool _parallel { get; } = false;
        public abstract string Name { get; }
        public virtual bool NeedCity { get; } = true;

        public event Action<float> ProgressUpdated;

        public void Init(List<string> file, string city)
        {
            _fileData = file.Select(x => new IdsData(x)).ToList();
            _city = city;
        }

        public async Task SaveToFile()
        {
            if (_parallel)
            {
                await SaveParalell();
            } else
            {
                await SaveToFileWithDriver();
            }
        }

        private async Task SaveToFileWithDriver()
        {
            ProgressUpdated.Invoke(0);
            var driver = InitWebDriver();
            Login(driver);
            SetCity(driver, _city);
            List<Apteka> result = new List<Apteka>();
            for (int i = 0; i < _fileData.Count; i++)
            {
                var data = _fileData[i];
                ClearBasket(driver);
                var res = await AddProduct(driver, data);
                result.AddRange(res);
                ProgressUpdated.Invoke((float)(i + 1) / _fileData.Count);
            }
            ProgressUpdated.Invoke(1);
            var dataWriter = new DataWriter();
            dataWriter.Write($"{Name}_{_city}", result);
            driver.Quit();
        }

        private async Task SaveParalell()
        {
            ProgressUpdated.Invoke(0);
            List<Apteka> result = new List<Apteka>();
            List<Task<List<Apteka>>> tasks = new List<Task<List<Apteka>>>();
            foreach (var data in _fileData)
            {
                var task = new Task<List<Apteka>>(() => AddProduct(null, new IdsData(data)).Result);
                tasks.Add(task);
                task.Start();
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                result.AddRange(item);
            }

            ProgressUpdated.Invoke(1);
            var dataWriter = new DataWriter();
            dataWriter.Write($"{Name}_{_city}", result);
        }

        protected void WebWait(Action predicate)
        {
            WebWait(() =>
            {
                predicate.Invoke();
                return true;
            });
        }

        protected void WebWait(Func<bool> predicate)
        {
            bool ready;
            do
            {
                Thread.Sleep(100);
                try
                {
                    ready = predicate.Invoke();
                }
                catch
                {
                    ready = false;
                }
            } while (!ready);
        }

        protected abstract IWebDriver InitWebDriver();
        protected abstract void Login(IWebDriver driver);
        protected abstract void SetCity(IWebDriver driver, string city);
        protected virtual async Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data) => null;
        protected abstract void ClearBasket(IWebDriver driver);
    }
}
