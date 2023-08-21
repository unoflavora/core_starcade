using Agate.Starcade.Core.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Runtime.Backend
{
	public static class RequestHandler
    {
        public static UnityEvent<object, InfoAction, InfoAction> OnError { get; set; } = new UnityEvent<object, InfoAction, InfoAction>();
        
        private static bool isFail;
        private static int totalRetry = 0;
        private static int maxRetry = 5;

        private static bool _isRequesting;
  
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static async Task<GenericResponseData<T>> CheckResult<T>(Func<Task<GenericResponseData<T>>> func)
        {
            bool isDone = false;
            GenericResponseData<T> res = new GenericResponseData<T>();
            while (!isDone)
            {
                res = await func();
                //Debug.Log("res data = " + res.Error.Code);
                if (res.Error != null)
                {
                    Debug.Log("CURRENT ERROR IS " + res.Error.Code);
                    switch (res.Error.Code)
                    {
                        //CONNECTION ERROR
                        case "0":
                            isFail = true;
                            Debug.LogError("CONNECTION ERROR");
                            if (totalRetry >= maxRetry)
                            {
                                isDone = true;
                                Debug.LogError("MAX RETRY EXCEEDED");
                                OnError.Invoke(ErrorType.GameError, new InfoAction()
                                {
                                    ActionName = "Restart",
                                    Action = () => 
                                    {
                                        isFail = false;
                                        Debug.Log("start restart");
                                        MainSceneController.Instance.GoToTitle();
                                    }
                                }, null);
                                while (isFail)
                                {
                                    await Task.Yield();
                                }
                            }
                            else
                            {
                                OnError.Invoke(ErrorType.ConnectionError,new InfoAction()
                                {
                                    ActionName = "Retry",
                                    Action = () =>
                                    {
                                        isFail = false;
                                        totalRetry++;
                                    }
                                }, null);
                                while (isFail)
                                {
                                    await Task.Yield();
                                }
                            }
                            break;
  
                        //MAINTENANCE
                        case "14004": //TODO CHANGE TO MAINTENANCE CODE
                            isFail = true;
                            Debug.LogError("MAIN ERROR");
                            isDone = true;
                            OnError.Invoke(ErrorType.Maintenance, new InfoAction()
                            {
                                ActionName = "Close",
                                Action = AppHelper.Quit,
                            },null);
                            while (isFail)
                            {
                                await Task.Yield();
                            }
                            break;
						//EXPIRED TOKEN
						case "401":
						case "10303":
                            isFail = true;
                            Debug.LogError("EXPIRED ERROR");
                            
                            if(totalRetry == 0)
                            {
                                var fetchRes = await MainSceneController.Instance.AuthBackend.FetchRefreshToken(MainSceneController.Instance.Auth.RefreshToken);
                                if(fetchRes.Data != null && fetchRes.Error == null)
                                {
                                    MainSceneController.Instance.Auth.SaveAuthData(fetchRes.Data);
								}

								isFail = false;
								totalRetry++;
							}
                            else
                            {
								isDone = true;
								OnError.Invoke(ErrorType.TokenExpired, new InfoAction()
								{
									ActionName = "Close",
									Action = MainSceneController.Instance.Auth.SignOut,
								}, null);
							}

                            while (isFail)
                            {
                                await Task.Yield();
                            }
                            break;
						//PROTOCOL ERROR
						case "1":
							isFail = true;
							Debug.LogError("PROTOCOL ERROR");
							Debug.LogError(res.Message);
							isDone = true;
							OnError.Invoke(ErrorType.GameError, new InfoAction()
							{
								ActionName = "Restart",
								Action = MainSceneController.Instance.GoToTitle
							}, null);
							while (isFail)
							{
								await Task.Yield();
							}
							break;
						//SOMETHING WRONG HAPPEN ERROR
						default:
                            Debug.LogError("ERROR BUT NOT GENERAL ERROR WITH CODE " + res.Error.Code);
                            totalRetry = 0;
                            return res;
                    }
                }
                else
                {
                    isDone = true; 
                }
                
            }
            totalRetry = 0;
            return res; // CLEAR RETURN
        }

        public static async Task<GenericResponseData<T>> Request<T>(Func<Task<GenericResponseData<T>>> func)
        {
            GenericResponseData<T> res = await CheckResult(func);
            return res;
        }

		public static async Task<bool> Request(Func<Task<bool>> func)
		{
			bool isDone = false;
			bool isSuccess = false;
			while (!isDone)
			{
				isSuccess = await func();
				//Debug.Log("res data = " + res.Error.Code);
				if (!isSuccess)
				{
					isFail = true;
					Debug.LogError("CONNECTION ERROR");
					if (totalRetry >= maxRetry)
					{
						isDone = true;
						Debug.LogError("MAX RETRY EXCEEDED");
						OnError.Invoke(ErrorType.GameError, new InfoAction()
						{
							ActionName = "Restart",
							Action = MainSceneController.Instance.GoToTitle
						}, null);
					}
					else
					{
						OnError.Invoke(ErrorType.ConnectionError, new InfoAction()
						{
							ActionName = "Retry",
							Action = () =>
							{
								isFail = false;
								totalRetry++;
							}
						}, null);
						while (isFail)
						{
							await Task.Yield();
						}
					}
				}
				else
				{
					isDone = true;
				}

			}
			totalRetry = 0;
			return isSuccess; // CLEAR RETURN
		}

		private static async Task Delay()
        {
            await Task.Delay(5000);
        }
    }
}