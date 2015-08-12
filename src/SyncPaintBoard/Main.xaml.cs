using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SyncPaintBoard;

namespace SyncPainBoard
{
    public partial class Main
    {
        private SimpleObject _movedSimpleObject;
        private Point _lastPosition;
        private MainViewModel _viewModel;

        public Main()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void RectangleLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_movedSimpleObject != null)
                return;
            var rectangle = sender as Rectangle;
            if (null == rectangle)
                return;
            _movedSimpleObject = rectangle.DataContext as SimpleObject;
            _movedSimpleObject.BorderBrush = Brushes.Orange.ToString();
            _lastPosition = Mouse.GetPosition(this);
        }

        private void Window_OnMouseMove(object sender, MouseEventArgs e)
        {

            if (_movedSimpleObject == null)
                return;

            var currentPosition = Mouse.GetPosition(this);
            _movedSimpleObject.Left = _movedSimpleObject.Left + ((int)currentPosition.X - (int)_lastPosition.X);
            _movedSimpleObject.Top = _movedSimpleObject.Top + ((int)currentPosition.Y - (int)_lastPosition.Y);
            _lastPosition = currentPosition;
        }

        private void Window_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (null != _movedSimpleObject)
                _movedSimpleObject.BorderBrush =_movedSimpleObject.Brush;
            _movedSimpleObject = null;
        }

        private void CanvasMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null == _viewModel)
                return;
            _viewModel.CreateNewSimpleObject(Mouse.GetPosition(sender as IInputElement));
        }

        private void RectangleRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var rectangle = sender as Rectangle;
            if (null == rectangle)
                return;
            var simpleObject = rectangle.DataContext as SimpleObject;
            if (null != simpleObject)
                _viewModel.RemoveSimpleObject(simpleObject);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender == Client)
                _viewModel = new ClientMainViewModel();
            else if (sender == Server)
                _viewModel = new ServerMainViewModel();

            DataContext = _viewModel;

        }
    }
}
