using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class SettingsApp
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string CurrentLanguage { get; set; }
        public float NetworkFailTime { get; set; }
        public float LowBatteryTime { get; set; }
        public float RestPCTime { get; set; }
        public bool ShutdownOSAlongWithPCFlag { get; set; }
        public bool StartAppAlongWithAppFlag { get; set; }

    }
}
