using FormsGtkLive.Views;
using Xamarin.Forms;

namespace FormsGtkLive
{
    public class AppEditor : Application
    {
        public AppEditor()
        {
            MainPage = new CustomNavigationPage(new EditorView());
        }
    }
}
