namespace batch_rename
{
    internal class File
    {
        private string name;
        private string newName;
        private string path;
        private string error;

        public File(string name, string newName, string path, string error)
        {
            this.Name = name;
            this.NewName = newName;
            this.Path = path;
            this.Error = error;
        }

        public string Name { get => name; set => name = value; }
        public string NewName { get => newName; set => newName = value; }
        public string Path { get => path; set => path = value; }
        public string Error { get => error; set => error = value; }
    }
}