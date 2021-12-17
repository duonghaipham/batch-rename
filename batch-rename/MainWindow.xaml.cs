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
using System.Windows.Forms;

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

        // List of those folders which is ready to impose on
        private BindingList<File> _folders = new BindingList<File>();

        private enum FileType
        {
            File,
            Folder
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Load rule(s) in dll(s), then render prototype button(s)
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

                var button = new System.Windows.Controls.Button()
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(5, 3, 5, 3),
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Content = rule.Title,
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
        // Check rule whether is plug-and-play, for not to render edit rule for no parameters one
        // Add new rule to run rule list
        private void btnAddRunRule_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (sender as System.Windows.Controls.Button).Tag as String;

            IRenameRuleParser parser = _ruleParserPrototypes[selectedTagName];

            _runRules.Add(new RunRule()
            {
                Index = _runRules.Count,
                Name = selectedTagName,
                Title = parser.Title,
                IsPlugAndPlay = parser.IsPlugAndPlay,
                Command = parser.IsPlugAndPlay ? selectedTagName : ""
            });

            EvokeToUpdateNewName();
        }

        // Edit rule in a window dialog, only works with no parameters rule
        private void btnEditRunRule_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse((sender as System.Windows.Controls.Button).Tag.ToString());
            RunRule rule = _runRules[index];

            if (!rule.IsPlugAndPlay)
            {
                var window = _windowPrototypes[rule.Name].CreateInstance();
                window.Command = rule.Command;

                if ((bool)window.ShowDialog())
                {
                    _runRules[index].Command = window.Command;

                    EvokeToUpdateNewName();
                }
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
            var btnRemove = sender as System.Windows.Controls.Button;
            _runRules.RemoveAt(int.Parse(btnRemove.Tag.ToString()));

            UpdateOrder();

            EvokeToUpdateNewName();
        }

        // Clear all run rules
        private void btnClearRunRule_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();

            EvokeToUpdateNewName();
        }

        #endregion

        #region Files handlers

        // Load one or multiple file(e) by choose it(them) in OpenFileDialog
        // Before add a new File instance to the files list, firstly, check if it was added to
        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    if (!IsAdded(openFileDialog.FileNames[i], (int)FileType.File))
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
        }

        // Load all files inside a directory recursively
        // Before add a new File instance to the files list, firstly, check if it was added to
        private void btnAddFilesInDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file,
                            Error = ""
                        });
                    }
                }
            }
        }

        // Remove a file by selected index
        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedIndex != -1)
            {
                _files.RemoveAt(lvFiles.SelectedIndex);
            }
        }

        // Clear all files
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

        // Check a file (is representative by its path) whether was added to the files or folders list
        private bool IsAdded(string path, int type)
        {
            BindingList<File> browser;
            browser = (type == (int)FileType.File) ? _files : _folders;
            foreach (var item in browser)
            {
                if (item.Path == path)
                {
                    return true;
                }
            }
            return false;
        }

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

        #region Preset handlers

        private void btnOpenPreset_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            
            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    _runRules.Clear();
                    lblPresetName.Content = openFileDialog.SafeFileName;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                        {
                            int firstSpaceIndex = line.IndexOf(" ");
                            string firstWord = (firstSpaceIndex > 0) ? line.Substring(0, firstSpaceIndex) : line;

                            IRenameRuleParser parser = _ruleParserPrototypes[firstWord];
                            IRenameRule rule = parser.Parse(line);

                            _runRules.Add(new RunRule()
                            {
                                Index = _runRules.Count,
                                Name = firstWord,
                                Title = parser.Title,
                                IsPlugAndPlay = parser.IsPlugAndPlay,
                                Command = line
                            });
                        }
                    }

                    EvokeToUpdateNewName();
                }
            }
        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (_runRules.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Preset file (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string writer = "";

                    for (int i = 0; i < _runRules.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(_runRules[i].Command))
                        {
                            writer += _runRules[i].Command;

                            if (i != _runRules.Count - 1)
                                writer += '\n';
                        }
                    }

                    System.IO.File.WriteAllText(saveFileDialog.FileName, writer);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You have to define at least one rule");
            }
        }

        #endregion
    }
}
