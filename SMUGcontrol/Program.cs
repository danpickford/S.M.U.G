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
                                               .OrderByDescending(d => d.LastWriteTime)
                                               .DefaultIfEmpty(new DirectoryInfo(LoggedMugUser.SyncFolder)).First();
                Console.WriteLine("LastModded: " + lastModDir);


            }

            private void GetAlbumStructFromSM()
            {
                var oah = new OAuthHelper(ApiKey, Secret, LoggedMugUser.OAuthToken, LoggedMugUser.OAuthTokenSecret);
                
                Console.WriteLine(String.Format(
                    "https://secure.smugmug.com/services/api/json/1.3.0/?method=smugmug.albums.get&oauth_consumer_key={0}&" +
                    "oauth_signature={1}&oauth_signature_method={2}&oauth_timestamp={3}&oauth_token={4}",
                    ApiKey, Secret, "HMAC-SHA1", oah.GenerateTimeStamp(), LoggedMugUser.OAuthToken)
                    );
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
