using FormsGtkLive.ViewModels;
using Xamarin.Forms;

namespace FormsGtkLive.Views
{
    public partial class EditorView : ContentPage
    {
        public EditorView()
        {
            InitializeComponent();

            BindingContext = new EditorViewModel();
        }
    }
}