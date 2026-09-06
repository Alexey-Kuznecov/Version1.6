
using IconMaker.Core.Mvvm.Base;

namespace IconMaker.Core.Models
{
    public sealed class IconTheme : PropertiesChanged
    {
        private bool _isMonochrome;
        private double _scale;
        private string _monochromeColor;

        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? PackId { get; set; }

        public string? ColorSchemeId { get; set; }

        public bool IsMonochrome
        {
            get => _isMonochrome;
            set
            {
                _isMonochrome = value;
                OnPropertyChanged(nameof(IsMonochrome));
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }

        public string MonochromeColor
        {
            get => _monochromeColor;
            set
            {
                _monochromeColor = value;
                OnPropertyChanged(nameof(MonochromeColor));
            }
        }
    }
}
