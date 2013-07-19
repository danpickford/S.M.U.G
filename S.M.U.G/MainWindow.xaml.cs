using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using S.M.U.G.CustomControls;
using SMUGBase;
using dOhAuth;

namespace S.M.U.G
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ApiKey = "ptImcZ4sIXVNENb5inyDpeBivcP6FEgB";
        const string Secret = "9ad05e770c6bce11a430fbfafae10396";
        private int _registered = 0;
        private string _curoauthTokenSecret = "";
        private string _curoauthToken = "";
        public MainWindow()
        {
            InitializeComponent();
            setupHomeScreen();
        }

        private void LoginClick(object sender, MouseButtonEventArgs e)
        {
            allUsers.Visibility = Visibility.Hidden;
            if (_registered > 0)
            {
                new ManageYourSmug(_registered).Show();
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
                    
                _curoauthToken = oar["oauth_token"];
                _curoauthTokenSecret = oar["oauth_token_secret"];
                 
                smugwb.Visibility = Visibility.Visible;
                smugwb.Navigate(
                    string.Format(
                        "http://api.smugmug.com/services/oauth/authorize.mg?oauth_token={0}&Access=Full&Permissions=Modify",
                        _curoauthToken));

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
                var oah = new OAuthHelper(ApiKey, Secret, _curoauthToken, _curoauthTokenSecret);
                var oar = oah.AcquireAccessToken("http://api.smugmug.com/services/oauth/getAccessToken.mg", "GET",
                                                 _curoauthTokenSecret);

                int newUserId;
                var aUser = new Muguser { UserName = Username.Text, OAuthToken = oar["oauth_token"], 
                                          AccessToken = SingleTonKen.Instance.AccessTokenResponse,
                                          LoggedUser = 1,
                                          OAuthTokenSecret = oar["oauth_token_secret"]
                };
                using (var db = new SmugContexts())
                {
                    db.Mugusers.ToList().ForEach(x => x.LoggedUser = 0);
                    db.Mugusers.Add(aUser);
                    db.SaveChanges();
                    newUserId = aUser.UserId;
                }
                var dt = (DispatcherTimer) sender;
                dt.Stop();
                new ManageYourSmug(newUserId).Show();
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

            Console.WriteLine();

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
                    var this_tile = new UserTile(muguser.UserName);
                    this_tile.UserChanged += UserChangedEvent;
                    allUsers.Children.Add(this_tile);
                }
            }
        }
       
        private void setupHomeScreen()
        {
            allUsers.Children.Clear();
            allUsers.Visibility = Visibility.Hidden;
            using (var db = new SmugContexts())
            {
                var query = from b in db.Mugusers
                            where b.LoggedUser == 1
                            select b;

                if (query.FirstOrDefault() != null)
                {
                    Instructions.Text = String.Format("Welcome back {0} click login to manage you S.M.U.G. If you would like to switch or create a new user select from the options below.", query.FirstOrDefault().UserName);
                    Username.Visibility = Visibility.Hidden;
                    _registered = query.FirstOrDefault().UserId;
                    changeUser.Visibility = Visibility.Visible;
                    newUser.Visibility = Visibility.Visible;
                }
            }
        }
       
        public void UserChangedEvent(object sender, EventArgs e)
        {
            setupHomeScreen();
        }
        
        private void NewUserClick(object sender, MouseButtonEventArgs e)
        {
            Instructions.Text =
                "Smart Multiple Upload Gadget requires access to your SmugMug account. This is so that the application is able to Synchronise it's self with your account. Please enter a username to associate with S.M.U.G and click login. You will be taken to the SmugMug site for authentication.";
            Username.Visibility = Visibility.Visible;
            changeUser.Visibility = Visibility.Visible;
            newUser.Visibility = Visibility.Hidden;
            allUsers.Visibility = Visibility.Hidden;
            _registered = 0;
        }

    }
}
