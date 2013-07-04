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
using SMUGBase;

namespace S.M.U.G.CustomControls
{
    /// <summary>
    /// Interaction logic for UserTile.xaml
    /// </summary>
    public partial class UserTile : UserControl
    {
        public string Username { get; set; }
        public event EventHandler UserChanged;

        protected virtual void OnUserChanged()
        {
            EventHandler handler = UserChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public UserTile(string u)
        {
            Username = u;
            InitializeComponent();
            UserNameDisplay.Content = Username;
        }

        private void UserNameDisplay_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            using (var db = new SmugContexts())
            {
                db.Mugusers.ToList().ForEach(x => x.LoggedUser = 0);
                var firstOrDefault = db.Mugusers.FirstOrDefault(x => x.UserName == Username);
                if (firstOrDefault != null)
                    firstOrDefault.LoggedUser = 1;
                db.SaveChanges();
                OnUserChanged();
            }
        }
    }
}
