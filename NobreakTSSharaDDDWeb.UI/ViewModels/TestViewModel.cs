using System;
using System.ComponentModel;
using System.Linq;

namespace NobreakTSSharaDDDWeb.UI.ViewModels
{
    public class TestViewModel : ViewModelBase, IDataErrorInfo
    {
        public int _TestTime;
        private SettingsWorkQuery settingsWorkQuery;
        public TestViewModel()
        {
            settingsWorkQuery = new SettingsWorkQuery();
        }

        public int TestTime
        {
            get { return _TestTime; }
            set
            {
                _TestTime = value;
                RaisePropertyChanged("TestTime");
            }
        }

        public string FindCurrentLanguage()
        {
            return settingsWorkQuery.FindCurrentLanguage();
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                return Validate(TestTime);
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
