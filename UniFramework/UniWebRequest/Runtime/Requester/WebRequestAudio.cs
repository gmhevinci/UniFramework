using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace UniFramework.WebRequest
{
    public sealed class WebRequestAudio : WebRequestBase
    {
        private RequestAsset _cachedAsset;

        public WebRequestAudio(string url) : base(url)
        {
        }

        /// <summary>
        /// 发送资源请求
        /// </summary>
        /// <param name="timeout">超时：从请求开始计时</param>
        public void SendRequest(AudioType audioType, bool streamAudio = false, bool compressed = false, int timeout = 0)
        {
            if (_webRequest == null)
            {
                _webRequest = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbGET);
                var downloadHandler = new DownloadHandlerAudioClip(URL, audioType);
                downloadHandler.streamAudio = streamAudio;
                downloadHandler.compressed = compressed;
                _webRequest.downloadHandler = downloadHandler;
                _webRequest.disposeDownloadHandlerOnDispose = true;
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
                var audioClip = DownloadHandlerAudioClip.GetContent(_webRequest);
                _cachedAsset = new RequestAsset(URL, audioClip);
            }
            return _cachedAsset;
        }
    }
}