using System.Collections;
using System.Collections.Generic;

namespace Agate.Starcade.Runtime.Backend
{
    public class Error
    {
		public string Code { get; set; }
		public string Message { get; set; }
		public string Target { get; set; }
		public object Details { get; set; }
		public object InnerError { get; set; }
	}
}
