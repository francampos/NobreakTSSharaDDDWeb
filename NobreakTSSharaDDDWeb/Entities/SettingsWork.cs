using System.ComponentModel.DataAnnotations;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class SettingsWork 

    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public string CurrentLanguage { get; set; }
        public int NetworkFailureTime { get; set; }
        public int LowBatteryTime { get; set; }
        public int RestPCTime { get; set; }
        public bool ShutdownOSAlongWithPCFlag { get; set; }
        public bool StartAppAlongWithAppFlag { get; set; }
        public int AutonomyEndTimeFlag { get; set; }
        public int Port { get; set; }
        
        [MaxLength]
        public string UserToken { get; set; }
        [MaxLength]
        public string UserEmail { get; set; }
    }
}
