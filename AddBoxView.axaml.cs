using Avalonia.Controls;
using Kitbox_app.ViewModels;

namespace Kitbox_app.Views
{
    public partial class AddBoxView : Window
    {
        public AddBoxView()
        {
            InitializeComponent();
        }

        private void OnValidateClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is AddBoxViewModel vm)
            {
                Close(vm.ValidateCommand.Execute().Result);
            }
        }
    }
}
