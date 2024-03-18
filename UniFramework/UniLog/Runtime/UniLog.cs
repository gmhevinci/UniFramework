using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Diagnostics;
using System.IO.Compression;
using Debug = UnityEngine.Debug;

namespace UniFramework.Log
{
    public class UniLog
    {
        private const string TimeFormat = "HH:mm:ss";
        private const string TimeFormat2 = "yyyyMMddHHmmss";
        private const int BREAKDISKLENGTH = 10485760;           //日志文件分卷大小 10M
#if UNITY_STANDALONE
        private const int MAXRETENTIONDAYS = 14;                //日志文件的最大保存天数
#else
        private const int MAXRETENTIONDAYS = 7;
#endif
        private FileStream fileStream;
        private StreamWriter streamWriter;
        private StringBuilder sbuilder;
        private bool _isManualFlush;
        private bool _isDirty;
        private int breakDiskCount = 1;                         //当前产生的日志分卷数量
        private bool isEditorCreate = false;                    //是否在编辑器中也产生日志文件

        public bool IsManualFlush { get => _isManualFlush;set { _isManualFlush = value; } }
        public GameObject driver;

        #region instance
        private static bool _isInitialize = false;
        private static readonly object obj = new object();
        private static UniLog _instance;
        public static UniLog Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (obj)
                    {
                        if (_instance == null)
                        {

                            _instance = new UniLog();
                            _isInitialize = true;
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        private UniLog()
        {
        }

        /// <summary>
        /// 初始化事件系统
        /// </summary>
        public static void Initalize()
        {
            if (_isInitialize == false)
            {
                _ = Instance;
                _isInitialize = true;

                Instance.SetLogOptions(true, LogType.Log, true);
                Instance.StartTrace();

                UniLogger.Log($"{nameof(UniLog)} initalize !");
            }
        }

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="logEnable">是否记录日志</param>
        /// <param name="showFrams">是否显示所有堆栈帧 默认只显示当前帧 如果设为0 则显示所有帧</param>
        /// <param name="filterLogType">过滤 默认log级别以上</param>
        /// <param name="editorCreate">是否在编辑器中产生日志记录 默认不需要</param>
        public void SetLogOptions(bool logEnable, LogType filterLogType = LogType.Log, bool editorCreate = false, bool isManualFlush = false)
        {
            Debug.unityLogger.logEnabled = logEnable;
            Debug.unityLogger.filterLogType = filterLogType;
            isEditorCreate = editorCreate;
            _isManualFlush = isManualFlush;
        }

        /// <summary>
        /// 开启跟踪日志信息
        /// </summary>
        public void StartTrace()
        {
            if (!Debug.unityLogger.logEnabled) return;

#if UNITY_EDITOR
            if (!isEditorCreate) return;
#endif
            CreateOutlog();

            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        }

        /// <summary>
        /// 获取存储目录
        /// </summary>
        /// <returns></returns>
        private string GetDirectory()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Application.dataPath + "/../" + "Logs/UniLog";
#else
            return Application.persistentDataPath + "/" + "Logs/UniLog";
#endif
        }

        /// <summary>
        /// 删除过期日志(超出最大的存储限制时长)
        /// </summary>
        private void DeleteOldLogFiles()
        {
            string[] files = Directory.GetFiles(GetDirectory());
            DateTime dateTime = DateTime.Now;

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);

                if (string.IsNullOrWhiteSpace(fileName)) continue;

                fileName = fileName.Split("-")[0];

                if (DateTime.TryParseExact(fileName, TimeFormat2, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
                {
                    if (dateTime.Subtract(expiryDate).Days > MAXRETENTIONDAYS) File.Delete(files[i]);
                }
            }
        }

        /// <summary>
        /// 创建日志本地文件
        /// </summary>
        private void CreateOutlog()
        {
            string directory = GetDirectory();
            string path = directory + "/" + DateTime.Now.ToString(TimeFormat2) + (breakDiskCount > 1 ? "-" + breakDiskCount.ToString() : string.Empty);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

#if UNITY_STANDALONE || UNITY_EDITOR
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/../" + "Logs/GTrace");
                dirInfo.Attributes |= FileAttributes.Hidden;
            }
            catch (Exception)
            {
            }
#endif

#if !UNITY_EDITOR
            //非编译器情况下进行日志过期删除逻辑
            DeleteOldLogFiles();
#endif

            fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            streamWriter = new StreamWriter(fileStream);
            sbuilder = new StringBuilder();
        }

