
using Microsoft.Maui.Controls;
//using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
//using Application = Microsoft.Maui.Controls.Application;

namespace Anniversaries
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
