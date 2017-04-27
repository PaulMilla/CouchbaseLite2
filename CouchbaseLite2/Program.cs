using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Couchbase.Lite;
using Couchbase.Lite.Util;
using Newtonsoft.Json;

namespace CouchbaseLite2
{
    class Program
    {
        private const string DbName = "app2";

        private const string OtherDbName = "app1";
        private const string OtherDbIp = "127.0.0.1";
        private const ushort OtherDbPort = 59840;

        private static readonly Uri OtherUrl = new Uri($"http://{OtherDbIp}:{OtherDbPort}/{OtherDbName}");

        private static Manager manager;
        private static Database db;

        private static void Main()
        {
            SetupLogger();

/*
            var directoryPath = $"D:\\{Process.GetCurrentProcess().ProcessName}";
            manager = new Manager(
                Directory.CreateDirectory(directoryPath),
                ManagerOptions.Default);
*/
            manager = Manager.SharedInstance;

            db = manager.GetDatabase(DbName);

            //Peer-to-peer replication doesn't support web sockets
            var pull = db.CreatePullReplication(OtherUrl);
            pull.ReplicationOptions.UseWebSocket = false;
            pull.Continuous = true;
            pull.Start();

            var push = db.CreatePushReplication(OtherUrl);
            push.ReplicationOptions.UseWebSocket = false;
            push.Continuous = true;
            push.Start();

            Console.WriteLine("Press ESC to stop");

            var shutdownTokenSource = new CancellationTokenSource();
            HandleCommands(shutdownTokenSource);
        }

        private static string Tag => "MAIN (Main)";

        private static void SetupLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
            Log.SetLogger(new Log4NetLogger());

            //Log.Level = Log.LogLevel.Debug;
            //Log.Domains.All.Level = Log.LogLevel.Debug;
            Log.Domains.Sync.Level = Log.LogLevel.Debug;
        }

        private static void HandleCommands(CancellationTokenSource shutdownTokenSource)
        {
            while (!shutdownTokenSource.IsCancellationRequested)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Spacebar:
                        PrintDocuments();
                        break;

                    case ConsoleKey.Enter:
                        CreateDocument();
                        break;

                    case ConsoleKey.NumPad0:
                        DeleteDocument();
                        break;

                    case ConsoleKey.Escape:
                        shutdownTokenSource.Cancel();
                        return;
                }
            }
        }

        private static void PrintDocuments()
        {
            var sb = new StringBuilder("Documents:" + Environment.NewLine);
            var allDocumentsQuery = db.CreateAllDocumentsQuery();
            var rows = allDocumentsQuery.Run();

            var count = 0;
            foreach (var row in rows)
            {
                sb.AppendLine($" {++count}) {JsonConvert.SerializeObject(row.Document.Properties)}");
            }

            Log.I(Tag, sb.ToString());
        }

        private static void CreateDocument()
        {
            var doc = db.CreateDocument();
            doc.PutProperties(
                new Dictionary<string, object>
                {
                    {"time", DateTime.Now.ToString("G")}
                });

            Log.I(Tag, $"Created {JsonConvert.SerializeObject(doc.Properties)}");
        }

        private static void DeleteDocument()
        {
            var allDocumentsQuery = db.CreateAllDocumentsQuery();
            var startKeyDocId = allDocumentsQuery.Run().FirstOrDefault()?.DocumentId;

            if (startKeyDocId == null)
                return;

            var document = db.GetDocument(startKeyDocId);
            Log.I(Tag, $"Deleted {JsonConvert.SerializeObject(document.Properties)}");
            document.Delete();
        }
    }
}
