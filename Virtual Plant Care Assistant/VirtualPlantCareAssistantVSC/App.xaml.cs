using System.Windows;

namespace VirtualPlantCareAssistantVSC
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Data.DatabaseHelper.InitializeDatabase();
        }
    }
}
