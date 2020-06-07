using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SimpleTextEditor.UI.ViewModels;

namespace SimpleTextEditor.UI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel model = new MainWindowViewModel();
            DataContext = model;
        }

        public void OpenFile(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();

            if(!File.Exists(filename))
            {
                WarningMessage.Show($"Nie odnaleziono pliku: {filename}");
                return;
            }

            InformationMessage.Show($"Otrzymano plik: {filename}");
        }
    }
}
