using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BeamerStreamer.Classes
{
	public class DelegateCommand<T> : ICommand
	{

		private string caption;
		private Func<T, bool> canExecuteMethod;

		#region Constructors


		public DelegateCommand()
		{ }

		public DelegateCommand(string caption)
		{
			this.caption = caption;
		}

		public DelegateCommand(Action<T> action)
		{
			Action = action;
		}

		public DelegateCommand(string caption, Action<T> action)
		{
			this.caption = caption;
			Action = action;
		}

		public DelegateCommand(Action<T> action, Func<T, bool> canExecuteMethod)
		{
			CanExecuteMethod = canExecuteMethod;
			Action = action;
		}

		public DelegateCommand(string caption, Action<T> action, Func<T, bool> canExecuteMethod)
		{
			this.caption = caption;
			Action = action;
			CanExecuteMethod = canExecuteMethod;
		}

		#endregion


		/// <summary>
		/// Id um das Command zu identifizieren
		/// </summary>
		/// <value></value>
		public Guid Id { get; set; }

		/// <summary>
		/// Aktion für den Command
		/// </summary>
		public Action<T> Action { get; set; }

		/// <summary>
		/// CanExecuteMethod
		/// </summary>
		public Func<T, bool> CanExecuteMethod
		{
			get { return canExecuteMethod; }
			set
			{
				canExecuteMethod = value;
				OnCanExecuteChanged(new EventArgs());
				NotifyPropertyChanged("CanExecuteMethod");
			}
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		public virtual void Execute(T parameter)
		{
			if (Action != null)
			{
				Action(parameter);
			}
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		void ICommand.Execute(object parameter)
		{
			Execute(parameter is T ? (T)parameter : default(T));
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		public virtual bool CanExecute(T parameter)
		{
			return CanExecuteMethod == null || CanExecuteMethod(parameter);
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute(parameter is T ? (T)parameter : default(T));
		}

		/// <summary>
		/// Shortcut als KeyGesture
		/// </summary>
		public KeyGesture KeyGesture { get; set; }

		/// <summary>
		/// Occurs when [can execute changed].
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// RaiseCanExecuteChanged
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Raise CanExecuteChanged Event
		/// </summary>
		public void OnCanExecuteChanged(EventArgs args)
		{
			//EventHandler handler = CanExecuteChanged;
			//if (handler != null) handler(this, e);

			var dispatcher = (Dispatcher)null;
			if (Application.Current != null)
				dispatcher = Application.Current.Dispatcher;
			EventHandler eventHandler = this.CanExecuteChanged;
			if (eventHandler == null)
				return;
			if (dispatcher != null && !dispatcher.CheckAccess())
				dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => OnCanExecuteChanged(args)));
			else
				eventHandler(this, EventArgs.Empty);
		}


		/// <summary>
		/// PropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// NotifyPropertyChanged
		/// </summary>
		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// NotifyPropertyChanged for all Properties 
		/// </summary>
		protected void NotifyAllPropertiesChanged()
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
				NotifyPropertyChanged(property.Name);
		}

		/// <summary>
		/// Gets the caption.
		/// </summary>
		public virtual string Caption
		{
			get { return caption; }
			set
			{
				caption = value;
				NotifyPropertyChanged("Caption");
			}
		}


	
	}
}