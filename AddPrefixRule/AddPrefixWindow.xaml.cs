using Contract;
using System.Windows;

namespace AddPrefixRule
{
    /// <summary>
    /// Interaction logic for AddPrefixWindow.xaml
    /// </summary>
    public partial class AddPrefixWindow : BaseWindow
    {
        public override string ClassName => "AddPrefix";
        public string Prefix { get; set; }

        public AddPrefixWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new AddPrefixWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            AddPrefixRuleParser parser = new AddPrefixRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                AddPrefixRule rule = parser.Parse(Command) as AddPrefixRule;
                Prefix = rule.Prefix;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {Prefix}";
            DialogResult = true;
        }
    }
}
