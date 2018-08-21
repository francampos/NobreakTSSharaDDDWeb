using NobreakTSSharaDDDWeb.Infra;
using System.Collections.Generic;
using System.Linq;
using NobreakTSSharaDDDWeb.Domain.Entities;
using System.Windows.Input;
using System.ComponentModel;
using System;
using NobreakTSSharaDDDWeb.Globalization;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {

        private List<Languages> _languageList;
        private Languages _selectedLanguage;
        private Languages _previousLanguage;
        private SettingsWorkQuery settingsWorkQuery;
        private SettingsWorkCommand SettingsWorkCommand;
        public ICommand SaveCommand { get; set; }

        public SettingsViewModel()
        {
            LoadResources();
            //Initialize();
            settingsWorkQuery = new SettingsWorkQuery();
            SelectedLanguage = FindCurrentLanguage();
            PreviousLanguage = SelectedLanguage;
            setFields();
            SettingsWorkCommand = new SettingsWorkCommand(this);
        }

        private Languages FindCurrentLanguage()
        {
            string currentLanguageCode = settingsWorkQuery.FindCurrentLanguage();
            return LanguageList.Find(l => l.Code == currentLanguageCode);
        }

        public List<Languages> LanguageList
        {
            get { return _languageList; }
            set
            {
                _languageList = value;
                RaisePropertyChanged("LanguageList");
            }
        }
        public Languages SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                RaisePropertyChanged("SelectedLanguage");
                //updateLanguage()
            }
        }
        public Languages PreviousLanguage
        {
            get { return _previousLanguage; }
            set
            {
                _previousLanguage = value;
                RaisePropertyChanged("PreviousLanguage");
            }
        }
        private void LoadResources()
        {

            LanguageList = new List<Languages>
            {
                new Languages() { Code = "pt-BR", Name = "Portuguese" },
                new Languages() { Code = "en-US", Name = "English" },
                new Languages() { Code = "es-ES", Name = "Spanish" }
            };
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Logica para persistencia de dados da tela SettingsView.xaml
        /// </summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////
        public int _NetworkFailureTime;
        public int _LowBatteryTime;
        public int _ShutdownNobreakTime;
        public bool _ShutdownNobreakWithSOFlag;
        public bool _StartAppWithSOFlag;
        public string _CurrentLanguage;
        public string _ActionShutdownOrHibernate;
        public bool _ShutdownFlag;
        public bool _HibernateFlag;
        private string _userPassword;
        private string _userEmail;

        public int NetworkFailureTime
        {
            get { return _NetworkFailureTime; }
            set
            {
                _NetworkFailureTime = value;
                RaisePropertyChanged("NetworkFailureTime");
            }
        }
        public int LowBatteryTime
        {
            get { return _LowBatteryTime; }
            set
            {
                _LowBatteryTime = value;
                RaisePropertyChanged("LowBatteryTime");
            }
        }
        public int ShutdownNobreakTime
        {
            get { return _ShutdownNobreakTime; }
            set
            {
                _ShutdownNobreakTime = value;
                RaisePropertyChanged("ShutdownNobreakTime");
            }
        }
        public bool ShutdownNobreakWithSOFlag
        {
            get { return _ShutdownNobreakWithSOFlag; }
            set
            {
                _ShutdownNobreakWithSOFlag = value;
                RaisePropertyChanged("ShutdownNobreakWithSOFlag");
            }
        }
        public bool StartAppWithSOFlag
        {
            get { return _StartAppWithSOFlag; }
            set
            {
                _StartAppWithSOFlag = value;
                RaisePropertyChanged("StartAppWithSOFlag");
            }
        }
        public string CurrentLanguage
        {
            get { return _CurrentLanguage; }
            set
            {
                _CurrentLanguage = value;
                RaisePropertyChanged("CurrentLanguage");
            }
        }
        public string ActionShutdownOrHibernate
        {
            get { return _ActionShutdownOrHibernate; }
            set
            {
                _ActionShutdownOrHibernate = value;
                RaisePropertyChanged("ActionShutdownOrHibernate");
            }
        }
        public bool ShutdownFlag
        {
            get { return _ShutdownFlag; }
            set
            {
                _ShutdownFlag = value;
                RaisePropertyChanged("ShutdownFlag");
            }
        }
        public bool HibernateFlag
        {
            get { return _HibernateFlag; }
            set
            {
                _HibernateFlag = value;
                RaisePropertyChanged("HibernateFlag");
            }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                RaisePropertyChanged("UserPassword");
            }
        }

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                RaisePropertyChanged("UserEmail");
            }
        }


        private void setFields()
        {
            SettingsWork _settingsWork = settingsWorkQuery.FindFirstData();
            NetworkFailureTime = _settingsWork.NetworkFailureTime;
            LowBatteryTime = _settingsWork.LowBatteryTime;
            ShutdownNobreakTime = _settingsWork.RestPCTime;
            ShutdownNobreakWithSOFlag = _settingsWork.ShutdownOSAlongWithPCFlag;
            StartAppWithSOFlag = _settingsWork.StartAppAlongWithAppFlag;

            ShutdownFlag = ResolveFieldIntToBoolean(_settingsWork.AutonomyEndTimeFlag);
            if (ShutdownFlag)
                HibernateFlag = false;
            else
                HibernateFlag = true;
        }

        private bool ResolveFieldIntToBoolean(int autonomyEndTime)
        {
            return (autonomyEndTime == 0 ? true : false);
        }

        //public void Initialize()
        //{
        //    SaveCommand = new SettingsWorkCommand(this);
        //}

        public void Save()
        {
            SettingsWorkCommand.Save();
        }
        /////////////////////////////////////////////////////////////////////////////////////Validation

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "NetworkFailureTime":
                        result = Validate(NetworkFailureTime);
                        break;
                    case "LowBatteryTime":
                        result = Validate(LowBatteryTime);
                        break;
                    case "ShutdownNobreakTime":
                        result = Validate(ShutdownNobreakTime);
                        break;
                    default:
                        break;
                }
                return result;

            }
        }

        private string Validate(int value)
        {
            if (value <= 0)
                return new TranslateToCurrentLanguage().GetCurrentLanguageDictionary("TipErrorNegativeValueField");
            return "";
        }

        private bool CheckIfHaveLetter(string v)
        {
            return v.Where(c => char.IsLetter(c)).Count() > 0;
        }
    }


}
