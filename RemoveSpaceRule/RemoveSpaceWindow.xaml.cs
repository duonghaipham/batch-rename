using Contract;
using System.Windows;

namespace RemoveSpaceRule
{
    /// <summary>
    /// Interaction logic for RemoveSpaceWindow.xaml
    /// </summary>
    public partial class RemoveSpaceWindow : BaseWindow
    {
        public override string ClassName => "RemoveSpace";

        public RemoveSpaceWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new RemoveSpaceWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName}";
            DialogResult = true;
        }
    }
}
