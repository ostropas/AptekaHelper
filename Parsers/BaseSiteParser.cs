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
        protected virtual string _outPutFileName { get; }

        public void Init(List<string> file, string city)
        {
            _fileData = file.Select(x => new IdsData(x)).ToList();
            _city = city;
        }

        public void SaveToFile()
        {
            var driver = InitWebDriver();
            Login(driver);
            SetCity(driver, _city);
            List<Apteka> result = new List<Apteka>();
            foreach (var data in _fileData)
            {
                ClearBasket(driver);
                result.AddRange(AddProduct(driver, data));
            }
            var dataWriter = new DataWriter();
            dataWriter.Write(_outPutFileName, result);
            driver.Quit();
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
        protected abstract List<Apteka> AddProduct(IWebDriver driver, IdsData data);
        protected abstract void ClearBasket(IWebDriver driver);
    }
}
