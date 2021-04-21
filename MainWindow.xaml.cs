using AptekaHelper.Logger;
using AptekaHelper.Parsers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Threading;
using System;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int _currentVersion = 8;
        private Config _config;
        private string _configPath;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"AptekaHelper v:{ _currentVersion }";
            this.ParseButton.IsEnabled = false;
        }

        
        private string GetFileName()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter = "Ini documents (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                return filename;
            }

            return null;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var configFileName = GetFileName();
            if (string.IsNullOrEmpty(configFileName))
                return;
            var fileData = File.ReadAllText(configFileName);
            _config = JsonConvert.DeserializeObject<Config>(fileData);

            _configPath = Path.GetDirectoryName(configFileName);
            Logger.Logger.SetLogPath(Path.Combine(_configPath, "Output", "Logs.txt"));
            this.FileButton.Content = $"File:\"{configFileName}\"";
            this.ParseButton.IsEnabled = true;
        }

        private void UpdateProgress(float progress)
        {
            var value = progress * 100;
            Progress.Value = value;
            ProgressText.Content = $"{Math.Round(value, 0)}%";
        }

        private async void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_config == null)
            {
                return;
            }

            this.FileButton.IsEnabled = false;
            this.ParseButton.IsEnabled = false;
            Directory.CreateDirectory(Path.Combine(_configPath, "Output"));

            var allIdsCount = _config.Parsers.Sum(x => x.Ids.Count * x.Cities.Count);
            var completedIds = 0;
            var localIds = 0;

            Action<float> localUpdateProgress = (progress) =>
            {
                UpdateProgress((float)(completedIds + localIds * progress) / allIdsCount);
            };

            List<Apteka> result = new List<Apteka>();

            var dataWriter = new DataWriter();
            dataWriter.SetDirectory(Path.Combine(_configPath, "Output"));


            foreach (var parser in _config.Parsers)
            {
                await parser.SiteParser.CommonInit();
                foreach (var _city in parser.Cities)
                {
                    parser.SiteParser.Init(parser.Ids, _city);
                    localIds = parser.Ids.Count;
                    parser.SiteParser.ProgressUpdated += localUpdateProgress;
                    var res = await parser.SiteParser.ParseSite();
                    result.AddRange(res);
                    parser.SiteParser.ProgressUpdated -= localUpdateProgress;
                    completedIds += localIds;
                }
            }

            dataWriter.Write($"Результаты парсинга", result);


            this.FileButton.IsEnabled = true;
            this.ParseButton.IsEnabled = true;
        }
    }
}