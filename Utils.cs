using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DesctopAptekaHelper
{
    public static class Utils
    {
        public static void WebWait(Action predicate)
        {
            WebWait(() =>
            {
                predicate.Invoke();
                return true;
            });
        }

        public static void WebWait(Func<bool> predicate)
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
    }
}
