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
        private BindingList<RunRule> _rules = new BindingList<RunRule>();

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

                Button button = new Button()
                {
                    Content = rule.Name,
                    Tag = rule.Name
                };

                button.Click += btnPrototype_Click;
                wpMethodChooser.Children.Add(button);
            }

            lvRunMethods.ItemsSource = _rules;
        }

        private void btnPrototype_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (sender as Button).Tag as String;

            if (selectedTagName == "Add prefix")
            {
                _rules.Add(new RunRule()
                {
                    Index = _rules.Count,
                    Name = "Add prefix"
                });
            }
            else
            {
                _rules.Add(new RunRule()
                {
                    Index = _rules.Count,
                    Name = "Add suffix"
                });
            }
        }

        private void btnClearMethod_Click(object sender, RoutedEventArgs e)
        {
            _rules.Clear();
        }

        private void btnRemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunMethods.SelectedIndex != -1)
                _rules.RemoveAt(lvRunMethods.SelectedIndex);
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

        private void btnRemoveMethodItself_Click(object sender, RoutedEventArgs e)
        {
            Button btnRemove = sender as Button;
            _rules.RemoveAt(Int32.Parse(btnRemove.Tag.ToString()));

            for (int i = 0; i < _rules.Count; i++)
                _rules[i].Index = i;
        }
    }
}
