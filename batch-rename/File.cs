using System.ComponentModel;

namespace batch_rename
{
    internal class File : INotifyPropertyChanged
    {
        private string _name;
        private string _newName;
        private string _path;
        private string _error;

        public string Name 
        {
            get
            { 
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string NewName
        {
            get
            {
                return _newName;
            }
            set
            {
                _newName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewName"));
            }
        }
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Path"));
            }
        }
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Error"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}