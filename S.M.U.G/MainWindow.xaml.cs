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
using S.M.U.G.Classes;

namespace S.M.U.G
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_MouseEnter_1(object sender, MouseEventArgs e)
        {
            System.Console.WriteLine("Test");
            TextBox tb = (TextBox)sender;
            tb.Text = "";

        }

        private void Run_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            const string apiKey = "ptImcZ4sIXVNENb5inyDpeBivcP6FEgB";
            const string secret = "9ad05e770c6bce11a430fbfafae10396";

            OAuthHelper oah = new OAuthHelper(apiKey,secret);
            OAuthResponse oar = oah.AcquireRequestToken("http://api.smugmug.com/services/oauth/getRequestToken.mg", "GET");

            string oauthToken = oar["oauth_token"];
            string oauthTokenSecret = oar["oauth_token_secret"];

            smugwb.Visibility = Visibility.Visible;
            smugwb.Navigate(string.Format("http://api.smugmug.com/services/oauth/authorize.mg?oauth_token={0}&Access=Full&Permissions=Modify", oauthToken));

        }

    }
}
