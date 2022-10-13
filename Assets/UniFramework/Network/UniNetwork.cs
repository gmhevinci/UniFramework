using System;
using System.Collections.Generic;
using UniFramework.Utility;

namespace UniFramework.Network
{
	public static class UniNetwork
	{
		private static bool _isInitialize = false;
		private readonly static List<TcpClient> _tcpClients = new List<TcpClient>();

		/// <summary>
		/// 初始化网络系统
		/// </summary>
		public static void Initalize()
		{
			if (_isInitialize)
				throw new Exception($"{nameof(UniNetwork)} is initialized !");

			if (_isInitialize == false)
			{
				// 创建驱动器
				_isInitialize = true;
				UnityEngine.GameObject driverGo = new UnityEngine.GameObject($"[{nameof(UniNetwork)}]");
				driverGo.AddComponent<UniNetworkDriver>();
				UnityEngine.Object.DontDestroyOnLoad(driverGo);
			}
		}

		/// <summary>
		/// 更新网络系统
		/// </summary>
		internal static void Update()
		{
			if (_isInitialize)
			{
				foreach (var client in _tcpClients)
				{
					client.Update();
				}
			}
		}

		/// <summary>
		/// 销毁网络系统
		/// </summary>
		internal static void Destroy()
		{
			if (_isInitialize)
			{
				foreach (var client in _tcpClients)
				{
					client.Destroy();
				}

				_tcpClients.Clear();
				_isInitialize = false;
				UniLogger.Log($"{nameof(UniNetwork)} destroy all !");
			}
		}


		/// <summary>
		/// 创建TCP客户端
		/// </summary>
		/// <param name="packageCoderType">通信频道使用的网络包编码解码器类型</param>
		/// <param name="packageBodyMaxSize">网络包体最大长度</param>
		public static TcpClient CreateTcpClient(int packageBodyMaxSize, INetPackageEncoder encoder, INetPackageDecoder decoder)
		{
			var client = new TcpClient(packageBodyMaxSize, encoder, decoder);
			_tcpClients.Add(client);
			return client;
		}
	}
}