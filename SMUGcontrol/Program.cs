using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;

namespace SMUGcontrol
{
    class Program
    {
        public class SmugSync
        {
            readonly Timer _timer;
            public SmugSync()
            {
                Console.WriteLine("SMUG is alive... alive at {0} and all is well", DateTime.Now);
                _timer = new Timer(1000) { AutoReset = true };
                _timer.Elapsed += lookAtSmug;
            }

            public void Start() { _timer.Start(); }
            public void Stop() { _timer.Stop(); }
            
            private void lookAtSmug(object sender, EventArgs eventArgs)
            {
                Console.WriteLine("Tick Tock Y'all");

            }
        }
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                                                    //1
            {
                x.Service<SmugSync>(s =>                                                            //2
                {
                    s.ConstructUsing(name => new SmugSync());                                       //3
                    s.WhenStarted(tc => tc.Start());                                                //4
                    s.WhenStopped(tc => tc.Stop());                                                 //5
                });
                x.RunAsLocalSystem();                                                               //6

                x.SetDescription("S.M.U.G service does the heavy lifting for S.M.U.G");             //7
                x.SetDisplayName("SMUGService");                                                    //8
                x.SetServiceName("SMUGService");                                                    //9
            });
        }
    }
}
