using AptekaHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class BaseSiteParser
    {
        public abstract Task<List<Apteka>> ParseSite(Stopwatch sw);
        public abstract string Name { get; }
        public event Action<float> ProgressUpdated;
        protected abstract string _siteUrl { get; }

        protected List<IdsData> _fileData;
        protected string _city;
        protected bool _showBrowser;

        public void Init(List<string> file, string city, bool showBrowser)
        {
            _fileData = file.Select(x => new IdsData(x)).ToList();
            _city = city;
            _showBrowser = showBrowser;
        }

        protected string GetAbsolutePath(string relPath) => $"{_siteUrl}/{relPath}";

        public void WriteToFile(List<Apteka> result, DataWriter dataWriterP = null)
        {
            var dataWriter = dataWriterP == null ? new DataWriter() : dataWriterP;
            dataWriter.Write($"{Name}_{_city}", result);
        }

        protected void UpdateProgress(float progress) => ProgressUpdated?.Invoke(progress);
    }
}
