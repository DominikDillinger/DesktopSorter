using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace DesktopSorter
{
    class Sortiermachine
    {
        public DataTable GetTabble(string query)
        {
            //Führt eine query in der Datenbank aus und gibt die Tabelle zurück.
            List<string> ImportedFiles = new List<string>();
            using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                using (var conn = new SQLiteConnection(@"Data Source=Datenbank.db"))
                {
                    var tab = new DataTable();
                    conn.Open();
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn))
                    {
                        da.AcceptChangesDuringFill = false;
                        da.Fill(tab);
                    }
                    conn.Close();
                    return tab;
                }
            }
        }

        public void Sort(string sortpath)
        {
            //Methode, die die Files in einen Angegeben pfad sortiert.

            //Files von destination entgegennehmen und whitelist checken
            string[] fileList = Directory.GetFiles(sortpath);
            var whitelist = GetTabble("SELECT * FROM whitelist");
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
            var dest = GetTabble("SELECT * FROM Destinations");

            foreach (DataRow dataRow in dest.Rows)
            {
                foreach (string file in sortList.AsEnumerable().Where(p => p.Split('.').Last() == dataRow.ItemArray.Last().ToString()))
                {
                    File.Move(file, dataRow.ItemArray.First().ToString() + '\\' + file.Split('\\').Last());
                }
            }
        }
    }
}
