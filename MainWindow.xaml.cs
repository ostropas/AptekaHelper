using DesctopAptekaHelper.Parsers;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DesctopAptekaHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Dictionary<int, Page> _openedPages = new Dictionary<int, Page>();
        public MainWindow()
        {
            InitializeComponent();
            _openedPages[0] = new AptekaApril(new AprilSiteParser());
            _openedPages[1] = new Volgofarm();
            PagesFrame.Navigate(_openedPages[0]);
            PagesFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selectIndex = Tabs.SelectedIndex;
            PagesFrame.Navigate(_openedPages[selectIndex]);
        }
    }
}