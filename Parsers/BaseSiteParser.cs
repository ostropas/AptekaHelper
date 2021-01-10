﻿using AptekaHelper;
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

        public void Init(List<string> file, string city)
        {
            _fileData = file.Select(x => new IdsData(x)).ToList();
            _city = city;
        }

        protected string GetAbsolutePath(string relPath) => $"{_siteUrl}/{relPath}";

        public void WriteToFile(List<Apteka> result)
        {
            var dataWriter = new DataWriter();
            dataWriter.Write($"{Name}_{_city}", result);
        }

        protected void UpdateProgress(float progress) => ProgressUpdated?.Invoke(progress);
    }
}
