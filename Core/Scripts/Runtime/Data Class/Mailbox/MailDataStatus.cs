using System;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class MailDataStatus
    {
        public bool Collected;
        public string ReceivedAt;
        public string ReadAt;
        public string ExpiredAt;
    }
}