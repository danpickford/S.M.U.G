using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using S.M.U.G.Classes;
using System.Data.Entity;
using S.M.U.G.Classes.EF;
using S.M.U.G.CustomControls;

namespace S.M.U.G
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ApiKey = "ptImcZ4sIXVNENb5inyDpeBivcP6FEgB";
        const string Secret = "9ad05e770c6bce11a430fbfafae10396";
        private int registered = 0;
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new SmugContexts())
            {
                var query = from b in db.Mugusers
                            orderby b.UserName
                            select b;

                if (query.FirstOrDefault() != null)
                {
                    Instructions.Text = String.Format("Welcome back {0} click login to manage you S.M.U.G. If you would like to switch or create a new user select from the options below.", query.FirstOrDefault().UserName);
                    Username.Visibility = Visibility.Hidden;
                    registered = query.FirstOrDefault().UserId;
                    changeUser.Visibility = Visibility.Visible;
                    newUser.Visibility = Visibility.Visible;
                }
            }
        }

        private void LoginClick(object sender, MouseButtonEventArgs e)
        {
            allUsers.Visibility = Visibility.Hidden;
            if (registered > 0)
            {
                new ManageYourSmug(registered).Show();
                Close();
            }
            if (Username.Text == "Enter a username here" || Username.Text == "")
            {
                ErrorBox.Visibility = Visibility.Visible;
                ErrorBox.Text =
                    "Whoah there you need to enter a username so that we can store your settings. It's a one time thing man, promise.";

            }
            else
            {
                if (!theUserExists()) { 
                var oah = new OAuthHelper(ApiKey, Secret);
                var oar = oah.AcquireRequestToken("http://api.smugmug.com/services/oauth/getRequestToken.mg",
                                                            "GET");

                string oauthToken = oar["oauth_token"];
                string oauthTokenSecret = oar["oauth_token_secret"];

                smugwb.Visibility = Visibility.Visible;
                smugwb.Navigate(
                    string.Format(
                        "http://api.smugmug.com/services/oauth/authorize.mg?oauth_token={0}&Access=Full&Permissions=Modify",
                        oauthToken));

                StartListener();
                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
                }
            }
        }

        private  bool theUserExists()
        {
            var retVal = false;
             using (var db = new SmugContexts())
                {
                    var query = from b in db.Mugusers
                                where b.UserName == Username.Text
                                select b;
                    if (query.FirstOrDefault() != null)
                    {
                        ErrorBox.Text =
                            String.Format(
                                "Sorry this {0} is already in use on your computer. Please chose another Username.", Username.Text);
                        ErrorBox.Visibility = Visibility.Visible;
                        retVal = true;
                    }
                }
            return retVal;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (SingleTonKen.Instance.AccessTokenResponse != null)
            {
                var newUserID = 0;
                using (var db = new SmugContexts())
                {
                    var newUser = new Muguser { UserName = Username.Text, OAuthToken = SingleTonKen.Instance.AccessTokenResponse};
                    db.Mugusers.Add(newUser);
                    db.SaveChanges();
                    newUserID = newUser.UserId;
                }
                var dt = (DispatcherTimer) sender;
                dt.Stop();
                new ManageYourSmug(newUserID).Show();
                Close();
            }
        }

        private void Smugwb_OnNavigated(object sender, NavigationEventArgs e)
        {
            Console.WriteLine(e.Uri.ToString());
        }
        
        protected void HttpListener_Callback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;

            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            SingleTonKen.Instance.AccessTokenResponse = request.QueryString["oauth_token"];

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            response.Close();
            listener.Close();
            
        }

        private void StartListener()
        {
            string listenerAddress = string.Format("http://localhost:3749/");
            // Setup the httplistener class
            var listener = new HttpListener();
            listener.Prefixes.Add(listenerAddress);
            listener.Start();
            // Set our callback method
            IAsyncResult asyncResult = listener.BeginGetContext(HttpListener_Callback, listener);
        }

        private void Username_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.Text = "";
        }

        private void ChangeUserClick(object sender, MouseButtonEventArgs e)
        {
            allUsers.Children.Clear();
            allUsers.Visibility = Visibility.Visible;
            newUser.Visibility = Visibility.Visible;
            using (var db = new SmugContexts())
            {
                var query = from b in db.Mugusers
                            orderby b.UserName
                            select b;
                foreach (var muguser in query)
                {
                    allUsers.Children.Add(new UserTile(muguser.UserName));
                }
            }
        }

        private void NewUserClick(object sender, MouseButtonEventArgs e)
        {
            Instructions.Text =
                "Smart Multiple Upload Gadget requires access to your SmugMug account. This is so that the application is able to Synchronise it's self with your account. Please enter a username to associate with S.M.U.G and click login. You will be taken to the SmugMug site for authentication.";
            Username.Visibility = Visibility.Visible;
            changeUser.Visibility = Visibility.Visible;
            newUser.Visibility = Visibility.Hidden;
            allUsers.Visibility = Visibility.Hidden;
            registered = 0;
        }
    }
}
