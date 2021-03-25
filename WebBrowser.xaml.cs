using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : Window
    {
        public WebBrowser()
        {
            InitializeComponent();
        }

        public void SetContent(string content)
        {
            Input.Text = content;
        }

        public void SetCookie(string cookies)
        {
            Cookie.Text = cookies;
        }
    }
}
