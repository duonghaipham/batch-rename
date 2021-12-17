using Contract;
using System.Windows;

namespace ChangeExtensionRule
{
    /// <summary>
    /// Interaction logic for ChangeExtensionWindow.xaml
    /// </summary>
    public partial class ChangeExtensionWindow : BaseWindow
    {
        public override string ClassName => "ChangeExtension";

        public ChangeExtensionWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new ChangeExtensionWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeExtensionRuleParser parser = new ChangeExtensionRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                ChangeExtensionRule rule = parser.Parse(Command) as ChangeExtensionRule;
                txtExtension.Text = rule.Extension;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {txtExtension.Text}";
            DialogResult = true;
        }
    }
}
