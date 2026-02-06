using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundation.CommonModel;
using UnityEngine;

namespace Foundation.TBA
{
    public class MessageManager :Singleton<MessageManager>
    {
        public Dictionary<string, MessageItem> mOnNetWorkInfo = new Dictionary<string, MessageItem>();

        public Queue<MessageItem> mFreeItems = new Queue<MessageItem>();
        
        public Task<TResult> Post<TResult>(
                string url,
                string logid,
                string pos,
                Dictionary<string, string> headers,
                object bodys,
                int timeout = 60)
                where TResult : GetData, new()
        {
            return Post<TResult>(url, logid, pos, headers, bodys, CancellationToken.None, timeout);
        }

        public Task<TResult> Post<TResult>(
            string url,
            string logid,
            string pos,
            Dictionary<string, string> headers,
            object bodys,
            CancellationToken cancellationToken,
            int timeout = 60)
            where TResult : GetData, new()
        {
            MessageItem item = GetFreeItem();
            item.mDataModel.mNetWordkCount = 0;
            item.mDataModel.mLogId = logid;
            item.mDataModel.mUrl = url;
            item.mDataModel.mHeader = headers;
            item.mDataModel.mTimeOut = timeout;
            item.mDataModel.mIsGet = false;
            item.mDataModel.mBody = bodys;
            item.mDataModel.mPosId = pos;
            item.name = pos;
            mOnNetWorkInfo.Add(logid, item);
            return item.Post<TResult>(cancellationToken);
        }

        public Task<TResult> Get<TResult>(
            string url,
            string pos,
            int timeout = 60)
            where TResult : GetData, new()
        {
            return Get<TResult>(url, pos, new Dictionary<string, string>(), CancellationToken.None, timeout);
        }

        public Task<TResult> Get<TResult>(
            string url,
            string pos,
            Dictionary<string, string> headers,
            CancellationToken cancellationToken,
            int timeout = 60)
            where TResult : GetData, new()
        {
            MessageItem item = GetFreeItem();
            item.mDataModel.mNetWordkCount = 0;
            item.mDataModel.mLogId = "";
            item.mDataModel.mUrl = url;
            item.mDataModel.mHeader = headers;
            item.mDataModel.mTimeOut = timeout;
            item.mDataModel.mIsGet = true;
            item.mDataModel.mPosId = pos;
            
            item.name = pos;
            return item.Get<TResult>();
        }

        public Task Get<TResult>(
            string url,
            string pos,
            Dictionary<string, string> headers,
            int timeout = 60,
            Action<TResult> callBack = null)
            where TResult : GetData, new()
        {
            MessageItem item = GetFreeItem();
            item.mDataModel.mNetWordkCount = 0;
            item.mDataModel.mUrl = url;
            item.mDataModel.mHeader = headers;
            item.mDataModel.mTimeOut = timeout;
            item.mDataModel.mIsGet = true;
            item.mDataModel.mPosId = pos;
            
            item.name = pos;
            return item.Get<TResult>(callBack);
        }

        private static object obj = new object();
        private MessageItem GetFreeItem()
        {
            Monitor.Enter(obj);
            MessageItem item;
            if (mFreeItems.Count > 0)
            {
                item = mFreeItems.Dequeue();
            }
            else
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(itemParent);
                
                item = obj.AddComponent<MessageItem>();
            }
            Monitor.Exit(obj);
            return item;
        }

        private Transform itemParent;
        
        public void OnInit()
        {
            if (itemParent == null)
            {
                var message = GameUtility.MGR.Find("Message");
                if (message == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "Message";
                    obj.transform.SetParent(GameUtility.MGR);
                    message = obj.transform;
                }
                itemParent = message;
            }
            
        }
        
    }
}