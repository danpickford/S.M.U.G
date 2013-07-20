using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SMUGBase;
using SMUGDirectoryWorker;
using Topshelf;
using dOhAuth;
using Newtonsoft.Json.Linq;
namespace SMUGcontrol
{
    class Program
    {
        public class SmugSync
        {
            const string ApiKey = "ptImcZ4sIXVNENb5inyDpeBivcP6FEgB";
            const string Secret = "9ad05e770c6bce11a430fbfafae10396";
            private DirectoryWorker dw;
            readonly Timer _timer;
            protected Muguser LoggedMugUser;
            public SmugSync()
            {
                Console.WriteLine("SMUG is alive... alive at {0} and all is well", DateTime.Now);
                using (var db = new SmugContexts())
                {
                    LoggedMugUser = db.RetrieveMugUser();
                }
                Console.WriteLine("Looking up Users SmugDirectory");
                if (LoggedMugUser == null) Stop();
                if (LoggedMugUser.SyncFolder == null) Stop();
                var albumsFromSmugmug = RetrieveAlbumsFromSmugMug();
                dw = new DirectoryWorker(string.Format("{0}\\SMUG", LoggedMugUser.SyncFolder));
                dw.CreateSmugMugAlbumStruct(albumsFromSmugmug);
                using (var db = new SmugContexts())
                {
                    db.insertCategoriesAndAlbumsFromJSON(albumsFromSmugmug);
                }

                _timer = new Timer(10000) { AutoReset = true };
                _timer.Elapsed += scanForPhotographs;

            }

            public void Start() { _timer.Start(); }
            public void Stop() { _timer.Stop(); }

            private void scanForPhotographs(object sender, EventArgs eventArgs)
            {
                Console.WriteLine("Tick Tock Y'all");
                var newFiles = dw.RecurseDirectories();
                if (newFiles.Count > 0)
                {
                    
                }
            }
            /// <summary>
            /// Connect to smugmug and retrieve a list of Albums.
            /// </summary>
            private string RetrieveAlbumsFromSmugMug()
            {
                var oah = new OAuthHelper(ApiKey, Secret, LoggedMugUser.OAuthToken, LoggedMugUser.OAuthTokenSecret);
                var _params = new Dictionary<string, string>();

                var result =
                    oah.executeFunction("https://api.smugmug.com/services/api/json/1.3.0/?&method=smugmug.albums.get",
                                        "GET", _params);
                return result;
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
