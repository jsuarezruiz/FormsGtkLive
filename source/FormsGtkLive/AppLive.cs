using FormsGtkLive.Views;
using Xamarin.Forms;

namespace FormsGtkLive
{
    public class AppLive : Application
    {
        public AppLive()
        {
            MainPage = new LiveView();
        }
    }
}
