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
        public string Extension { get; set; }

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
            DataContext = this;

            ChangeExtensionRuleParser parser = new ChangeExtensionRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                ChangeExtensionRule rule = parser.Parse(Command) as ChangeExtensionRule;
                Extension = rule.Extension;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {Extension}";
            DialogResult = true;
        }
    }
}
