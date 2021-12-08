using System;
using System.Data;
using System.Data.SQLite;
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
        readonly Sortiermachine machine = new Sortiermachine();
        SQLiteDataAdapter destDa = new SQLiteDataAdapter();
        SQLiteDataAdapter whiteDa = new SQLiteDataAdapter();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MyWindow_Loaded;

        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //init Desktoppfad
            path.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //init deleteColumn
            var template = new DataTemplate();
            FrameworkElementFactory deleteButton = new FrameworkElementFactory(typeof(Button));
            deleteButton.SetValue(Button.ContentProperty, "X");
            deleteButton.SetValue(Button.CommandProperty, System.Windows.Input.ApplicationCommands.Delete);
            template.VisualTree = deleteButton;

            var deleteColumn1 = new DataGridTemplateColumn();
            deleteColumn1.Header = "Delete";
            deleteColumn1.CellTemplate = template;

            var deleteColumn2 = new DataGridTemplateColumn();
            deleteColumn2.Header = "Delete";
            deleteColumn2.CellTemplate = template;

            //init destinationTable
            destinationTable.ItemsSource = machine.GetTable("SELECT * FROM Destinations",ref destDa).DefaultView;
            destinationTable.Columns.Add(deleteColumn1);

            //init whitelistTable
            whitelistTable.ItemsSource = machine.GetTable("SELECT * FROM Whitelist",ref whiteDa).DefaultView;
            whitelistTable.Columns.Add(deleteColumn2);
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

        private void saveDirectories_Click(object sender, RoutedEventArgs e)
        {
            destDa.Update((destinationTable.ItemsSource as DataView).Table);
        }

        private void saveWhitelist1_Click(object sender, RoutedEventArgs e)
        {
            whiteDa.Update((whitelistTable.ItemsSource as DataView).Table);
        }
    }
}
