using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace UniFramework.WebRequest
{
    /// <summary>
    /// 请求器基类
    /// 说明：UnityWebRequest(UWR) supports reading streaming assets since 2017.1
    /// </summary>
    public abstract class WebRequestBase : IEnumerator
    {
        protected UnityWebRequest _webRequest;
        protected UnityWebRequestAsyncOperation _operation;
        protected System.Action<WebRequestBase> _callback;

        /// <summary>
        /// 请求URL地址
        /// </summary>
        public string URL { private set; get; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public EReqeustStatus Status
        {
            get
            {
                if (_webRequest == null)
                    return EReqeustStatus.None;

                if (_webRequest.result == UnityWebRequest.Result.InProgress)
                    return EReqeustStatus.InProgress;
                else if (_webRequest.result == UnityWebRequest.Result.Success)
                    return EReqeustStatus.Succeed;
                else if (_webRequest.result == UnityWebRequest.Result.ConnectionError)
                    return EReqeustStatus.ConnectionError;
                else if (_webRequest.result == UnityWebRequest.Result.ProtocolError)
                    return EReqeustStatus.ProtocolError;
                else if (_webRequest.result == UnityWebRequest.Result.DataProcessingError)
                    return EReqeustStatus.DataProcessingError;
                else
                    throw new System.NotImplementedException(_webRequest.result.ToString());
            }
        }

        /// <summary>
        /// 返回的错误信息
        /// </summary>
        public string RequestError
        {
            get
            {
                if (_webRequest == null)
                    return string.Empty;
                return _webRequest.error;
            }
        }

        /// <summary>
        /// 返回的HTTP CODE
        /// </summary>
        public long ResponseCode
        {
            get
            {
                if (_webRequest == null)
                    return -1;
                return _webRequest.responseCode;
            }
        }

        /// <summary>
        /// 下载进度（0-1f）
        /// </summary>
        public float DownloadProgress
        {
            get
            {
                if (_webRequest == null)
                    return 0;
                return _webRequest.downloadProgress;
            }
        }

        /// <summary>
        /// 已经下载的总字节数
        /// </summary>
        public ulong DownloadedBytes
        {
            get
            {
                if (_webRequest == null)
                    return 0;
                return _webRequest.downloadedBytes;
            }
        }

        /// <summary>
        /// 上传进度（0-1f)
        /// </summary>
        public float UploadProgress
        {
            get
            {
                if (_webRequest == null)
                    return 0;
                return _webRequest.uploadProgress;
            }
        }

        /// <summary>
        /// 已经上传的总字节数
        /// </summary>
        public ulong UploadedBytes
        {
            get
            {
                if (_webRequest == null)
                    return 0;
                return _webRequest.uploadedBytes;
            }
        }

        /// <summary>
        /// 完成委托
        /// </summary>
        public event System.Action<WebRequestBase> Completed
        {
            add
            {
                if (IsDone())
                    value.Invoke(this);
                else
                    _callback += value;
            }
            remove
            {
                _callback -= value;
            }
        }


        public WebRequestBase(string url)
        {
            URL = url;
        }

        /// <summary>
        /// 释放下载器
        /// </summary>
        public void Dispose()
        {
            if (_webRequest != null)
            {
                _webRequest.Dispose();
                _webRequest = null;
                _operation = null;
                _callback = null;
            }
        }

        /// <summary>
        /// 是否完毕（无论成功失败）
        /// </summary>
        public bool IsDone()
        {
            if (_operation == null)
                return false;
            return _operation.isDone;
        }

        #region 异步相关
        protected void CompleteInternal(AsyncOperation op)
        {
            _callback?.Invoke(this);
        }

        bool IEnumerator.MoveNext()
        {
            return !IsDone();
        }
        void IEnumerator.Reset()
        {
        }
        object IEnumerator.Current
        {
            get { return null; }
        }
        #endregion
    }
}