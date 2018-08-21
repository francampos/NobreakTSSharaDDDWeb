using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class StatusUpdate //TODO Mover para projeto Domain
    {
        public int ConnectedClients { get; set; }
        public Update UpsData { get; set; }
        public StatusInfo StatusInfo { get; set; }
        //    public Nobreak Nobreak { get; set; }
    }
}
