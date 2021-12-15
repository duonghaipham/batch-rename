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
        // Prototype to create a functioned window which handles a rule
        private Dictionary<string, BaseWindow> _windowPrototypes = new Dictionary<string, BaseWindow>();

        // Prototype to create a rule parser which handles a rule
        private Dictionary<string, IRenameRuleParser> _ruleParserPrototypes = new Dictionary<string, IRenameRuleParser>();

        // List of rules to impose
        private BindingList<RunRule> _runRules = new BindingList<RunRule>();

        // List of those files which is ready to impose on
        private BindingList<File> _files = new BindingList<File>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Load rule in dll, then render prototype buttons
        // Start to make data-binding for run rules list and files list
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

            // Dynamically load rule and create buttons for desired rules
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

                button.Click += btnAddRunRule_Click;
                wpRuleChooser.Children.Add(button);
            }

            lvRunRules.ItemsSource = _runRules;

            lvFiles.ItemsSource = _files;
        }

        #region Run rule handlers

        // Create a new run rule by clicking on a prototype rule button
        // Add new rule to run rule list
        private void btnAddRunRule_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (sender as Button).Tag as String;

            _runRules.Add(new RunRule()
            {
                Index = _runRules.Count,
                Name = selectedTagName,
                Command = ""
            });
        }

        // Edit rule in a window dialog
        private void btnEditRunRule_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse((sender as Button).Tag.ToString());
            RunRule rule = _runRules[index];

            var window = _windowPrototypes[rule.Name].CreateInstance();
            window.Command = rule.Command;

            if ((bool)window.ShowDialog())
            {
                _runRules[index].Command = window.Command;

                EvokeToUpdateNewName();
            }
        }

        // Remove a run rule by selected index
        private void btnRemoveRunRule_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunRules.SelectedIndex != -1)
            {
                _runRules.RemoveAt(lvRunRules.SelectedIndex);

                UpdateOrder();

                EvokeToUpdateNewName();
            }
        }

        // Remove a run rule by its Remove button
        private void btnRemoveRunRuleItself_Click(object sender, RoutedEventArgs e)
        {
            Button btnRemove = sender as Button;
            _runRules.RemoveAt(int.Parse(btnRemove.Tag.ToString()));

            UpdateOrder();

            EvokeToUpdateNewName();
        }

        // Clear all run rules
        private void btnClearRunRule_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();
        }

        #endregion

        #region Files handlers

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    _files.Add(new File()
                    {
                        Name = openFileDialog.SafeFileNames[i],
                        NewName = ImposeRule(openFileDialog.SafeFileNames[i]),
                        Path = openFileDialog.FileNames[i],
                        Error = ""
                    });
                }
            }
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedIndex != -1)
            {
                _files.RemoveAt(lvFiles.SelectedIndex);
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
        }

        #endregion

        #region Folder handlers

        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Helpers business

        // Iterate over the current rules list to update new index, which is stored in Tag property
        // Index changed due to some actions like add, delete, edit a rule
        private void UpdateOrder()
        {
            for (int i = 0; i < _runRules.Count; i++)
                _runRules[i].Index = i;

            lvRunRules.ItemsSource = null;
            lvRunRules.ItemsSource = _runRules;
        }

        // Impose rule(s) to original string, return a string which satisfied with all current rules 
        private string ImposeRule(string original)
        {
            string newName = original;

            foreach (var runRule in _runRules)
            {
                if (!string.IsNullOrEmpty(runRule.Command))
                {
                    IRenameRuleParser parser = _ruleParserPrototypes[runRule.Name];
                    IRenameRule rule = parser.Parse(runRule.Command);
                    newName = rule.Rename(newName);
                }
            }

            return newName;
        }

        // Evoke to update new file name as if changing rule, such as add, delete, edit rule(s)
        private void EvokeToUpdateNewName()
        {
            foreach (var file in _files)
            {
                file.NewName = ImposeRule(file.Name);
            }
        }

        #endregion
    }
}
