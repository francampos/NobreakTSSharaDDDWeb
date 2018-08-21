using System;
using NobreakTSSharaDDDweb.HidRS232;

namespace NobreakTSSharaDDDWeb.HidRS232
{
    public class NobreakEventArgs : EventArgs
    {
        public UpsData UpSdata { get; set; }
    }
}
