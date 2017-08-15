using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace FormsGtkLive.GTK
{
    class Program
    {
        static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();
            var app = new App();
            Live.Init(app);
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Xamarin.Forms Live GTK Backend");
            window.Show();
            Gtk.Application.Run();
        }
    }
}