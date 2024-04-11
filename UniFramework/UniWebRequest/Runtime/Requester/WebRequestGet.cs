using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UniFramework.WebRequest
{
	public sealed class WebRequestGet : WebRequestBase
	{
		public WebRequestGet(string url) : base(url)
		{
		}

		/// <summary>
		/// 发送GET请求
		/// </summary>
		/// <param name="timeout">超时：从请求开始计时</param>
		public void SendRequest(int timeout = 0)
		{
			if (_webRequest == null)
			{
				_webRequest = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbGET);
				DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
				_webRequest.downloadHandler = handler;
				_webRequest.disposeDownloadHandlerOnDispose = true;
				_webRequest.timeout = timeout;
				_operation = _webRequest.SendWebRequest();
				_operation.completed += CompleteInternal;
			}
		}

		/// <summary>
		/// 获取下载的字节数据
		/// </summary>
		public byte[] GetData()
		{
			if (_webRequest != null && IsDone())
				return _webRequest.downloadHandler.data;
			else
				return null;
		}

		/// <summary>
		/// 获取下载的文本数据
		/// </summary>
		public string GetText()
		{
			if (_webRequest != null && IsDone())
				return _webRequest.downloadHandler.text;
			else
				return null;
		}
	}
}