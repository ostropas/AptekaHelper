using System;
using System.Collections.Generic;
using System.Linq;
using AptekaHelper.Parsers;
using Newtonsoft.Json;

namespace AptekaHelper
{
    [Serializable]
    public class Config
    {
        public List<Parser> Parsers { get; set; }
    }

    public enum ParserEnum
    {
        April,
        Asna,
        Volgofarm
    }

    [Serializable]
    public class Parser
    {
        private Dictionary<ParserEnum, Type> _parsers = new Dictionary<ParserEnum, Type>()
        {
            [ParserEnum.April] = typeof(AprilSiteParser),
            [ParserEnum.Asna] = typeof(AsnaSiteParser),
            [ParserEnum.Volgofarm] = typeof(VolgofarmSiteParser)
        };
        
       public ParserEnum ParserType { get; set; }
       public BaseSiteParser SiteParser { get; set; }
       public List<string> Cities { get; set; }
       public List<IdsData> Ids { get; set; }

       public Parser(ParserEnum parserType, List<string> cities, List<string> ids)
       {
           ParserType = parserType;
           Cities = cities;
           SiteParser = (BaseSiteParser)Activator.CreateInstance(_parsers[parserType]);
           Ids = ids.Select(x => new IdsData(x)).ToList();
       }
    }
    
    
    [Serializable]
    public class IdsData
    {
        public string ProductName;
        public string Id;
        public string Count;

        public IdsData(string productName, string id, string count)
        {
            ProductName = productName;
            Id = id;
            Count = count;
        }

        public IdsData(string def)
        {
            var vals = def.Split(';');
            ProductName = vals[0];
            Id = vals[1];
            Count = vals[2];
        }

        public IdsData(IdsData idsData)
        {
            ProductName = idsData.ProductName;
            Id = idsData.Id;
            Count = idsData.Count;
        }
    }
}