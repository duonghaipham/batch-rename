using Contract;
using System.ComponentModel;

namespace batch_rename
{
    internal class RunRule : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}