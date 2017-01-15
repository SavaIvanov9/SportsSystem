namespace SportsSystem.Importer.Core
{
    using System;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using SportsSystem.Importer.Seeding;
    using SportSystem.Data;
    using SportSystem.Models;

    public sealed class Engine
    {
        private static volatile Engine _instance;
        private static readonly object Lock = new object();
        private const string RequestLink = "http://vitalbet.net/sportxml";
        private const string DataFolder = "WebData";
        private readonly string _dataFile = $"{DateTime.Now:yyyy-MM-dd_hh;mm;ss}.xml";
        private SportSystemData _db;

        private Engine()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SportSystemDbContext>());
            _db = new SportSystemData();
        }

        public static Engine Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Engine();
                        }
                    }
                }

                return _instance;
            }
        }

        public void Start()
        {
            var data = GetWebData(RequestLink);

            CheckDirectory(DataFolder);

            string path = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), 
                $@"{DataFolder}\{_dataFile}");

            CreateDataFile(path, data);

            MultiThreadImport(data);
        }

        private void MultiThreadImport(string data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            var root = doc.DocumentElement;

            var sports = root.GetElementsByTagName("Sport");

            Console.WriteLine("Started seeding data...");

            //int addedElements = 0;

            var seedManager = new SeedManager();
            seedManager.SeedData(sports, typeof(Sport));
            
            _db.SaveChanges();
            Console.WriteLine("Seeding compleated.");
            Console.WriteLine($"{_db.Sports.All().ToList().Count} sports added");
            Console.WriteLine($"{_db.Events.All().ToList().Count} events added");
            Console.WriteLine($"{_db.Matches.All().ToList().Count} matches added");
            Console.WriteLine($"{_db.Bets.All().ToList().Count} bets added");
            Console.WriteLine($"{_db.Odds.All().ToList().Count} odds added");
        }

        private string GetWebData(string link)
        {
            var request = WebRequest.Create(link) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "GET";

            var response = request.GetResponse();
            var responseReader = new StreamReader(response.GetResponseStream());

            var data = responseReader.ReadToEnd();
            responseReader.Close();

            return data;
        }

        private void CheckDirectory(string folderName)
        {
            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pathString = Path.Combine(dirName, folderName);
            if (!Directory.Exists(pathString))
            {
                Directory.CreateDirectory(pathString);
            }
        }

        private void CreateDataFile(string path, string content)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter file = File.CreateText(path))
                {
                    file.Write(content);
                }
            }
        }
    }
}
