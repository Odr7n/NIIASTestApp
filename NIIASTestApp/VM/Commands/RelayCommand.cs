using System;

namespace NIIASTestApp.VM.Commands
{
    public class RelayCommand : CommandBase
    {
        /* Private Vars */
        #region Props

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        #endregion

        #region Ctors

        public RelayCommand() { }
        public RelayCommand(Action execute) : this(execute, null) { }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region Methods
        public override bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public override void Execute(object parameter) { _execute(); }

        #endregion
    }
}
