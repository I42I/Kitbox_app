using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;

namespace KitBoxDesigner.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// Safely executes an action on the UI thread
        /// </summary>
        protected void RunOnUIThread(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.UIThread.Post(action);
            }
        }
        
        /// <summary>
        /// Safely updates a property with thread-safe property change notification
        /// </summary>
        protected bool SafeSetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            // CommunityToolkit.Mvvm handles cross-thread marshalling automatically
            return SetProperty(ref field, value, propertyName);
        }
        
        /// <summary>
        /// Thread-safe version of RaiseAndSetIfChanged for compatibility with existing ViewModels
        /// </summary>
        protected bool SafeRaiseAndSetIfChanged<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            return SetProperty(ref field, value, propertyName);
        }
        
        /// <summary>
        /// Thread-safe property change notification
        /// </summary>
        protected void SafeRaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
