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
using System.Linq;

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
            lvFolders.ItemsSource = _folders;
        }

        // Check whether rename files or folders, then start renaming them
        // Renaming business is surrounded in try, to handle duplication name
        // Use a dictionary with new path as key, value is the ammount of key (duplications)
        // If there is any duplication, append to name an index of duplication
        // After renaming successfully, increase that index
        private void btnStartBatch_Click(object sender, RoutedEventArgs e)
        {
            int type = tcTargets.SelectedIndex;

            MessageBoxResult msg = System.Windows.MessageBox.Show(
                "Are you sure you want to make the renaming?",
                "Start renaming",
                MessageBoxButton.YesNo
            );

            if (msg == MessageBoxResult.Yes)
            {
                if (type == (int)FileType.File)
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var file in _files)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(file.Path), file.NewName);

                        try
                        {
                            System.IO.File.Move(
                                file.Path,
                                newIdealName
                            );
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(file.NewName)} ({duplications[newIdealName]}){Path.GetExtension(file.NewName)}";

                            System.IO.File.Move(
                                file.Path,
                                Path.Combine(Path.GetDirectoryName(file.Path), newLessCollisionName)
                            );
                        }
                    }

                    _files.Clear();
                }
                else
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var folder in _folders)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(folder.Path), folder.NewName);

                        try
                        {
                            System.IO.File.Move(
                                folder.Path,
                                newIdealName
                            );
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(folder.NewName)} ({duplications[newIdealName]})";

                            Directory.Move(
                                folder.Path,
                                Path.Combine(Path.GetDirectoryName(folder.Path), newLessCollisionName)
                            );
                        }
                    }

                    _folders.Clear();
                }
            }
        }

        #region Project handlers

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                if (ReadProjectFile(openFileDialog.FileName, false) == true)
                {
                    Title = $"Batch rename - {openFileDialog.FileName}";

                    EvokeToUpdateNewName();
                }
            }
        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (Title == "Batch rename")
            {
                
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Project file (*.prj)|*.prj";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string projectName = saveFileDialog.FileName;

                    SaveProjectFile(projectName);

                    Title = $"Batch rename - {projectName}";
                }
            }
            else
            {
                string projectName = Title.Split(new string[] { "Batch rename - " }, StringSplitOptions.None)[1];

                SaveProjectFile(projectName);
            }
        }

        private void btnSaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Project file (*.prj)|*.prj";

            if (saveFileDialog.ShowDialog() == true)
            {
                string projectName = saveFileDialog.FileName;

                SaveProjectFile(projectName);

                Title = $"Batch rename - {projectName}";
            }
        }

        // Close the current project by clearing all processing data
        private void btnCloseProject_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();
            _files.Clear();
            _folders.Clear();
            lblPresetName.Content = "";
            Title = "Batch rename";
        }

        // Simply close the software
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

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
                            Path = openFileDialog.FileNames[i]
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
                            Path = file
                        });
                    }
                }
            }
        }

        // Drop a/mulitple file(s) into listview
        // Make sure it has not added to the software yet
        private void lvFiles_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
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

        // Load a folder to listview by choose it in FolderBrowserDialog
        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                if (!IsAdded(directory, (int)FileType.Folder))
                {
                    _folders.Add(new File()
                    {
                        Name = Path.GetFileName(directory),
                        NewName = ImposeRule(Path.GetFileName(directory)),
                        Path = directory
                    });
                }
            }
        }

        // Remove a folder by selected index
        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (lvFolders.SelectedIndex != -1)
            {
                _folders.RemoveAt(lvFolders.SelectedIndex);
            }
        }

        // Drop a/mulitple folder(s) into listview
        // Make sure it has not added to the software yet
        private void lvFolders_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] folders = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (var folder in folders)
                {
                    if (!IsAdded(folder, (int)FileType.File))
                    {
                        _folders.Add(new File()
                        {
                            Name = Path.GetFileName(folder),
                            NewName = ImposeRule(Path.GetFileName(folder)),
                            Path = folder
                        });
                    }
                }
            }
        }

        // Clear all folders
        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {
            _folders.Clear();
        }

        #endregion

        #region Preset handlers

        // Open an available preset and load rules
        private void btnOpenPreset_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                if (ReadProjectFile(openFileDialog.FileName, true) == true)
                {
                    lblPresetName.Content = openFileDialog.SafeFileName;

                    EvokeToUpdateNewName();
                }
            }
        }

        // Save the current use preset into preset file
        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (_runRules.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Preset file (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string writer = CreateWriterFromRunRules();

                    System.IO.File.WriteAllText(saveFileDialog.FileName, writer);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You have to define at least one rule");
            }
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

            foreach (var folder in _folders)
            {
                folder.NewName = ImposeRule(folder.Name);
            }
        }

        private string CreateWriterFromRunRules()
        {
            string writer = "Rules\n";

            for (int i = 0; i < _runRules.Count; i++)
            {
                if (!string.IsNullOrEmpty(_runRules[i].Command))
                {
                    writer += _runRules[i].Command + "\n";
                }
            }

            return writer;
        }

        private string CreateWriterFromTargets(int type)
        {
            BindingList<File> targets;
            string writer = "";

            if (type == (int)FileType.File)
            {
                writer = "Files\n";
                targets = _files;
            }
            else
            {
                writer = "Folders\n";
                targets = _folders;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (!string.IsNullOrEmpty(targets[i].Path))
                {
                    writer += targets[i].Path + "\n";
                }
            }

            return writer;
        }

        private void SaveProjectFile(string projectName)
        {
            string writer = "";

            writer += CreateWriterFromRunRules();
            writer += CreateWriterFromTargets((int)FileType.File);
            writer += CreateWriterFromTargets((int)FileType.Folder);

            System.IO.File.WriteAllText(projectName, writer);
        }

        private bool ReadProjectFile(string fileName, bool isOnlyPreset)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    if (!isOnlyPreset)
                    {
                        _files.Clear();
                        _folders.Clear();
                    }

                    _runRules.Clear();

                    string state = "";
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line == "Rules" || line == "Files" || line == "Folders")
                        {
                            state = line;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (state == "Rules")
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
                                else if (state == "Files")
                                {
                                    _files.Add(new File()
                                    {
                                        Name = Path.GetFileName(line),
                                        NewName = "",
                                        Path = line
                                    }) ;
                                }
                                else if (state == "Folders")
                                {
                                    _files.Add(new File()
                                    {
                                        Name = "",
                                        NewName = "",
                                        Path = line
                                    });
                                }
                                else
                                {

                                }
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
