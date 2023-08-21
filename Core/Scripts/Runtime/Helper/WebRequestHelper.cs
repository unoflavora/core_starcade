using Agate.Starcade.Runtime.Backend;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Agate.Starcade.Runtime.Helper
{
	public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }
    public class WebRequestHelper
    {
        //private static UnityWebRequest request;
        private SHA256Managed _sha256Managed;
        private int _timeout;

        private bool _isDone;
        private bool _isFail;

        private int _maxRetry = 5;
        private int _totalRetry;

        private string result;

        private bool _fullLog = true;

        public void InitWebRequest(int timeout)
        {
            _timeout = timeout;
            _sha256Managed = new SHA256Managed();
        }

        public WebRequestHelper(int timeout)
        {
            _timeout = timeout;
        }
        
        public WebRequestHelper(int timeout,bool fullLog)
        {
            _timeout = timeout;
            _fullLog = fullLog;
        }
        
        public WebRequestHelper()
        {
            _timeout = 20;
        }
        
        public async Task<GenericResponseData<T>> GetRequest<T>(string url, Dictionary<string,string> header)
        {
            if(_fullLog) Debug.Log("Get Request to url:"+url);
            
            _isDone = false;
            
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = _timeout;
            request.certificateHandler = new BypassCertificate();

            if (header != null)
            {
                foreach (var data in header)
                {
                    request.SetRequestHeader(data.Key,data.Value);
                }
            }
            
            request.SendWebRequest();
                
            while (!request.isDone)
            {
                await Task.Yield();
            }

			return HandleRequestResponse<T>(request);
        }

        public async Task<GenericResponseData<T>> PostRequest<T>(string url, Dictionary<string, string> header, string body)
        {
            UnityWebRequest request = UnityWebRequest.Put(url,body);
            request.timeout = _timeout;
            request.certificateHandler = new BypassCertificate();

            if (header != null)
            {
                foreach (var data in header)
                {
                    request.SetRequestHeader(data.Key, data.Value);
                }
            }

            request.method = "POST";

            request.SendWebRequest();
            
            while (!request.isDone)
            {
                await Task.Yield();
            }

            return HandleRequestResponse<T>(request);
        }

        private GenericResponseData<T> HandleRequestResponse<T>(UnityWebRequest request)
        {
			switch (request.result)
			{
				case UnityWebRequest.Result.ConnectionError:
					if(_fullLog) Debug.LogError(request?.error);

					request.Dispose();
					return ConnectionError<T>();
                case UnityWebRequest.Result.Success:
                    if(_fullLog) Debug.Log(request.downloadHandler.text);    
					var successResult =
						JsonConvert.DeserializeObject<GenericResponseData<T>>(request.downloadHandler.text);
                    request.Dispose();
                    return successResult;
				default:
                    if(_fullLog) Debug.Log("Error Bad Result on call " + request.url);
                    if(_fullLog) Debug.Log(request?.downloadHandler?.text);
                    if(_fullLog) Debug.LogError(request?.error); 

					if (request.responseCode != 400)
					{
                        if(_fullLog) Debug.Log("BAD REQ 400");
						var badResult = ProtocolError<T>("ERROR", request.responseCode.ToString());
						request.Dispose();
						return badResult;
					}
					else
					{
						var badResult = JsonConvert.DeserializeObject<GenericResponseData<T>>(request.downloadHandler.text);
                        if(_fullLog) Debug.Log("error code " + badResult.Error.Code);
                        request.Dispose();
						return badResult;
					}
			}
		}

        public async Task<GenericResponseData<T>> PutRequest<T>(string uri, Dictionary<string,string> header, string body)
        {
            UnityWebRequest request = UnityWebRequest.Put(uri,body);
            request.timeout = _timeout;
            request.certificateHandler = new BypassCertificate();

            if (header != null)
            {
                foreach (var data in header)
                {
                    request.SetRequestHeader(data.Key, data.Value);
                }
            }
            
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    if(_fullLog) Debug.Log("ERROR");
                    request.Dispose();
                    return ConnectionError<T>();
                case UnityWebRequest.Result.ProtocolError:
                    if(_fullLog) Debug.Log("Error Bad Result on call " + uri + " - " + request.downloadHandler.text);
                    var badResult =
                        JsonConvert.DeserializeObject<GenericResponseData<T>>(request.downloadHandler.text);
                    request.Dispose();
                    return badResult;
                case UnityWebRequest.Result.Success:
                    var successResult =
                        JsonConvert.DeserializeObject<GenericResponseData<T>>(request.downloadHandler.text);
                    request.Dispose();
                    return successResult;
                default:
                    if(_fullLog) Debug.Log("Error Default Bad Result on call " + uri + " - " + request.downloadHandler.text);
                    var badDefaultResult =
                        JsonConvert.DeserializeObject<GenericResponseData<T>>(request.downloadHandler.text);
                    request.Dispose();
                    return badDefaultResult;
            }
        }
        
        private string Checksum(string bodyText)
        {
            byte[] hash = _sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(bodyText));
            StringBuilder checksumHash = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                checksumHash.Append(b.ToString("x2"));
            }
            return checksumHash.ToString();
        }
        
        private GenericResponseData<T> ConnectionError<T>()
        {
            return new GenericResponseData<T>()
            {
                Data = default,
                Error = new Error()
                {
                    Code = "0",
                    Message = "Connection Error"
                },
                Message = "Web request failed : Connection Error",
                Meta = null,
                StatusCode = 0,
            };
        }


        private GenericResponseData<T> ProtocolError<T>(string message, string responseCode)
        {
            return new GenericResponseData<T>()
            {
                Data = default,
                Error = new Error()
                {
                    Code = responseCode ?? "1",
                    Message = "Protocol Error"
                },
                Message = "Web request failed : Protocol Error - " + message,
                Meta = null,
                StatusCode = 1,
            };
        }
    }

    public class ErrorTemp
    {
        
    }
}
