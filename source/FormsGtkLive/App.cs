using FormsGtkLive.Views;
using Xamarin.Forms;

namespace FormsGtkLive
{
    public class App : Application
    {
        public App()
        {
            MainPage = new LiveView();
        }
    }
}
