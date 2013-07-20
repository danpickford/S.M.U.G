using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
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
        private UiAnimation animation = new UiAnimation();
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
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (animation.AnimationLengthSeconds > 0)
            {
                var o = animation.ObjectToFocus;
                var p = new Dictionary<string, object>();
                foreach (var newProperties in animation.NewProperties)
                {
                    if (getProperty(o, newProperties.Key) != newProperties.Value)
                    {
                        p.Add(newProperties.Key, getProperty(o, newProperties.Key));
                        setProperty(o, newProperties.Key, newProperties.Value);
                    }
                }
                if (p.Count > 0)
                {
                    animation.OriginalProperties = p;
                }
                animation.AnimationLengthSeconds -= 1;
            }
            if (animation.AnimationLengthSeconds == 0 && animation.ObjectToFocus != null)
            {
                foreach (var originalProperty in animation.OriginalProperties)
                {
                    setProperty(animation.ObjectToFocus, originalProperty.Key, originalProperty.Value);
                }
                animation = new UiAnimation();
            }
        }

        private void setProperty(object Object, string property, object value)
        {
            var p = Object.GetType().GetProperty(property);
            p.SetValue(Object, value, null);
        }

        private object getProperty(object Object, string property)
        {
            var p = Object.GetType().GetProperty(property);
            return p.GetValue(Object, null);
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
                var converter = new BrushConverter();

                animation.AnimationLengthSeconds = 2;
                animation.ObjectToFocus = Save;
                animation.NewProperties = new Dictionary<string, Object>
                    {
                        { "Content", "Saved" },
                        {"Background",converter.ConvertFromString("#FFAAFF00")}
                    };
            }
        }

        private class UiAnimation
        {
            public int AnimationLengthSeconds { get; set; }
            public object ObjectToFocus { get; set; }
            public Dictionary<String, Object> OriginalProperties { get; set; }
            public Dictionary<String, Object> NewProperties { get; set; }
        }

    }
}
