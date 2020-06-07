using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleTextEditor.UI.Helpers
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
