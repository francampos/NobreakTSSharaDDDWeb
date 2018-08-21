using NobreakTSSharaDDDWeb.UI.ViewModels;
using System.ComponentModel;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    //public class MonitoringViewModel : ViewModelBase, IDataErrorInfo
    public class MonitoringViewModel : ViewModelBase
    {

        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }
        private string serial;

        public string Serial
        {
            get { return serial; }
            set
            {
                serial = value;
                RaisePropertyChanged("Serial");
            }
        }
        private string nickname;

        public string Nickname
        {
            get { return nickname; }
            set
            {
                nickname = value;
                RaisePropertyChanged("Nickname");
            }
        }

        private string ipAddress;

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value;
                RaisePropertyChanged("IPAddress");
            }
        }

        private void SaveChanges(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Salvar mudanças");
        }

    }
}
