using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace NobreakTSSharaDDDWeb.UI.Hubs
{
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance =
            new Lazy<Broadcaster>(() => new Broadcaster());
        // We're going to broadcast to all clients a maximum of 25 times per second
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;
        private MonitorModel _model;
        private bool _modelUpdated;
        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<Monitor>();
            _model = new MonitorModel();
            _modelUpdated = false;
            // Start the broadcast loop
            //_broadcastLoop = new Timer(
            //    BroadcastShape,
            //    null,
            //    BroadcastInterval,
            //    BroadcastInterval);
        }
        //public void BroadcastShape(object state)
        //{
        //    // No need to send anything if our model hasn't changed
        //    if (_modelUpdated)
        //    {
        //        // This is how we can access the Clients property 
        //        // in a static hub method or outside of the hub entirely
        //        _hubContext.Clients.AllExcept(_model.LastUpdatedBy).updateShape(_model);
        //        _modelUpdated = false;
        //    }
        //}
        public void UpdateMonitor(MonitorModel monitorModel)
        {
            _model = monitorModel;
            _modelUpdated = true;
        }
        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }

    public class Monitor : Hub
    {
//        [Authorize]
        public void Update(MonitorModel model, string serial)
        {
            //var user = Authentication.User.Identity;
            string name = Context.User.Identity.Name;
            ////Clients.All.Update(model);
            model.LastUpdatedBy = Context.ConnectionId;
            // Update the shape model within our broadcaster
            //Clients.AllExcept(model.LastUpdatedBy).updateMonitor(model);
            Clients.Group(serial).updateMonitor(model);

        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            //Groups.Add(Context.ConnectionId, name);

            ApplicationDbContext db = new ApplicationDbContext();

            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserName.Equals(name));

                foreach (var u in user.Nobreaks)
                {
                    Groups.Add(Context.ConnectionId, u.Serial);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Erro OnConnected: " + ex.Message);
            }
            return base.OnConnected();
        }

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    MonitorModel model = new MonitorModel();
        //    model.Bateria = 0;
        //    model.Carga = 0;
        //    model.Frequencia = 0;
        //    model.TensaoEntrada = 0;
        //    model.TensaoSaida = 0;
        //    model.Temperatura = 0;

        //    var email = Context.Request.User.Identity.Name;
        //    Clients.Group(email).updateMonitor(model);
        //    return base.OnDisconnected(stopCalled);

        //}

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
            }

            return base.OnDisconnected(stopCalled);
        }





    }


}