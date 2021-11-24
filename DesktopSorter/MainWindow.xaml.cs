using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DesktopSorter

{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool iscurrentpathcorrect = true;


        public MainWindow()
        {
            InitializeComponent();

            Loaded += MyWindow_Loaded;

            path.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //init Sortiermachine
            var machine = new Sortiermachine();

            //init destinationTable
            destinationTable.ItemsSource = machine.GetTable("SELECT * FROM Destinations").DefaultView;

            //init whitelistTable
            whitelistTable.ItemsSource = machine.GetTable("SELECT * FROM Whitelist").DefaultView;
        }

        public void sort(string sortpath, string[][] sortierDestinations, string[] whitelist)
        {
            //Methode, die die Files in einen Angegeben pfad sortiert.

            //Files von destination entgegennehmen und whitelist checken
            

            //Files in sortpath in destinations sortierren
        }

        private void sort_Click(object sender, RoutedEventArgs e)
        {
            if (iscurrentpathcorrect)
            {


                //Sortierfunktion hier einbinden
                progressbar.Value = 100;

                if (progressbar.Value == 100)
                {
                    MessageBox.Show("Sorting process successful!");
                    progressbar.Value = 0;
                }
                //Sortierfiunktion hier darüber einbinden


            }
            else
            {
                MessageBox.Show("Please choose a available directory!");
            }


        }

        private void pathchange_Click(object sender, RoutedEventArgs e)
        {
            var BrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            BrowserDialog.UseDescriptionForTitle = true;
            BrowserDialog.Description = "Please choose your folder to sort";
            BrowserDialog.ShowDialog();

            // Falls Abbrechen gedrückt wird -> keine Änderung am Pfad
            if (BrowserDialog.SelectedPath != "")
            {
                path.Text = BrowserDialog.SelectedPath;
            }

        }

        private void path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(path.Text))
            {
                iscurrentpathcorrect = true;
                pathincorrect.Text = ""; 
            }
            else
            {
                iscurrentpathcorrect = false;

                if (pathincorrect != null)
                {
                    pathincorrect.Text = "Error: Directory does not exists!";
                }
            }
        }
    }
}
