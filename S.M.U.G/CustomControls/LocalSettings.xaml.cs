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
using System.Windows.Forms;
using S.M.U.G.Classes.EF;
using UserControl = System.Windows.Controls.UserControl;

namespace S.M.U.G.CustomControls
{
    /// <summary>
    /// Interaction logic for LocalSettings.xaml
    /// </summary>
    public partial class LocalSettings : UserControl
    {
        public int Userid { get; set; }
        public LocalSettings(int uid)
        {
            InitializeComponent();
            Userid = uid;
            using (var db = new SmugContexts())
            {
                var query = from b in db.Mugusers
                            where b.UserId == uid
                            select b;
                var firstOrDefault = query.FirstOrDefault();
                if (firstOrDefault != null) syncFolder.Text = firstOrDefault.SyncFolder;
            }
        }

        private void searchForFolder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var browse = new FolderBrowserDialog();
            browse.RootFolder = Environment.SpecialFolder.MyComputer;
            browse.ShowDialog();
            syncFolder.Text = browse.SelectedPath;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new SmugContexts())
            {
                db.Mugusers.FirstOrDefault(x => x.UserId == Userid).SyncFolder = syncFolder.Text;
                db.SaveChanges();
            }
        }

    }
}
