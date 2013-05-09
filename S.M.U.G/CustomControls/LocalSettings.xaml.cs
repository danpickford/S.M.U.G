using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using SMUGBase;
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
