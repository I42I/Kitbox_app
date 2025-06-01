using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;
using Avalonia.Threading;

namespace KitBoxDesigner.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        /// <summary>
        /// Safely raises property changed notifications on the UI thread
        /// </summary>
        protected void SafeRaisePropertyChanged(string propertyName)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                this.RaisePropertyChanged(propertyName);
            }
            else
            {
                Dispatcher.UIThread.Post(() => 
                {
                    this.RaisePropertyChanged(propertyName);
                });
            }
        }
        
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
        protected bool SafeRaiseAndSetIfChanged<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
                
            field = value;
            
            if (propertyName != null)
            {
                SafeRaisePropertyChanged(propertyName);
            }
            
            return true;
        }
    }

    /// <summary>
    /// Simple command implementation that doesn't use reactive patterns to avoid threading issues
    /// </summary>
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public SimpleCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Simple command implementation with parameter that doesn't use reactive patterns
    /// </summary>
    public class SimpleCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public SimpleCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T?)parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute((T?)parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Simple async command implementation that doesn't use reactive patterns
    /// </summary>
    public class SimpleAsyncCommand : ICommand
    {
        private readonly Func<System.Threading.Tasks.Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public SimpleAsyncCommand(Func<System.Threading.Tasks.Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                await _execute();
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }

    /// <summary>
    /// Simple async command implementation with parameter that doesn't use reactive patterns
    /// </summary>
    public class SimpleAsyncCommand<T> : ICommand
    {
        private readonly Func<T?, System.Threading.Tasks.Task> _execute;
        private readonly Func<T?, bool>? _canExecute;
        private bool _isExecuting;

        public SimpleAsyncCommand(Func<T?, System.Threading.Tasks.Task> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke((T?)parameter) ?? true);
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                await _execute((T?)parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }
}
