namespace batch_rename
{
    internal class Method
    {
        private string id;
        private string name;

        public Method(string id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
    }
}