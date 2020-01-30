using System;
using System.Diagnostics;
using Npgsql;

namespace EventViewerAndWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            Collector.ReadEvents();
            //Collector.InsertItem();
            //PostgreSQL postgre = PostgreSQL.getInstance();
            //postgre.InsertItem(new LogItem());
            
        }
    }

    class Collector
    {
        public static void ReadEvents()
        {
            string journalName = "System";
            EventLog eventLog = new EventLog();
            eventLog.Log = journalName;

            PostgreSQL postgreSQL = PostgreSQL.getInstance();

            foreach(EventLogEntry log in eventLog.Entries)
            {
                LogItem logItem = new LogItem(log.InstanceId, 2, log.Source, log.Message, log.UserName, log.MachineName, 1, log.TimeGenerated);
                postgreSQL.InsertItem(logItem);
                //Console.WriteLine("{0}\n", log.Message);
            }
        }
    }

    class LogItem
    {
        private long event_id;
        private int type;
        private string source;
        private string descr;
        private string user;
        private string computer;
        private int log_level;
        private DateTime date;

        public int Type { get => type; set => type = value; }
        public string Source { get => source; set => source = value; }
        public string Descr { get => descr; set => descr = value; }
        public string User { get => user; set => user = value; }
        public string Computer { get => computer; set => computer = value; }
        public int Log_level { get => log_level; set => log_level = value; }
        public DateTime Date { get => date; set => date = value; }
        public long Event_id { get => event_id; set => event_id = value; }

        public LogItem(long event_id, int type, string source, string descr, string user, string computer, int log_level, DateTime date)
        {
            this.Event_id = event_id;
            this.Type = type;
            this.Source = source;
            this.Descr = descr.Replace("'","''");
            this.User = user;
            this.Computer = computer;
            this.Log_level = log_level;
            this.Date = date;
        }
        public LogItem() { }

        private void InsertItem()
        {

        }
    }

    class PostgreSQL
    {
        private static PostgreSQL instance;
        private NpgsqlConnection connection;

        private PostgreSQL()
        {
            var connString = "Host=localhost;Username=postgres;Password=ThisIsThat!123;Database=tjparserdb";
            NpgsqlConnection con = new NpgsqlConnection(connString);

            this.connection = con;
            this.connection.Open();
        }

        public static PostgreSQL getInstance()
        {
            if (instance == null)
                instance = new PostgreSQL();
            return instance;
        }

        public void InsertItem(LogItem logItem)
        {
            

            //string sql = "SELECT version()";

            string sql = "INSERT INTO public.windows_log_items(log_type, log_event_id, log_source, log_descr, log_user, log_computer, log_level) VALUES (" + logItem.Type + "," + logItem.Event_id + ",'" + logItem.Source + "','" + logItem.Descr + "','" + logItem.User + "','" + logItem.Computer + "'," + logItem.Log_level + ");";
            var cmd = connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            //var version = cmd.ExecuteScalar().ToString();
            //Console.WriteLine($"PostgreSQL version: {version}");
        }
    }


}
