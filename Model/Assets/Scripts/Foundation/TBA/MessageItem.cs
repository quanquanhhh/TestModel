using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Foundation.TBA
{
    public class MessageItem : MonoBehaviour
    {
        public HttpRequestModel mDataModel = new HttpRequestModel();

        public void InitData(HttpRequestModel info)
        {
            mDataModel = info;
            mDataModel.mNetWordkCount = 0;
            if (mDataModel.mIsGet)
            {
                Get<GetData>(CancellationToken.None);
            }
            else
            {
                Post<GetData>(CancellationToken.None);
            }
        }

        public Task<TResult> Get<TResult>(
               Action<TResult> callBack = null)
               where TResult : GetData, new()
        {
            var tcs = new TaskCompletionSource<TResult>();
            StartCoroutine(GetInternal<TResult>((result) =>
            {
                tcs.SetResult(result);
                callBack?.Invoke(result);
            }));
            return tcs.Task;
        }

        public Task<TResult> Get<TResult>(
                CancellationToken cancellationToken)
                where TResult : GetData, new()
        {
            var tcs = new TaskCompletionSource<TResult>();
            StartCoroutine(GetInternal<TResult>((result) =>
            {
                tcs.SetResult(result);
            }));
            return tcs.Task;
        }

        public Task<TResult> Post<TResult>(
                CancellationToken cancellationToken)
                where TResult : GetData, new()
        {
            var tcs = new TaskCompletionSource<TResult>();
            StartCoroutine(PostInternal<TResult>((result) =>
            {
                tcs.SetResult(result);
            }));
            return tcs.Task;
        }

        private IEnumerator GetInternal<TResult>(
                Action<TResult> callback)
                where TResult : GetData, new()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(mDataModel.mUrl);
            foreach (var keyValue in mDataModel.mHeader)
            {
                webRequest.SetRequestHeader(keyValue.Key, keyValue.Value);
            }
            webRequest.timeout = mDataModel.mTimeOut;
            long timestamp = DateTime.UtcNow.Ticks / 10000;
            var result = new TResult();

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError ||
                webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                long nowTimestamp = DateTime.UtcNow.Ticks / 10000;

                if (nowTimestamp - timestamp > mDataModel.mTimeOut * 1000)
                {
                    result.ErrorCode = 408;
                    result.ErrorMessage = webRequest.error;
                }
                else
                {
                    result.ErrorCode = webRequest.responseCode;
                    result.ErrorMessage = webRequest.error;
                }
            }
            else
            {
                result.ErrorCode = webRequest.responseCode;
                result.RawData = webRequest.downloadHandler.text;
            }


            mDataModel.mIsOnNetWork = false;
            callback(result);
            MessageManager.Instance.mFreeItems.Enqueue(this);
            yield return null;
            webRequest.Dispose();
        }
        private IEnumerator PostInternal<TResult>(
               Action<TResult> callback,
               int tryCount = 10)
               where TResult : GetData, new()
        {
            while (mDataModel.mNetWordkCount < 10)
            {

                string body = JsonConvert.SerializeObject(mDataModel.mBody);
                UnityWebRequest webRequest = new UnityWebRequest(mDataModel.mUrl, "POST");
                webRequest.SetRequestHeader("Content-Layer", "application/json");
                foreach (var keyValue in mDataModel.mHeader)
                {
                    webRequest.SetRequestHeader(keyValue.Key, keyValue.Value);
                }
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = mDataModel.mTimeOut;
                long timestamp = DateTime.UtcNow.Ticks / 10000;
                var result = new TResult();

                Debug.Log("TODO: " +
                   $"----[TBA]-----send:\n" +
                   $"----event=[{mDataModel.mPosId}]----\n" +
                   $"----url=[{mDataModel.mUrl}]----\n" +
                   $"----data={body}\n----");

                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ProtocolError ||
                    webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    long nowTimestamp = DateTime.UtcNow.Ticks / 10000;

                    if (nowTimestamp - timestamp > mDataModel.mTimeOut * 1000)
                    {
                        result.ErrorCode = 408;
                        result.ErrorMessage = webRequest.error;
                    }
                    else
                    {
                        result.ErrorCode = webRequest.responseCode;
                        result.ErrorMessage = webRequest.error;
                    }
                }
                else
                {
                    result.ErrorCode = webRequest.responseCode;
                    result.RawData = webRequest.downloadHandler.text;
                }

                if (result.ErrorCode == 200)
                {
                    mDataModel.mNetWordkCount = 15;
                    mDataModel.mIsOnNetWork = false;
                    MessageManager.Instance.mOnNetWorkInfo.Remove(mDataModel.mLogId);
                    MessageManager.Instance.mFreeItems.Enqueue(this);

                    callback(result);

                    yield return null;
                    webRequest.Dispose();
                    yield break;
                }
                mDataModel.mNetWordkCount++;
                yield return new WaitForSeconds(3f);
            }
        }

    }

    [Serializable]
    public class HttpRequestModel
    {
        public string mLogId;

        public int mNetWordkCount = 0;

        public bool mIsOnNetWork;

        public bool mIsGet;

        public Dictionary<string, string> mHeader;

        public int mTimeOut;

        public object mBody;

        public string mPosId;

        public string mUrl;
    }

    public class GetData
    {
        [JsonProperty("errorCode")] public long ErrorCode { get; set; }
        [JsonProperty("errorMessage")] public string ErrorMessage { get; set; }

        [JsonProperty("requestId")] public string RequestId { get; set; }
        [JsonProperty("timestamp")] public long Timestamp { get; set; }

        [JsonProperty("rawData")] public string RawData { get; set; }
    }

    public class ResponseData<T> : GetData
    {
        [JsonProperty("data")] public T Data { get; set; }
    }
}