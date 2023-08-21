using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class SessionData
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsStarted { get; set; }
    }
}