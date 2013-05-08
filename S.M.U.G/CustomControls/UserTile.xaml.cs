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

namespace S.M.U.G.CustomControls
{
    /// <summary>
    /// Interaction logic for UserTile.xaml
    /// </summary>
    public partial class UserTile : UserControl
    {
        public string Username { get; set; }
        public UserTile(string u)
        {
            Username = u;
            InitializeComponent();
            UserNameDisplay.Content = Username;
        }
        
    }
}
