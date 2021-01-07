using DesctopAptekaHelper.Parsers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DesctopAptekaHelper
{
    /// <summary>
    /// Interaction logic for AptekaApril.xaml
    /// </summary>
    public partial class AptekaApril : Page
    {
        BaseSiteParser _parser;
        private List<string> _file;
        public AptekaApril(BaseSiteParser parser)
        {
            InitializeComponent();
            _parser = parser;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parser.Init(_file, City.Text);
            //_parser.SaveToFile();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                this.FileButton.Content = $"File:\"{filename}\"";
                _file = System.IO.File.ReadAllLines(filename).ToList();
            }
        }
    }
}
