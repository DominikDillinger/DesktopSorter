using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs;

namespace DesktopSorter

{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void sort(string sortpath, string[][] sortierDestinations, string[] whitelist)
        {
            //Methode, die die Files in einen Angegeben pfad sortiert.

            //Files von destination entgegennehmen und whitelist checken
            

            //Files in sortpath in destinations sortierren
        }

        private void sort_Click(object sender, RoutedEventArgs e)
        {
            progressbar.Value = 100;

            if (progressbar.Value == 100)
            {
                MessageBox.Show("Sorting process successful!");
                progressbar.Value = 0;
            }
        }

        private void pathchange_Click(object sender, RoutedEventArgs e)
        {
            var BrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            BrowserDialog.UseDescriptionForTitle = true;
            BrowserDialog.Description = "Please choose your folder to sort";
            BrowserDialog.ShowDialog();
            path.Text = BrowserDialog.SelectedPath;

        }
    }
}