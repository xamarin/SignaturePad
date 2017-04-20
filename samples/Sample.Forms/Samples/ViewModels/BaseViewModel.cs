using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Samples.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		protected void Refresh ([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}

		protected bool Set<T> (ref T field, T value, [CallerMemberName]string propertyName = null)
		{
			if (!object.Equals (field, value))
			{
				field = value;
				PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
				return true;
			}
			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public virtual void OnDisappearing ()
		{
		}

		public virtual void OnAppearing ()
		{
		}
	}
}
