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
        public override async Task<List<Apteka>> ParseSite()
        {
            UpdateProgress(0);
            InitWebDriver();
            Login(_webDriver);
            int counter = 0;
            while (!SetCity(_webDriver, _city)) {
                if (++counter > 5)
                {
                    Logger.Logger.Log("Невозможно установить город");
                    throw new Exception();
                }
            }
            List<Apteka> result = new List<Apteka>();
            for (int i = 0; i < _fileData.Count; i++)
            {
                var data = _fileData[i];
                try
                {
                    ClearBasket(_webDriver);
                    var res = await AddProduct(_webDriver, data);
                    result.AddRange(res);
                }
                catch (Exception e)
                {
                    Logger.Logger.Log($"Не удается распознать товар: {data.ProductName}, id: {data.Id}, сеть: {Name}, город:{_city}", e);
                    Logger.Logger.LogScreenShot(i, _webDriver);
                }                
                UpdateProgress((float)(i + 1) / _fileData.Count);
            }
            UpdateProgress(1);
            CloseWebDriver();
            return result;
        }

        protected virtual void Login(IWebDriver driver) { }
        protected abstract bool SetCity(IWebDriver driver, string city);
        protected abstract Task<List<Apteka>> AddProduct(IWebDriver driver, IdsData data);
        protected abstract void ClearBasket(IWebDriver driver);
    }
}
