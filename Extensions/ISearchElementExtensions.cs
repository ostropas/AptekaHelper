using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Extensions
{
    public static class ISearchElementExtensions
    {
        public static bool ElementExist(this ISearchContext context, By by)
        {
            var elements = context.FindElements(by);
            return elements.Count > 0;
        }
    }
}
