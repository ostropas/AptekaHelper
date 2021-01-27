using AptekaHelper.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for SitePage.xaml
    /// </summary>
    public partial class SitePage : Page
    {
        private float _avrIterationTime;
        private Stopwatch _sw = new Stopwatch();
        private Stopwatch _allTime = new Stopwatch();
        BaseSiteParser _parser;
        DataWriter _dataWriter;
        private List<string> _idsFile = new List<string>();
        private List<string> _cityFile = new List<string>();
        private List<string> _configFile = new List<string>();
        private bool _showBrowser;

        public SitePage(BaseSiteParser parser)
        {
            InitializeComponent();
            _parser = parser;
            _parser.ProgressUpdated += UpdateProgressBar;
            _dataWriter = new DataWriter();
        }

        private void UpdateProgressBar(float progress)
        {
            var value = progress * 100;
            Progress.Value = value;
            ProgressText.Content = $"{Math.Round(value, 0)}%";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _sw.Start();
            _allTime.Start();

            _avrIterationTime = 0;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async delegate ()
            {
                foreach (var city in _cityFile)
                {
                    _parser.Init(_idsFile, city, _showBrowser);
                    var res = await _parser.ParseSite(_sw);
                    _parser.WriteToFile(res, _dataWriter);
                    _sw.Reset();
                    _allTime.Reset();
                }
            }));
        }

        private string GetFileName()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter = "Ini documents (.ini)|*.ini"; // Filter files by extension

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

        private List<string> ReadFileData(string fileName) => File.ReadAllLines(fileName).Where(x => !string.IsNullOrEmpty(x)).ToList();

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var configFileName = GetFileName();
            if (string.IsNullOrEmpty(configFileName))
                return;
            var fileData = ReadFileData(configFileName);
            string idsFile = fileData[0];
            string cityFile = fileData[1];
            string outPutDir = fileData[2];

            string configFileDirectory = Path.GetDirectoryName(configFileName);
            _idsFile = ReadFileData(Path.Combine(configFileDirectory, idsFile));
            _cityFile = ReadFileData(Path.Combine(configFileDirectory, cityFile));
            _dataWriter.SetDirectory(Path.Combine(configFileDirectory, outPutDir));
            this.FileButton.Content = $"File:\"{configFileName}\"";
        }

        private void ShowBrowser_Checked(object sender, RoutedEventArgs e)
        {
            var obj = (CheckBox)sender;
            _showBrowser = obj.IsChecked.HasValue && obj.IsChecked.Value;
        }
    }
}
