using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UniFramework.WebRequest
{
    public sealed class WebRequestFile : WebRequestBase
    {
        public WebRequestFile(string url) : base(url)
        {
        }

        /// <summary>
        /// 发送下载文件请求
        /// </summary>
        /// <param name="savePath">下载文件的保存路径</param>
        /// <param name="timeout">超时：从请求开始计时</param>
        public void SendRequest(string savePath, int timeout = 0)
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentNullException();

            if (_webRequest == null)
            {
                _webRequest = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbGET);
                DownloadHandlerFile handler = new DownloadHandlerFile(savePath);
                handler.removeFileOnAbort = true;
                _webRequest.timeout = timeout;
                _webRequest.downloadHandler = handler;
                _webRequest.disposeDownloadHandlerOnDispose = true;
                _operation = _webRequest.SendWebRequest();
                _operation.completed += CompleteInternal;
            }
        }
    }
}