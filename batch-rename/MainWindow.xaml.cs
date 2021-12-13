using Contract;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private Dictionary<string, IRenameRuleParser> _prototypes = new Dictionary<string, IRenameRuleParser>();
        private BindingList<string> _rules = new BindingList<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = new DirectoryInfo(exeFolder).GetFiles("*.dll");

            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFile(dll.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        if (typeof(IRenameRuleParser).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IRenameRuleParser;
                            _prototypes.Add(shape.Name, shape);
                        }
                    }
                }
            }


            foreach (var item in _prototypes)
            {
                var rule = item.Value as IRenameRuleParser;

                _rules.Add(rule.Name);
            }

            lvMethodChooser.ItemsSource = _rules;
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

        }
    }
}
