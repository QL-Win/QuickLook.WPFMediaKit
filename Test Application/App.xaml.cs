using System.Windows;
using WPFMediaKit;

namespace Test_Application
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            AppUserModel.Register("WPFMediaKit.TestApplication", "WPF-MediaKit Test Application");
        }
    }
}
