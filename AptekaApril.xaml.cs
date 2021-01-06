using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
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
        public AptekaApril()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IWebDriver driver = new ChromeDriver();
            LoginOnSite(driver, "9370886609", "Jd3oQ98P");
            SelectCity(driver, "Волгоград");
            AddProductToBasket(driver, "54098-trimedat_tabletki_200mg_30", 5);
            //driver.Quit();
        }

        private void LoginOnSite(IWebDriver driver, string user, string password)
        {
            driver.Navigate().GoToUrl("https://apteka-april.ru/login");
            IWebElement number = driver.FindElement(By.ClassName("q-edit-phone"));
            var input = number.FindElement(By.TagName("input"));
            input.Click();
            input.SendKeys(user);

            var passwordField = driver.FindElements(By.ClassName("q-edit"))
             .Where(x => x.GetProperty("placeholder").ToString()
             .Contains("Пароль"))
             .First();
            passwordField.Click();
            passwordField.SendKeys(password);

            var enterButton = driver.FindElement(By.ClassName("primary"));
            enterButton.Click();
        }

        private void SelectCity(IWebDriver driver, string city)
        {
            var selection = driver.FindElement(By.ClassName("c-select-city-desktop"));
            var button = selection.FindElement(By.TagName("button"));
            button.Click();
            var openedSelection = driver.FindElement(By.ClassName("opened"));
            var inputCity = openedSelection.FindElement(By.TagName("input"));
            inputCity.SendKeys(city);
            var firstCity = openedSelection.FindElement(By.ClassName("name"));
            firstCity.Click();
        }

        private void AddProductToBasket(IWebDriver driver, string productId, int count)
        {
            driver.Navigate().GoToUrl($"https://apteka-april.ru/product/{productId}");
            var selection = driver.FindElement(By.ClassName("buy"));
            var buyButton = selection.FindElement(By.TagName("button"));
            buyButton.Click();

            Utils.WebWait(() => driver.FindElement(By.ClassName("quantity")));
            driver.Navigate().GoToUrl("https://apteka-april.ru/basket");

            Utils.WebWait(() => driver.FindElement(By.ClassName("quantity")));

            var quantity = driver.FindElement(By.ClassName("quantity"));
            var input = quantity.FindElement(By.TagName("input"));
            input.SendKeys(Keys.Backspace);
            input.SendKeys(count.ToString());
            input.SendKeys(Keys.Enter);

            Utils.WebWait(() =>
            {
                var selectionList = driver.FindElement(By.ClassName("product-list"));
                var title = selectionList.FindElement(By.TagName("h1"));
                return title.Text == $"В корзине {count} товаров";
            });

            driver.Navigate().GoToUrl("https://apteka-april.ru/checkout");

            var pharmacies = driver.FindElements(By.ClassName("pharmacy"));
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
            }
        }
    }
}
