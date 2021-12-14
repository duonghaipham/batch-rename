using Contract;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace batch_rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, BaseWindow> _windowPrototypes = new Dictionary<string, BaseWindow>();
        private Dictionary<string, IRenameRuleParser> _ruleParserPrototypes = new Dictionary<string, IRenameRuleParser>();
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
                            var ruleParser = Activator.CreateInstance(type) as IRenameRuleParser;
                            _ruleParserPrototypes.Add(ruleParser.Name, ruleParser);
                        }

                        if (typeof(BaseWindow).IsAssignableFrom(type))
                        {
                            var window = Activator.CreateInstance(type) as BaseWindow;
                            _windowPrototypes.Add(window.ClassName, window);
                        }
                    }
                }
            }

            foreach (var item in _ruleParserPrototypes)
            {
                var rule = item.Value;

                Button button = new Button()
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(5, 3, 5, 3),
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Content = rule.Name,
                    Tag = rule.Name
                };

                button.Click += btnPrototype_Click;
                wpMethodChooser.Children.Add(button);
            }

            lvRunRules.ItemsSource = _rules;
        }

        private void btnPrototype_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (sender as Button).Tag as String;

            _rules.Add(new RunRule()
            {
                Index = _rules.Count,
                Name = selectedTagName,
                Command = ""
            });
        }

        private void btnClearMethod_Click(object sender, RoutedEventArgs e)
        {
            _rules.Clear();
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

        private void btnRemoveMethodItself_Click(object sender, RoutedEventArgs e)
        {
            Button btnRemove = sender as Button;
            _rules.RemoveAt(int.Parse(btnRemove.Tag.ToString()));

            UpdateOrder();
        }

        private void UpdateOrder()
        {
            for (int i = 0; i < _rules.Count; i++)
                _rules[i].Index = i;

            lvRunRules.ItemsSource = null;
            lvRunRules.ItemsSource = _rules;
        }

        private void btnRemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunRules.SelectedIndex != -1)
            {
                _rules.RemoveAt(lvRunRules.SelectedIndex);
                UpdateOrder();
            }
        }

        private void btnEditRule_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse((sender as Button).Tag.ToString());
            RunRule rule = _rules[index];

            var window = _windowPrototypes[rule.Name].CreateInstance();
            window.Command = rule.Command;

            if ((bool)window.ShowDialog())
            {
                _rules[index].Command = window.Command;
            }
        }
    }
}
