using AptekaHelper.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Dictionary<int, Page> _openedPages = new Dictionary<int, Page>();
        private List<BaseSiteParser> _parsers;
        public MainWindow()
        {
            InitializeComponent();
            _parsers = new List<BaseSiteParser>()
            {
                new AprilSiteParser(),
                new VolgofarmSiteParser()
            };

            var tabItems = _parsers.Select(x => new TabItem() { Header = x.Name, Name = x.Name }).ToList();
            foreach (var item in tabItems)
            {
                Tabs.Items.Add(item);
            }

            for (int i = 0; i < _parsers.Count; i++)
            {
                _openedPages.Add(i, new SitePage(_parsers[i], BlockElements));
            }
            PagesFrame.Navigate(_openedPages[0]);
            PagesFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selectIndex = Tabs.SelectedIndex;
            PagesFrame.Navigate(_openedPages[selectIndex]);
        }

        private void BlockElements(bool blocked)
        {
            foreach (var tab in Tabs.Items)
            {
                var t = tab as TabItem;
                t.IsEnabled = !blocked;
            }
        }
    }
}