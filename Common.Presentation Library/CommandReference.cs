using System;
using System.Windows;
using System.Windows.Input;

namespace Common.Presentation {
  // Source: M-V-VM Toolkit 0.1 (Modified)
  /// <summary>
  ///   This class facilitates associating a key binding in XAML markup to a command
  ///   defined in a View Model by exposing a Command dependency property.
  ///   The class derives from Freezable to work around a limitation in WPF when data-binding from XAML.
  /// </summary>
  public class CommandReference: Freezable, ICommand {
    #region Constants and Fields
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));
    #endregion

    #region Events and Properties
    public event EventHandler CanExecuteChanged;

    public ICommand Command {
      get { return (ICommand)this.GetValue(CommandProperty); }
      set { this.SetValue(CommandProperty, value); }
    }
    #endregion

    #region Methods
    public Boolean CanExecute(Object parameter) {
      if (this.Command != null) {
        return this.Command.CanExecute(parameter);
      }
      return false;
    }

    public void Execute(Object parameter) {
      this.Command.Execute(parameter);
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      CommandReference commandReference = d as CommandReference;
      if (d != null) {
        ICommand oldCommand = e.OldValue as ICommand;
        ICommand newCommand = e.NewValue as ICommand;

        if (oldCommand != null) {
          oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
        }
        if (newCommand != null) {
          newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
        }
      }
    }

    protected override Freezable CreateInstanceCore() {
      throw new NotImplementedException();
    }
    #endregion
  }
}