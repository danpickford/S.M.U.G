using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using S.M.U.G.CustomControls;
using SMUGBase;

namespace S.M.U.G
{
    /// <summary>
    /// Interaction logic for ManageYourSmug.xaml
    /// </summary>
    public partial class ManageYourSmug : Window
    {
        public int UserId { get; set; }
        public ManageYourSmug(int uid)
        {
            InitializeComponent();
            UserId = uid;
            using (var db = new SmugContexts())
            {
                var query = from b in db.Mugusers
                            where b.UserId == uid
                            select b;
                var firstOrDefault = query.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    username.Content = firstOrDefault.UserName;
                }
                else
                {
                    username.Content = "O dear I'm broken. Call tech support or something.";
                }
            }
        }

        private void LocalSettings_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mainContainer.Children.Count == 3)
            {
                mainContainer.Children.RemoveAt(2);
            }
            mainContainer.Children.Add(new LocalSettings(UserId));
        }
        private void smugmugsettings_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void schedule_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
