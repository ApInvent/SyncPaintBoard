using System.ComponentModel;
using System.Runtime.CompilerServices;
using SyncPaintBoard.Annotations;

namespace SyncPaintBoard
{
    public class SimpleObject : INotifyPropertyChanged
    {
        private string _brush;
        private int _width;
        private int _top;
        private int _left;
        private string _borderBrush;

        public int Left
        {
            get { return _left; }
            set
            {
                if (value.Equals(_left)) return;
                _left = value;
                OnPropertyChanged();
            }
        }

        public int Top
        {
            get { return _top; }
            set
            {
                if (value.Equals(_top)) return;
                _top = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value == _width) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        public string Brush
        {
            get { return _brush; }
            set
            {
                if (value.Equals(_brush)) return;
                _brush = value;
                OnPropertyChanged();
            }
        }

        public string BorderBrush
        {
            get { return _borderBrush; }
            set
            {
                if (Equals(value, _borderBrush)) return;
                _borderBrush = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
