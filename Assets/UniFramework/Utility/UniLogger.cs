using System.Diagnostics;

namespace UniFramework.Utility
{
	internal static class UniLogger
	{
		/// <summary>
		/// 日志
		/// </summary>
		[Conditional("DEBUG")]
		public static void Log(string info)
		{
			UnityEngine.Debug.Log($"[UniFramework] {info}");
		}

		/// <summary>
		/// 警告
		/// </summary>
		public static void Warning(string info)
		{
			UnityEngine.Debug.LogWarning($"[UniFramework] {info}");
		}

		/// <summary>
		/// 错误
		/// </summary>
		public static void Error(string info)
		{
			UnityEngine.Debug.LogError($"[UniFramework] {info}");
		}
	}
}