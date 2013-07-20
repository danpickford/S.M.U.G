using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace S.M.U.G.Classes
{
    class UiAnimation
    {
        public int AnimationLengthSeconds { get; set; }
        public object ObjectToFocus { get; set; }
        public Dictionary<String, Object> OriginalProperties { get; set; }
        public Dictionary<String, Object> NewProperties { get; set; }
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        
        public UiAnimation()
        {
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        public UiAnimation(int animationLengthSeconds, object objectToFocus,
                           Dictionary<string, Object> newProperties)
        {
            AnimationLengthSeconds = animationLengthSeconds;
            ObjectToFocus = objectToFocus;
            OriginalProperties = new Dictionary<string, object>();
            NewProperties = newProperties;
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        public void StartTimer()
        {
            dispatcherTimer.Start();
        }
        public void StopTimer()
        {
            dispatcherTimer.Stop();
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (AnimationLengthSeconds > 0)
            {
                var o = ObjectToFocus;
                var p = new Dictionary<string, object>();
                foreach (var newProperties in this.NewProperties)
                {
                    if (getProperty(o, newProperties.Key) != newProperties.Value)
                    {
                        p.Add(newProperties.Key, getProperty(o, newProperties.Key));
                        setProperty(o, newProperties.Key, newProperties.Value);
                    }
                }
                if (p.Count > 0)
                {
                    OriginalProperties = p;
                }
                AnimationLengthSeconds -= 1;
            }
            if (AnimationLengthSeconds == 0 && ObjectToFocus != null)
            {
                foreach (var originalProperty in OriginalProperties)
                {
                    setProperty(ObjectToFocus, originalProperty.Key, originalProperty.Value);
                }
                StopTimer();
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
    }
}
