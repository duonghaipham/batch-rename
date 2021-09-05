using Microsoft.Win32;
using System;
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

namespace batch_rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAddMethod_Click(object sender, RoutedEventArgs e)
        {
            bool isExisted = false;
            foreach (var item in lvMethods.Items.OfType<Method>())
                if (item.Name == cbMethodChooser.Text)
                {
                    isExisted = true;
                    break;
                }
            if (!isExisted)
                lvMethods.Items.Add(new Method(cbMethodChooser.SelectedIndex.ToString(), cbMethodChooser.Text));
        }

        private void btnClearMethod_Click(object sender, RoutedEventArgs e)
        {
            lvMethods.Items.Clear();
        }

        private void btnRemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            if (lvMethods.SelectedIndex != -1)
                lvMethods.Items.RemoveAt(lvMethods.SelectedIndex);
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    lvFiles.Items.Add(new File(
                        openFileDialog.SafeFileNames[i],
                        "",
                        openFileDialog.FileNames[i],
                        ""));
                }
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            lvFolders.Items.Clear();
        }

        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {
            lvFiles.Items.Clear();
        }

        private void tblConfig_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string tag = (sender as TextBlock).Tag.ToString();
            Window window;

            switch (tag)
            {
                case "Replace":
                    window = new MDReplace();
                    break;
                case "New case":
                    window = new MDNewCase();
                    break;
                case "Fullname normalize":
                    window = new MDFullnameNormalize();
                    break;
                case "Move":
                    window = new MDMove();
                    break;
                default:
                    window = new MDUniqueName();
                    break;
            }
            window.ShowDialog();
        }
    }
}
