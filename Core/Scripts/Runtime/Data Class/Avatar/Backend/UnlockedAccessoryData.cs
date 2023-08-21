using System.Collections.Generic;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class UnlockedAccessoryData
    {
        public string UseAvatar { get; set; }
        public string UseFrame { get; set; }
		public List<string> UnlockAvatar { get; set; } = new List<string>();
        public List<string> UnlockFrame { get; set; } = new List<string>();
    }
}