        /// <summary>
        /// 释放数据流
        /// </summary>
        private void DisposeOutlog()
        {
            ManualFlush();

#if !UNITY_EDITOR
            MemoryStream stream = new MemoryStream();
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(stream);
#endif

            streamWriter.Dispose();
            streamWriter.Close(); ;
            fileStream.Dispose();
            fileStream.Close();

#if !UNITY_EDITOR
            File.Delete(fileStream.Name);
            fileStream = new FileStream(fileStream.Name+".log", FileMode.Create, FileAccess.Write);
            CompressStream(stream, fileStream);
            fileStream.Dispose();
            fileStream.Close();
#endif
        }

        /// <summary>
        /// 释放所有内容
        /// </summary>
        private void DisposeAll()
        {
            if (driver != null) GameObject.Destroy(Instance.driver);
            
            Application.logMessageReceivedThreaded -= Application_logMessageReceivedThreaded;

            WriteLine($"{nameof(UniLog)} dispose !");

            DisposeOutlog();

            _instance = null;

            UniLogger.Log($"{nameof(UniLog)} dispose !");
        }

        /// <summary>
        /// 数据流压缩
        /// </summary>
        public void CompressStream(Stream sources, Stream target)
        {
            GZipStream compressionStream = new GZipStream(target, CompressionMode.Compress);
            if (sources.CanSeek) sources.Seek(0, SeekOrigin.Begin);
            sources.CopyTo(compressionStream);
            compressionStream.Close();
        }

        /// <summary>
        /// 创建日志本地文件分卷
        /// </summary>
        private void OutlogBreakDisk()
        {
            if (streamWriter.BaseStream.Length < BREAKDISKLENGTH) return;

            breakDiskCount++;
            DisposeOutlog();
            CreateOutlog();
        }

        /// <summary>
        /// 关闭跟踪日志信息
        /// </summary>
        public static void Destroy()
        {
            if (!_isInitialize) return;

            _instance.DisposeAll();
        }

        /// <summary>
        /// 主动写入日志信息
        /// </summary>
        /// <param name="logString">日志信息</param>
        public static void WriteLine(string logString)
        {
            if (_instance != null)
            {
                _instance.Application_logMessageReceivedThreaded(logString, string.Empty, LogType.Assert);
            }
        }

        /// <summary>
        /// 读取日志堆栈日志信息并写入本地
        /// </summary>
        private void Application_logMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            sbuilder.Clear();
            sbuilder.Append($"[{type.ToString().Substring(0, 3)}]   [{DateTime.Now.ToString(TimeFormat)}]  {logString}");

            StackTrace stack = new StackTrace(true);
            bool isDebuglog = false;

            for (int i = 0; i < stack.FrameCount; i++)
            {
                StackFrame sf = stack.GetFrame(i);
                if (isDebuglog)
                {
                    sbuilder.Append($"    at [{sf.GetMethod().DeclaringType.FullName}:{sf.GetMethod().Name}() Line:{sf.GetFileLineNumber()}]");
                    break;
                }
                else
                {
                    string fullName = sf.GetMethod().DeclaringType.FullName;

                    if (fullName.EndsWith("UnityEngine.Debug")) isDebuglog = true;
                    else if (fullName.EndsWith("UniLog") && sf.GetMethod().Name.Equals("WriteLine")) isDebuglog = true;
                }
            }

            streamWriter.WriteLine(sbuilder.ToString());

            _isDirty = true;

            if (!_isManualFlush || !(type == LogType.Assert || type == LogType.Log))
            {
                _isDirty = false;
                streamWriter.Flush();
            }

            OutlogBreakDisk();
        }

        /// <summary>
        /// 主动应用数据流到本地文件
        /// </summary>
        public void ManualFlush()
        {
            if (!(_isManualFlush && _isDirty)) return;

            _isDirty = false;
            streamWriter.Flush();

            OutlogBreakDisk();
        }

    }
}