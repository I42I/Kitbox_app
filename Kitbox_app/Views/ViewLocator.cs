using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Kitbox_app.ViewModels;

namespace Kitbox_app.Views;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data == null)
            return new TextBlock { Text = "No view model provided" };

        var viewModelName = data.GetType().FullName!;
        var viewName = viewModelName
            .Replace("Kitbox_app.ViewModels", "Kitbox_app.Views")
            .Replace("ViewModel", "View");

        var viewType = Type.GetType(viewName + ", Kitbox_app");

        if (viewType != null)
            return (Control)Activator.CreateInstance(viewType)!;

        return new TextBlock { Text = $"View not found: {viewName}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
