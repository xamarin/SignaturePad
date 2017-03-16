using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.Xam.Forms
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _title;
        private ICommand _saveSignatureCommand;
        private bool _isSignatureEmpty;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public bool IsSignatureEmpty
        {
            get { return _isSignatureEmpty; }
            set
            {
                _isSignatureEmpty = value;
                OnPropertyChanged();
            }
        }

        public void SaveSignature(Stream stream)
        {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
