using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System;

namespace DesktopSorter
{
    class Sortiermachine
    {
        private void CheckDatabase()
        {
            //Methode, die checkt, ob die Datenbank vorhanden ist und wenn nicht eine leere erstellt.
            Console.WriteLine(!File.Exists("Datenbank.db"));
            if (!File.Exists("Datenbank.db"))
            {
                //File erstellen
                SQLiteConnection.CreateFile("Datenbank.db");
                using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
                {
                    con.Open();
                    string query = "create table Destinations (Path TEXT NOT NULL, Type TEXT NOT NULL, PRIMARY KEY(Path,Type))";
                    var com = new SQLiteCommand(query, con);
                    com.ExecuteNonQuery();
                    query = "create table Whitelist (Name TEXT NOT NULL, PRIMARY KEY(Name))";
                    com = new SQLiteCommand(query, con);
                    com.ExecuteNonQuery();
                    con.Close();
                }
            }

            return;
        }

        public DataTable GetTable(string query, ref SQLiteConnection con, ref SQLiteDataAdapter da)
        {
            //Erzeugt Connection, Dataadapter und gibt die Tabelle aus

            //Datenbank Checken
            CheckDatabase();

            using (con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                var tab = new DataTable();
                con.Open();
                using (da = new SQLiteDataAdapter(query, con))
                {
                    da.Fill(tab);
                }
                con.Close();
                return tab;
            }
        }

        public DataTable GetTable(string query)
        {
            //Führt eine query in der Datenbank aus und gibt die Tabelle zurück.

            //Datenbank Checken
            CheckDatabase();

            using (var con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                var tab = new DataTable();
                con.Open();
                using (var da = new SQLiteDataAdapter(query, con))
                {
                    da.Fill(tab);
                }
                con.Close();
                return tab;
            }
        }

        public void SaveData(DataTable table, string tableNameInDatabank, ref SQLiteConnection con, ref SQLiteDataAdapter da)
        {
            //Methode, die ein Datagrid einliest und abspeichert.

            //Datenbank Checken
            CheckDatabase();

            //Speichert die Tabelle ab
            using (con = new SQLiteConnection(@"Data Source=Datenbank.db"))
            {
                con.Open();
                using (da = new SQLiteDataAdapter("SELECT * FROM " + tableNameInDatabank, con))
                {
                    SQLiteCommandBuilder build = new SQLiteCommandBuilder(da);
                    da.Update(table);
                }
                con.Close();
            }
        }

        public string Sort(string sortpath, System.Windows.Controls.ProgressBar prograssbar, System.Windows.Controls.TextBlock progressbartext)
        {
            //Methode, die die Files in einen Angegeben pfad sortiert.

            //Datenbank Checken
            CheckDatabase();

            string message = "Sorting process successful!";

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

            //Bestimmen der Anzahl von der zu sortierenden Dateien

            int filecount = 0;
            foreach (DataRow dataRow in dest.Rows)
            {
                foreach (string file in sortList.AsEnumerable().Where(p => p.Split('.').Last() == dataRow.ItemArray.Last().ToString()))
                {
                    filecount++;
                }
            }

            //Sind Dateien vorhanden??
            if (filecount != 0)
            {
                //Falls dateien vorhanden sind
                prograssbar.Maximum = filecount;


                foreach (DataRow dataRow in dest.Rows)
                {
                    foreach (string file in sortList.AsEnumerable().Where(p => p.Split('.').Last() == dataRow.ItemArray.Last().ToString()))
                    {
                        // Try Move inklusive Fehlerbehandlung
                        try
                        {
                            //Verschieben der Dateien
                            File.Move(file, dataRow.ItemArray.First().ToString() + '\\' + file.Split('\\').Last());

                            //Fortschritt anzeigen
                            prograssbar.Value++;
                            progressbartext.Text = "Moving File " + prograssbar.Value + "/" + prograssbar.Maximum;

                        }
                        catch (UnauthorizedAccessException e)
                        {

                            message = "Error:\n" + e.Message + "\n\"" + dataRow.ItemArray.First().ToString() + "\"";

                        }
                        catch (DirectoryNotFoundException e)
                        {
                            message = "Error:\n" + e.Message + "\n\"" + dataRow.ItemArray.First().ToString() + "\"";

                        }
                        catch
                        {
                            message = "Error: Something went wrong!";
                        }
                    }
                }
            }
            else
            {
                //Behandlung falls keine Dateien zur Bearbeitung zur verfügung stehen
                prograssbar.Maximum = 1;
                prograssbar.Value = 1;

                message = "Warning:\nNo matching files found.\nNothing changed!";
            }

            return message;
        }
    }
}
