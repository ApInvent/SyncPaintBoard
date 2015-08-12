using System;
using System.Windows.Input;

namespace SyncPaintBoard
{
    public class ActionCommand : ICommand
    {
        private readonly Action _executeAction;

        public ActionCommand(Action executeAction)
        {
            this._executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _executeAction();
        }

        public event EventHandler CanExecuteChanged;
    }
}