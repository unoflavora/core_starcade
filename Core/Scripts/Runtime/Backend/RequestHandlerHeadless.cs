using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Helper;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
//using Agate.Starcade.Scripts.Runtime.Info;
using UnityEngine;
using UnityEngine.Events;
namespace Agate.Starcade.Runtime.Backend
{
    public static class RequestHandlerHeadless
    {
        public static UnityEvent<object, InfoAction, InfoAction> OnError = new UnityEvent<object, InfoAction, InfoAction>();
        
        private static bool isFail;
        private static int totalRetry = 0;
        private static int maxRetry = 5;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static async Task<GenericResponseData<T>> CheckResult<T>(Func<Task<GenericResponseData<T>>> func)
        {
            bool isDone = false;
            GenericResponseData<T> res = new GenericResponseData<T>();
            while (!isDone)
            {
                res = await func();
                if (res.Error != null)
                {
                    Debug.LogError("[HEADLESS REQUEST HANDLER] ERROR " + res.Error.Code + " - " + res.Error.Message + " - " + func.Method.ReturnParameter.Name +"\n" +
                                   JsonConvert.SerializeObject(res));
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
                                OnError.Invoke(ErrorType.GameError,new InfoAction()
                                {
                                    ActionName = "Quit Game",
                                    Action = AppHelper.Quit
                                },null);
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
                        //PROTOCOL ERROR
                        case "1":
                            isFail = true;
                            Debug.LogError("PROTOCOL ERROR");
                            Debug.LogError(res.Message);
                            isDone = true;
                            OnError.Invoke(ErrorType.GameError,new InfoAction()
                            {
                                ActionName = "Quit Game",
                                Action = AppHelper.Quit
                            },null);
                            while (isFail)
                            {
                                await Task.Yield();
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
                        case "3": //TODO CHANGE TO EXPIRED TOKEN
                            isFail = true;
                            Debug.LogError("EXPIRED ERROR");
                            isDone = true;
                            OnError.Invoke(ErrorType.TokenExpired, new InfoAction()
                            {
                                ActionName = "Close",
                                Action = AppHelper.Quit,
                            },null);
                            while (isFail)
                            {
                                await Task.Yield();
                            }
                            break;
                        //SOMETHING WRONG HAPPEN ERROR
                        default:
                            //Debug.LogError("[HEADLESS REQUEST HANDLER] ERROR" + res.Error.Code + " - " + res.Error.Message );
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
        
        private static async Task Delay()
        {
            await Task.Delay(5000);
        }
    }
}