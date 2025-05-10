using CommunityToolkit.Mvvm.ComponentModel;

namespace Kitbox_app.ViewModels;

public partial class LockerViewModel : ObservableObject
{
    [ObservableProperty] private int height = 40;
    [ObservableProperty] private int width = 62;
    [ObservableProperty] private int depth = 50;
    [ObservableProperty] private bool hasDoor = false;

    public string Description => $"Locker - H:{Height}, W:{Width}, D:{Depth}, Door: {(HasDoor ? "Yes" : "No")}";
}
