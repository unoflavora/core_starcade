using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleWebRequest : MonoBehaviour
{
        private static async Task<string> AsyncPostRequest(string url, string body)
        {
            string urlRequest = url;
            var uwr = new UnityWebRequest(urlRequest, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);

            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            
            if (request.webRequest.result == UnityWebRequest.Result.ConnectionError || request.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Web Request Fail");
                return null;
            }
            
            Debug.Log("Success Request Fail");
            return request.webRequest.downloadHandler.text;
        }
        
        public static async Task<string> AsyncGetRequest(string url, string body)
        {
            string urlRequest = url;
            var uwr = new UnityWebRequest(urlRequest, "GET");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);

            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            
            if (request.webRequest.result == UnityWebRequest.Result.ConnectionError || request.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Web Request Fail");
                return null;
            }
            
            Debug.Log(request.webRequest.downloadHandler.text);
            return request.webRequest.downloadHandler.text;
        }


        public async Task<string> CallGet(string url, string body)
        {
            string bodyJson = JsonUtility.ToJson(body);
            string result = await AsyncGetRequest(url, bodyJson);
            return result;
        }
}
