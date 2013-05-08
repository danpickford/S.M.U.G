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
using S.M.U.G.Classes.EF;
using S.M.U.G.CustomControls;

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
                    username.Content = "Err dude i'm broken. Call tech support or something.";
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
