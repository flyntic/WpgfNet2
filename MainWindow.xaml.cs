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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace WpfAppNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;
        public string _respond;
        public MainWindow()
        {
            InitializeComponent();

        }

        private async void LoadPage(object sender, RoutedEventArgs e)
        {
            using (client = new HttpClient())
                try
                { 
                    string HttpAddress = @"https://proglib.io/vacancies/all?workType=all&workPlace=all&experience=&salaryFrom=&page=";
                    var respond = client.GetAsync(HttpAddress+"1");
                    
                    _respond = respond.Result.Content.ReadAsStringAsync().Result;

                    var rx = new Regex("data-total=\"(.*)\"");
                    var match = rx.Match(_respond);

                    int count = int.Parse(match.Groups[1].Value);//_respond;
                    pageTextBox.Text = string.Empty;

                    string[] pages = new string[count];
                    for (int i = 1; i <= count; i++)
                    {
                        pages[i - 1] = HttpAddress + i.ToString();
                    }
                    
                    rx = new Regex("class=\"preview-card__title\".*\\s*itemprop=\"title\">(.+)<\\/h2>\\s*.*\\s*.*\\s*<div itemprop=\"description\">.*<\\/div>\\s*<div itemprop=\"datePosted\">(.+)<\\/div>");

                    var tasks = pages.Select(page => client.GetStringAsync(page));
                    string[] pagesContent = await Task.WhenAll(tasks);
                    lock (pages)
                    {
                        pagesContent.ToList().ForEach( page=>
                             rx.Matches(page).ToList().ForEach(u => pageTextBox.Text += u.Groups[1].Value + " " + u.Groups[2].Value + "\n"));

                    }
                    
                }
                catch
                {
                    MessageBox.Show("Неудачная загрузка");
                }
        }
       
    }
}
