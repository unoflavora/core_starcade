using System.Collections.Generic;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    public class MonstamatchMatchMessageData
    {
        public string SessionId { get; set; }
        public List<CoordinatePath> Path { get; set; }
    }
    
}