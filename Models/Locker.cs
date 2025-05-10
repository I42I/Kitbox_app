using CommunityToolkit.Mvvm.ComponentModel;

namespace Kitbox_app.Models
{
    public partial class Locker : ObservableObject
    {
        [ObservableProperty]
        private string height = "40";

        [ObservableProperty]
        private string width = "62";

        [ObservableProperty]
        private string depth = "50";

        public string Description => $"Locker - H:{Height}, W:{Width}, D:{Depth}";
    }
}
