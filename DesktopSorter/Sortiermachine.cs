using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace DesktopSorter
{
    class Sortiermachine
    {
        public DataTable GetTable(string query,ref SQLiteDataAdapter da)
        {
            //Führt eine query in der Datenbank aus und gibt die Tabelle zurück.
            using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                var tab = new DataTable();
                con.Open();
                using (da = new SQLiteDataAdapter(query, con))
                {
                    da.AcceptChangesDuringFill = false;
                    da.Fill(tab);
                }
                con.Close();
                return tab;
            }
        }

        public DataTable GetTable(string query)
        {
            //Führt eine query in der Datenbank aus und gibt die Tabelle zurück.
            using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                var tab = new DataTable();
                con.Open();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, con))
                {
                    da.AcceptChangesDuringFill = false;
                    da.Fill(tab);
                }
                con.Close();
                return tab;
            }
        }

        public void InsertData(string query)
        {
            //Führt eine query in der Datenbank aus und gibt die Tabelle zurück.
            using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                SQLiteCommand com = new SQLiteCommand(query, con);
                com.ExecuteNonQuery();
            }
        }

        public int Sort(string sortpath, System.Windows.Controls.ProgressBar prograssbar)
        {
            //Methode, die die Files in einen Angegeben pfad sortiert.

            int status = 0;

            //Testweise setzen von progressbar status:
            prograssbar.Value = 100;


            //Files von destination entgegennehmen und whitelist checken
            string[] fileList = Directory.GetFiles(sortpath);
            var whitelist = GetTable("SELECT * FROM whitelist");
            var sortList = new List<string>();

            foreach (var file in fileList)
            {
                bool toList = true;
                foreach (var whiteRow in whitelist.AsEnumerable())
                {
                    if (file.Split('\\').Last() == whiteRow.ItemArray.First().ToString())
                    {
                        toList = false;
                    }
                }
                if (toList) { sortList.Add(file); }
            }

            //Files Sortieren
            var dest = GetTable("SELECT * FROM Destinations");

            foreach (DataRow dataRow in dest.Rows)
            {
                foreach (string file in sortList.AsEnumerable().Where(p => p.Split('.').Last() == dataRow.ItemArray.Last().ToString()))
                {
                    File.Move(file, dataRow.ItemArray.First().ToString() + '\\' + file.Split('\\').Last());
                }
            }

            return status;
        }
    }
}
