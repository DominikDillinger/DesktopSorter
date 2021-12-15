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

        public void MyWindow_Loaded(object sender, RoutedEventArgs e)
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

        public void sort_Click(object sender, RoutedEventArgs e)
        {

            int status = 0; // Fehlercode: [0] Success

            if (iscurrentpathcorrect)
            {
                status = machine.Sort(path.Text, progressbar);
            }
            else
            {
                status = 1; // Fehlercode: [1] directory path is not correct
            }

            // progressbar zurücksetzen
            progressbar.Value = 0;

            // je nach Status Message ausgeben 
            String message;
            switch (status)
            {
                case 1:
                    message = "Error: Directory path is not correct!";
                    break;
                case 2:
                    message = "Error: Sorting function error!";
                    break;
                default:
                    message = "Sorting process successful!";
                    break;
            }
            MessageBox.Show(message);

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
            Console.WriteLine("test");
        }

        private void saveWhitelist1_Click(object sender, RoutedEventArgs e)
        {
            whiteDa.Update((whitelistTable.ItemsSource as DataView).Table);
        }

        private void saveDirectories_Click_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("test");

        }
    }
}
