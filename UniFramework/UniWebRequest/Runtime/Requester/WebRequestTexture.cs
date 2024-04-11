using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace UniFramework.WebRequest
{
    public sealed class WebRequestTexture : WebRequestBase
    {
        private RequestAsset _cachedAsset;

        public WebRequestTexture(string url) : base(url)
        {
        }

        /// <summary>
        /// 发送资源请求
        /// </summary>
        /// <param name="timeout">超时：从请求开始计时</param>
        public void SendRequest(int timeout = 0)
        {
            if (_webRequest == null)
            {
                _webRequest = UnityWebRequestTexture.GetTexture(URL);
                _webRequest.timeout = timeout;
                _operation = _webRequest.SendWebRequest();
                _operation.completed += CompleteInternal;
            }
        }

        public RequestAsset GetRequestAsset()
        {
            if (IsDone() == false)
            {
                UniLogger.Warning("Web request is not finished yet!");
                return null;
            }

            if (Status != EReqeustStatus.Succeed)
                return null;

            if (_cachedAsset == null)
            {
                var texture = DownloadHandlerTexture.GetContent(_webRequest);
                _cachedAsset = new RequestAsset(URL, texture);
            }
            return _cachedAsset;
        }
    }
}