using System.Reactive;
using ReactiveUI;
using Kitbox_app.Models;

namespace Kitbox_app.ViewModels
{
    public class AddBoxViewModel : ViewModelBase
    {
        private double _height;
        
        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        public string Color1 { get; set; } = "Red";
        public string Color2 { get; set; } = "Blue";
        public string Color3 { get; set; } = "Green";
        public string Color4 { get; set; } = "Yellow";

        public ReactiveCommand<Unit, Box> ValidateCommand { get; }

        public AddBoxViewModel()
        {
            ValidateCommand = ReactiveCommand.Create(() =>
                new Box(new Face(Color1, Height), new Face(Color2, Height),
                        new Face(Color3, Height), new Face(Color4, Height)));
        }
    }
}
