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

        public AddPrefixWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new AddPrefixWindow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {txtPrefix.Text}";
            DialogResult = true;
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddPrefixRuleParser parser = new AddPrefixRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                AddPrefixRule rule = parser.Parse(Command) as AddPrefixRule;
                txtPrefix.Text = rule.Prefix;
            }
        }
    }
}
