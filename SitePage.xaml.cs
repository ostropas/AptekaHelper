using DesctopAptekaHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for SitePage.xaml
    /// </summary>
    public partial class SitePage : Page
    {
        BaseSiteParser _parser;
        private List<string> _file;
        public SitePage(BaseSiteParser parser)
        {
            InitializeComponent();
            _parser = parser;
            if (!_parser.NeedCity)
                City.IsEnabled = false;
            DownLoadButton.IsEnabled = false;
            _parser.ProgressUpdated += UpdateProgressBar;

        }

        private void UpdateProgressBar(float progress)
        {
            var value = progress * 100;
            Progress.Value = value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parser.Init(_file, City.Text);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async delegate ()
            {
                await _parser.SaveToFile();
            }));
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
                DownLoadButton.IsEnabled = true;
            }
        }
    }
}
