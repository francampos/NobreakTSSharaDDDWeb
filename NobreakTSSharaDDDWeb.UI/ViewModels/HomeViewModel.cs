using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NobreakTSSharaDDDweb.HidRS232;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface.Services;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged, INobreakServiceCallbackChannel
    {
        private UserControl _content;
        private bool _isMenuOpen;
        private string _criteria;
        private string _conexao;
        private string _conexaoIcon;
        private string _conexaoIconColor;
        private string _redeIcon;
        private string _redeIconColor = "Green";
        private string _rede;
        private IEnumerable<SampleGroupVm> _samples;
        private readonly IEnumerable<SampleGroupVm> _dataSource;
        private INobreakServiceChannel _channel;
        private bool _isConnected;
        public string Ip { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HomeViewModel()
        {
            IsMenuOpen = true;


            _dataSource = new[]
            {

                new SampleGroupVm
                {
                    Name = "Menu",
                    Items = new[]
                    {
                        //new SampleVm("Medidores", typeof(Medidores)),
                        new SampleVm("medidores", typeof(Medidores)),
                        new SampleVm("graficos", typeof(Graficos)),
                        new SampleVm("relatorios", typeof(ReportView)),
                        new SampleVm("configuracoes", typeof(SettingsView)),
                        new SampleVm("testes", typeof(TestView)),
                        new SampleVm("monitoramento", typeof(MonitoringView)),
                    }
                }
            };

            var sample = _dataSource.First().Items.First();
            var hvm = this;
            hvm.Content = (UserControl)Activator.CreateInstance(sample.Content);
            hvm.IsMenuOpen = false;
            _samples = _dataSource;

            Task.Factory.StartNew(() => Run());

        }

        #region Properties
        public IEnumerable<SampleGroupVm> Samples
        {
            get { return _samples; }
            set
            {
                _samples = value;
                OnPropertyChanged("Samples");
            }
        }

        public UserControl Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        public bool IsMenuOpen
        {
            get { return _isMenuOpen; }
            set
            {
                _isMenuOpen = value;
                OnPropertyChanged("IsMenuOpen");
            }
        }

        public string Criteria
        {
            get { return _criteria; }
            set
            {
                _criteria = value;
                OnPropertyChanged("Criteria");
                OnCriteriaChanged();
            }
        }

        public string Conexao
        {
            get { return _conexao; }
            set
            {
                _conexao = value;
                OnPropertyChanged("Conexao");
            }
        }

        public string ConexaoIcon
        {
            get { return _conexaoIcon; }
            set
            {
                _conexaoIcon = value;
                OnPropertyChanged("ConexaoIcon");
            }
        }

        public string ConexaoIconColor
        {
            get { return _conexaoIconColor; }
            set
            {
                _conexaoIconColor = value;
                OnPropertyChanged("ConexaoIconColor");
            }
        }

        public string RedeIcon
        {
            get { return _redeIcon; }
            set
            {
                _redeIcon = value;
                OnPropertyChanged("RedeIcon");
            }
        }

        public string RedeLabel
        {
            get { return _rede; }
            set
            {
                _rede = value;
                OnPropertyChanged("RedeLabel");
            }
        }

        public string RedeIconColor
        {
            get { return _redeIconColor; }

            set
            {
                _redeIconColor = value;
                OnPropertyChanged("RedeIconColor");
            }
        }

        public object Enumerations { get; private set; }
        #endregion

        public void ServiceShutdown()
        {
            Debug.WriteLine("Serviço sendo desligado...");
            App.UPSData = null;
            App.StatusInfo = null;
            Conexao = "Serviço indisponível";
            ConexaoIcon = "Connect"; //Icone Disconnect mostra tomara conectada 
            ConexaoIconColor = "Red";

            RedeLabel = "Desconhecido";
            RedeIcon = "CameraFlashOff";
            RedeIconColor = "Red";

            _isConnected = false;
        }

        /// <summary>
        /// Implementacao do Callback do Servico Monitor.
        /// </summary>
        /// <param name="status"></param>
        public void UpdateStatus(StatusUpdate status)
        {

            //Atualizacao das variaveis estaticas utilizadas pelas telas
            App.UPSData = status.UpsData;
            App.StatusInfo = status.StatusInfo;
            //App.Nobreak = status.Nobreak;


            //Atualização da barra de status
            //Conexao = status.StatusInfo.Conectado.ToString();

            RedeLabel = ResolveEnumGloblalizedStatus(status.StatusInfo.Rede);

            switch (status.StatusInfo.Rede)
            {
                case StatusRede.OperandoEmRede:
                    RedeIcon = "CameraFlash";
                    RedeIconColor = "Green";
                    break;
                case StatusRede.OperandoEmBateria:
                    RedeIcon = "Battery3";
                    RedeIconColor = "Green";
                    break;
                default:
                    RedeIcon = "CameraFlashOff";
                    RedeIconColor = "Red";
                    break;
            }

            if (status.StatusInfo.Bateria.Equals(StatusBateria.Baixa))
            {
                RedeIcon = "Battery1";
                RedeIconColor = "Red";
            }


            if (status.StatusInfo.Conectado)
            {
                Conexao = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelStatusConnectionOn");
                ConexaoIcon = "Disconnect"; //Icone Disconnect mostra tomara conectada 
                ConexaoIconColor = "Green";
            }
            else
            {
                Conexao = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelStatusConnectionOff");
                ConexaoIcon = "Connect";//Icone Connect mostra tomara conectada 
                ConexaoIconColor = "Red";
            }



        }

        private string ResolveEnumGloblalizedStatus(StatusRede rede)
        {
            string statusValue = "";
            switch (rede)
            {
                case StatusRede.Desconhecido:
                    statusValue = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelUnknow");
                    break;
                case StatusRede.Ok:
                    statusValue = "Ok";
                    break;
                case StatusRede.ModoByPass:
                    statusValue = "By pass";
                    break;
                case StatusRede.Anormal:
                    statusValue = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelAbnormal");
                    break;
                case StatusRede.Falha:
                    statusValue = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelFail");
                    break;
                case StatusRede.OperandoEmBateria:
                    statusValue = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelStatusBatteryOn");
                    break;
                case StatusRede.OperandoEmRede:
                    statusValue = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelStatusNetworkOn");
                    break;
                default:
                    break;
            }
            return statusValue;
        }

        /// <summary>
        /// Conecta o cliente ao serviço windows (local ou remoto)
        /// </summary>
        private void Connect()
        {
            while (!_isConnected)
            {
                try
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.Security.Mode = SecurityMode.None;
                    string ip = Properties.Settings.Default.IpNobreak;
                    ip = "127.0.0.1";

                    DuplexChannelFactory<INobreakServiceChannel> channelFactory = new DuplexChannelFactory<INobreakServiceChannel>(
                        new InstanceContext(this),
                        binding,
                        new EndpointAddress("net.tcp://" + ip + ":9999/powernt-service"));

                    _channel = channelFactory.CreateChannel();
                    _channel.Connect();

                    _isConnected = true;
                    App.Channel = _channel;

                }
                catch (Exception ex)
                {
                    Conexao = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("LabelUnavailableService");
                    RedeLabel = "";
                    App.UPSData = null;
                    App.StatusInfo = null;
                    Debug.WriteLine("Falha ao tentar conectar o canal: " + ex.Message);
                    _isConnected = false;
                    log.Error("Problema ao estabelecer conexão com serviço", ex);

                }

                Thread.Sleep(2000);
            }
        }

        //Executa a logica de 
        public void Run()
        {
            Random rand = new Random();
            int id = rand.Next(1000);

            // TODO: Avaliar alternativa para loop
            while (true)
            {
                try
                {
                    /*if (_isConnected)
                    {
                        if (rand.Next(100) < 5)
                        {
                            string not = _channel.DisplayMessage($" Cliente {id}") ? String.Empty : "not ";
                        }
                    }
                    else
                    {
                        Connect();
                    }*/

                    if (!_isConnected)
                    {
                        Connect();
                    }
                }
                catch (CommunicationObjectFaultedException ex)
                {
                    _isConnected = false;
                    log.Error("Erro no metodo Run: " + ex.Message, ex);
                }
                Thread.Sleep(2000);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCriteriaChanged()
        {
            if (string.IsNullOrWhiteSpace(Criteria))
            {
                Samples = _dataSource;
                return;
            }

            Samples = Samples.Select(x => new SampleGroupVm
            {
                Name = x.Name,
                Items = x.Items.Where(y => y.Title.ToLowerInvariant().Contains(Criteria.ToLowerInvariant()) ||
                                           y.Tags.ToLowerInvariant().Contains(Criteria.ToLowerInvariant()))
            });
        }

        private void SwitchCulture(string culture)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            try
            {
                ci = new CultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
                try
                {
                    // Try language without region
                    ci = new CultureInfo(culture.Substring(0, 2));
                }
                catch (Exception ex)
                {
                    ci = CultureInfo.InvariantCulture;
                    log.Error("Erro ao estabelcer Cultura da aplicação", ex);
                }
            }
            finally
            {
                LocalizeDictionary.Instance.Culture = ci;
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                //this.CultureTextBlock.Text = ci.EnglishName;
            }
        }


    }

    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
