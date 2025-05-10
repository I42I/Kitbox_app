using Avalonia.Controls;
using Kitbox_app.ViewModels;

namespace Kitbox_app.Views
{
    public partial class CreateCabinetView : UserControl
    {
        public CreateCabinetView()
        {
            InitializeComponent();

            // 🛠 Force à chaque fois (sans "if")
            DataContext = new CreateCabinetViewModel(App.NavigationService);
        }
    }
}
