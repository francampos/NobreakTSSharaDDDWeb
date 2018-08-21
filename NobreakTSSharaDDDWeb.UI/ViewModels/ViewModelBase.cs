using System;
using System.ComponentModel;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
