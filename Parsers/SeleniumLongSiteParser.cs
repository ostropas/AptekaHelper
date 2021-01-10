﻿using AptekaHelper.Extensions;
using AptekaHelper.Parsers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AptekaHelper
{
    public abstract class SeleniumLongSiteParser : SeleniumSiteParser
    {
        public override async Task<List<Apteka>> ParseSite(Stopwatch sw)
        {
            UpdateProgress(0);
            InitWebDriver();
            Login(_webDriver);
            SetCity(_webDriver, _city);
            List<Apteka> result = new List<Apteka>();
            sw.Restart();
            for (int i = 0; i < _fileData.Count; i++)
            {
                var data = _fileData[i];
                ClearBasket(_webDriver);
                var res = await AddProduct(_webDriver, data);
                result.AddRange(res);

                UpdateProgress((float)(i + 1) / _fileData.Count);
            }
            UpdateProgress(1);
            CloseWebDriver();
            return result;
        }

        protected void WebWait(Action predicate, int times = 100)
        {
            WebWait(() =>
            {
                predicate.Invoke();
                return true;
            }, times);
        }

        protected IWebElement WebWaitElement(ISearchContext context, By by)
        {
            while (!context.ElementExist(by))
            {
                Thread.Sleep(100);
            }
            return context.FindElement(by);
        }

        protected void WebWait(Func<bool> predicate, int times = 100)
        {
            bool ready;
            int timesCount = 0;
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

                if (timesCount++ > times)
                    return;
            } while (!ready);
        }

        protected virtual void Login(IWebDriver driver) { }
        protected abstract void SetCity(IWebDriver driver, string city);
        protected abstract Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data);
        protected abstract void ClearBasket(IWebDriver driver);
    }
}
