using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AptekaHelper.Parsers
{
    public abstract class BaseSiteParser
    {
        public abstract Task<List<Apteka>> ParseSite();
        public abstract string Name { get; }
        public event Action<float> ProgressUpdated;
        protected abstract string _siteUrl { get; }

        protected List<IdsData> _fileData;
        protected string _city;

        public void Init(List<IdsData> file, string city)
        {
            _fileData = file;
            _city = city;
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
