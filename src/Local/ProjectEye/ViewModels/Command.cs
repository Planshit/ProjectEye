using System;
using System.Windows.Input;

namespace ProjectEye.ViewModels
{
    public class Command : ICommand
    {
        private Action<object> _action;

        public Command(Action<object> action)
        {
            _action = action;
        }


        #region ICommand Members  
        public bool CanExecute(object parameter)
        {
            return true;
        }
#pragma warning disable CS0067 // 从不使用事件“Command.CanExecuteChanged”
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // 从不使用事件“Command.CanExecuteChanged”
        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                _action("NULL");
            }
            OnExecuted();
        }
        public delegate void ExecutedHandler(object parameter);
        public event ExecutedHandler Executed;
        public void OnExecuted()
        {
            Executed?.Invoke(null);
        }
        #endregion
    }
}