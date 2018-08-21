using NobreakTSSharaDDDWeb.Application;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Globalization;
using NobreakTSSharaDDDWeb.Helper;
using NobreakTSSharaDDDWeb.Helpers;
using NobreakTSSharaDDDWeb.Infra;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using NobreakTSSharaDDDWeb.Domain.Enuns;
using System;
using NobreakTSSharaDDDWeb.Infra.Repository;
using System.Linq;
using NobreakTSSharaDDDWeb.Infra.Repository.Query;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Atributos para controle de butão limpar
        /// </summary>
        private ObservableCollection<NobreakDemandDataView> list;
        private DBQuery Query = null;
        private ICommand _clickCommand;


        private bool _canExecute;

        /// <summary>
        /// Array dinâmico responsável pela atualização da tabela na view
        /// </summary>
        public ObservableCollection<NobreakDemandDataView> ReportList
        {
            get { return list; }
            set { list = value; }
        }

        public ReportViewModel()
        {

            Query = new DBQuery();
            _canExecute = true;
            ReportList = new ObservableCollection<NobreakDemandDataView>();
            SetDataGrid(GetNobreakDemandDataList());
            FilterCommand = new RelayCommand(Get);


        }
        /// <summary>
        /// Insere vetor auxiliar dinâmico no vetor final que atualiza a tabela na view
        /// </summary>
        public void SetDataGrid(ObservableCollection<NobreakDemandDataView> currentList)
        {
            if (ReportList.Count != 0)
                ClearDatagrid();
            foreach (var nobreakData in currentList)
            {
                ReportList.Add(nobreakData);
            }
        }

        /// <summary>
        /// Recupera os dados do banco de dados e coloca dentro do vetor dinâmico
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<NobreakDemandDataView> GetNobreakDemandDataList()
        {
            ObservableCollection<NobreakDemandDataView> nobreakDemandDataViewObservableList = new ObservableCollection<NobreakDemandDataView>();

            //foreach (var item in App.Channel.AllEvents()) //TODO: Trazer os dados do relatório através do Serviço
            foreach (var item in Query.FindAll() )
            {
                NobreakDemandDataView nobreakDemandDataView = new NobreakDemandDataView()
                {
                    Battery = item.Battery,
                    EventReasons = ResolveEventReasons(item.EventReasons),
                    Frequency = item.Frequency,
                    InputVoltage = item.InputVoltage,
                    OutputVoltage = item.OutputVoltage,
                    Load = item.Load,
                    Temperature = item.Temperature,
                    CreationData = TranslateData(item.CreationData)
                };
                nobreakDemandDataViewObservableList.Add(nobreakDemandDataView);
            }
            return nobreakDemandDataViewObservableList;
        }

        private string TranslateData(DateTime creationData)
        {
            string result = "";

            switch (Query.FindCurrentLanguage())
            {
                case "pt-BR":
                    result = "" + SetupDateTimeValue(creationData.Day) + "/" + SetupDateTimeValue(creationData.Month) + "/" + creationData.Year + " " + " " + SetupDateTimeValue(creationData.Hour) + ":" + SetupDateTimeValue(creationData.Minute) + ":" + SetupDateTimeValue(creationData.Second);
                    break;
                case "en-US":
                    result = "" + SetupDateTimeValue(creationData.Month) + "/" + SetupDateTimeValue(creationData.Day) + "/" + creationData.Year + " " + SetupDateTimeValue(creationData.Hour) + ":" + SetupDateTimeValue(creationData.Minute) + ":" + SetupDateTimeValue(creationData.Second);
                    break;
                case "es-ES":
                    result = "" + SetupDateTimeValue(creationData.Day) + "-" + SetupDateTimeValue(creationData.Month) + "-" + creationData.Year + " " + SetupDateTimeValue(creationData.Hour) + ":" + SetupDateTimeValue(creationData.Minute) + ":" + SetupDateTimeValue(creationData.Second);
                    break;
                default:
                    break;
            }
            return result;
        }

        private string SetupDateTimeValue(int dateValue)
        {
            return (dateValue < 10) ? "0" + dateValue : dateValue.ToString();

        }

        private string ResolveEventReasons(EventReasons eventReasons)
        {
            string result = "";
            switch (eventReasons)
            {
                case EventReasons.RedeOk:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionNetwork");
                    break;
                case EventReasons.BateriaBaixa:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionLowBattery");
                    break;
                case EventReasons.ComunicacaoOk:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionRunningCommunication");
                    break;
                case EventReasons.FalhaRede:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionNetFailure");
                    break;
                case EventReasons.FalhaComunicacao:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionCommunicationBreakdown");
                    break;
                case EventReasons.Anormalidade:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionAbnormality");
                    break;
                case EventReasons.BateriaOk:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionBatteryWorks");
                    break;
                case EventReasons.TesteSolicitado:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionRequestedTest");
                    break;
                case EventReasons.RetornoDeTeste:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionTestReturn");
                    break;
                case EventReasons.HibernacaoSistema:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionSystemHibernation");
                    break;
                case EventReasons.DesligamentoSistema:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionSystemShutdown");
                    break;
                case EventReasons.RetornoSistema:
                    result = new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TableDescriptionSystemReturn");
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Método que orquestra comando de limpar base de dados 
        /// </summary>
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => ClearReport(), _canExecute));
            }
        }
        /// <summary>
        /// Remove os registros do banco de dados
        /// </summary>
        private void ClearReport()
        {
            Query.DeleteNobreakDemandData(Query.FindAll());
            ClearDatagrid();
            NotifierHelper.ShowInformation(new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TipCleanEventReport"), AppConstant.TIP_SUCCESS_MESSAGE);
        }
        /// <summary>
        /// Limpa conteúdo ja deletado da datagrid
        /// </summary>
        private void ClearDatagrid()
        {
            ReportList.Clear();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand FilterCommand { get; set; }
        //public ObservableCollection<NobreakDemandData> ListAux { get; set; }
        string _Filter;

        public string Filter
        {
            get { return _Filter; }
            set
            {
                _Filter = value;
                OnPropertyChanged("Filter");
            }
        }

        private void Get()
        {
            ObservableCollection<NobreakDemandDataView> listAux = new ObservableCollection<NobreakDemandDataView>();
            string dateOperator = "/";
            if (Query.FindCurrentLanguage().Equals("es-ES"))
                dateOperator = "-";
            if (!Filter.Equals(""))
            {
                SetDataGrid(GetNobreakDemandDataList());
                foreach (var nobreak in ReportList)
                {
                    if (!Filter.Contains(dateOperator))
                    {
                        if (nobreak.EventReasons.ToString().Contains(Filter)
                            || nobreak.InputVoltage.ToString().Contains(Filter)
                            || nobreak.OutputVoltage.ToString().Contains(Filter)
                            || nobreak.Load.ToString().Contains(Filter)
                            || nobreak.Battery.ToString().Contains(Filter)
                            || nobreak.Frequency.ToString().Contains(Filter)
                            || nobreak.Temperature.ToString().Contains(Filter)
                            || nobreak.CreationData.Contains(Filter)
                            || nobreak.CreationData.Contains(Filter))
                        { listAux.Add(nobreak); }

                    }
                    if (Filter.Contains(dateOperator) || Filter.Contains(":"))
                    {
                        if (nobreak.CreationData.Contains(Filter)
                         || nobreak.CreationData.Contains(Filter))
                        {
                            listAux.Add(nobreak);
                        }
                    }
                }
                ClearDatagrid();
                SetDataGrid(listAux);
            }
            else
            {
                ClearDatagrid();
                SetDataGrid(GetNobreakDemandDataList());
            }
        }

        private void OnPropertyChanged(string pName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }
    }

}
