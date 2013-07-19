using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SMUGBase;
using Topshelf;
using dOhAuth;

namespace SMUGcontrol
{
    class Program
    {
        public class SmugSync
        {
            const string ApiKey = "ptImcZ4sIXVNENb5inyDpeBivcP6FEgB";
            const string Secret = "9ad05e770c6bce11a430fbfafae10396";
            readonly Timer _timer;
            protected Muguser LoggedMugUser;
            public SmugSync()
            {
                Console.WriteLine("SMUG is alive... alive at {0} and all is well", DateTime.Now);
                LoggedMugUser = RetrieveMugUser();
                if (LoggedMugUser != null)
                {
                    Console.WriteLine("Directory: " + LoggedMugUser.SyncFolder);
                }
                GetAlbumStructFromSM();
                _timer = new Timer(10000) { AutoReset = true };
                _timer.Elapsed += lookAtSmug;
            }

            public void Start() { _timer.Start(); }
            public void Stop() { _timer.Stop(); }

            private void lookAtSmug(object sender, EventArgs eventArgs)
            {
                Console.WriteLine("Tick Tock Y'all");
                var lastModDir =
                    new DirectoryInfo(LoggedMugUser.SyncFolder).GetDirectories("*", SearchOption.AllDirectories)
                                               .OrderByDescending(d => d.LastWriteTime).First();
                Console.WriteLine("LastModded: " + lastModDir);


            }

            private void GetAlbumStructFromSM()
            {
                var oah = new OAuthHelper(ApiKey, Secret, LoggedMugUser.OAuthToken, LoggedMugUser.OAuthTokenSecret);
                var _params = new Dictionary<string, string>();
                var result =
                    oah.executeFunction("https://api.smugmug.com/services/api/json/1.3.0/?&method=smugmug.albums.get",
                                        "GET", _params);
                Console.WriteLine(result);

 }

            private Muguser RetrieveMugUser()
            {
                using (var db = new SmugContexts())
                {
                    var loggedUser = db.Mugusers.FirstOrDefault(x => x.LoggedUser == 1);
                    return loggedUser;
                }
            }
        }
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                                                    //1
            {
                x.Service<SmugSync>(s =>                                                            //2
                {
                    s.ConstructUsing(name => new SmugSync());                                       //3
                    s.WhenStarted(ss => ss.Start());                                                //4
                    s.WhenStopped(ss => ss.Stop());                                                 //5
                });
                x.RunAsLocalSystem();                                                               //6

                x.SetDescription("S.M.U.G service does the heavy lifting for S.M.U.G");             //7
                x.SetDisplayName("SMUGService");                                                    //8
                x.SetServiceName("SMUGService");                                                    //9
            });
        }
    }
}
