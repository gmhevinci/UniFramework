using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UniFramework.WebRequest
{
	public sealed class WebRequestHeader : WebRequestBase
	{
		public WebRequestHeader(string url) : base(url)
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
				_webRequest = UnityWebRequest.Head(URL);
				_webRequest.timeout = timeout;
				_operation = _webRequest.SendWebRequest();
				_operation.completed += CompleteInternal;
			}
		}

		/// <summary>
		/// 获取回复的头部数据
		/// </summary>
		public string GetResponseHeader(string name)
		{
			if (_webRequest != null && IsDone())
				return _webRequest.GetResponseHeader(name);
			else
				return null;
		}
	}
}