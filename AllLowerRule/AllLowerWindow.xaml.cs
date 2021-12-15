using Contract;

namespace AllLowerRule
{
    /// <summary>
    /// Interaction logic for AllLowerWindow.xaml
    /// </summary>
    public partial class AllLowerWindow : BaseWindow
    {
        public override string ClassName => "AllLower";

        public AllLowerWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new AllLowerWindow();
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Command = $"{ClassName}";
            DialogResult = true;
        }
    }
}
