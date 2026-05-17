using System.Windows;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Start by loading the Login View
            MainFrame.Navigate(new LoginView());
        }
    }
}
