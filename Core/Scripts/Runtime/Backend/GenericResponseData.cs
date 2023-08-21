using Agate.Starcade.Boot;
using System.Collections;
using System.Collections.Generic;

namespace Agate.Starcade.Runtime.Backend
{
    public class GenericResponseData<T>
    {
		public T Data { get; set; }
		public object Meta { get; set; }
		public Error Error { get; set; }
		public string Message { get; set; }
		public int StatusCode { get; set; }
	}
}
