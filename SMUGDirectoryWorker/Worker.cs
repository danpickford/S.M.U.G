using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SMUGDirectoryWorker
{
    public class DirectoryWorker
    {
        private DateTime _lastScanTime = DateTime.Now;
        private string _rootWorkingDirecotry = "";

        public DirectoryWorker(string rootDir)
        {
            _rootWorkingDirecotry = rootDir;
        }

        public List<FileInfo> RecurseDirectories(string path)
        {
            var directories = new DirectoryInfo(path).GetDirectories();
            var locatefiles = new List<FileInfo>();
            foreach (var directory in directories)
            {
                locatefiles.AddRange(CheckFiles(directory.FullName));
                RecurseDirectories(directory.FullName);
            }
            return locatefiles;
        }

        public List<FileInfo> RecurseDirectories()
        {
           return RecurseDirectories(_rootWorkingDirecotry);
        }

        private List<FileInfo> CheckFiles(string path)
        {

            var newFiles =
                new DirectoryInfo(path).GetFiles().Where(x => x.LastWriteTime > _lastScanTime).ToList();
            foreach (var fileInfo in newFiles)
            {
                Console.WriteLine("New files detected: {0}", fileInfo.FullName);
                _lastScanTime = fileInfo.LastWriteTime;
            }
            return newFiles;
        }
        /// <summary>
        /// Method the will test for the existance of a directory. If the directory does not exist it will be created.
        /// </summary>
        /// <param name="path">The path to be checked / created.</param>
        private void checkCreateDirectory(string path)
        {
            if (Directory.Exists(path)) return;
            Console.WriteLine("Creating directory: {0}", path);
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Takes the JSON object of the albums a user has and creates a direcotry structure with it.
        /// Returns a list of added Categories and Albums.
        /// </summary>
        /// <param name="jsonrespons"></param>
        /// <param name="rootSmugMugDir"></param>
        public void CreateSmugMugAlbumStruct(string jsonrespons)
        {
            var jsonObject = JObject.Parse(jsonrespons);
            foreach (var category in jsonObject["Albums"].GroupBy(s => (string)s.SelectToken("Category.Name")).ToList())
            {
                checkCreateDirectory(string.Format("{0}\\{1}", _rootWorkingDirecotry, category.Key));
                foreach (var albums in jsonObject.SelectToken("Albums").Where(s => (string)s.SelectToken("Category.Name") == category.Key).Select(ss => ss.SelectToken("Title")).ToList())
                {
                    checkCreateDirectory(string.Format("{0}\\{1}\\{2}", _rootWorkingDirecotry, category.Key, albums));
                }
            }
        }
    }
}
