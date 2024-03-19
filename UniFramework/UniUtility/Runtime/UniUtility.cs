using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.IO.Compression;

namespace UniFramework.Utility
{
    public static class UniUtility
    {
        public static class String
        {
            /// <summary>
            /// 10进制数字字符串 转 Int
            /// </summary>
            public static int ToInt(string str) 
            {
                if (string.IsNullOrEmpty(str)) return 0;

                string numericString = string.Empty;

                foreach (char c in str)
                {
                    // 检查是否有数字字符(0-9)、负号、前导或尾随空格。
                    if ((c >= '0' && c <= '9') || c == ' ' || c == '-' || c == '+')
                    {
                        numericString = string.Concat(numericString, c);
                    }
                    else
                    {
                        break;
                    }
                }

                if (int.TryParse(numericString, out int j))
                {
                    return j;
                }
                return 0;
            }

            /// <summary>
            /// 16进制数字字符串 转 Int
            /// </summary>
            public static int HexToInt(string str)
            {
                string numericString = string.Empty;

                foreach (var c in str)
                {
                    // 检查数字字符(十六进制)或前导或尾随空格。
                    if ((c >= '0' && c <= '9') || (char.ToUpperInvariant(c) >= 'A' && char.ToUpperInvariant(c) <= 'F') || c == ' ')
                    {
                        numericString = string.Concat(numericString, c.ToString());
                    }
                    else
                    {
                        break;
                    }
                }

                if (int.TryParse(numericString, NumberStyles.HexNumber, null, out int j))
                {
                    return j;
                }
                return 0;
            }

            /// <summary>
            /// 10进制数字字符串 转 Float
            /// </summary>
            public static float ToFloat(string str)
            {
                if (string.IsNullOrEmpty(str)) return 0;

                string numericString = string.Empty;

                foreach (char c in str)
                {
                    // 检查是否有数字字符(0-9)、负号、前导或尾随空格。
                    if ((c >= '0' && c <= '9') || c == ' ' || c == '-' || c == '+' || c == '.')
                    {
                        numericString = string.Concat(numericString, c);
                    }
                    else
                    {
                        break;
                    }
                }

                if (float.TryParse(numericString, out float j))
                {
                    return j;
                }
                return 0;
            }

            /// <summary>
            /// 版本字符串 转 Int
            /// </summary>
            public static int VersionToInt(string version)
            {
                string[] nums = version.Split('.');
                int sum = 0;

                for (int i = 0; i < nums.Length; i++)
                {
                    string item = nums[nums.Length - i - 1];
                    item = System.Text.RegularExpressions.Regex.Match(item, "[0-9]+", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace).Value;
                    int num = ToInt(item);

                    if (item.Length > 2) { return -1; }

                    sum += num * (int)System.Math.Pow(100, i);
                }
                return sum;
            }

            /// <summary>
            /// Int 转 版本字符串
            /// </summary>
            public static string IntToVersion(int sum)
            {
                string version = string.Empty;

                int step = Mathf.CeilToInt(sum.ToString().Length / 2f);

                for (int i = 0; i < step; i++)
                {
                    version = string.Concat("." , sum % 100 , version);
                    sum /= 100;
                }
                return version.Remove(0, 1);
            }
        }

        public static class FILE
        {
            /// <summary>
            /// 创建文件夹如果不存在的话
            /// </summary>
            /// <param name="direPath"></param>
            public static void CreateDirectory(string direPath)
            {
                string direct = Path.GetDirectoryName(direPath);
                if (!Directory.Exists(direct)) Directory.CreateDirectory(direct);
            }

            /// <summary>
            /// 删除指定文件夹下的所有文件与文件夹（不包括自身文件夹）
            /// </summary>
            /// <param name="direPath">指定文件夹的地址</param>
            /// <param name="includingSelf">是否删除自身文件夹</param>
            public static void DeleteDirectoryAll(string direPath, bool includingSelf = false)
            {
                if (!Directory.Exists(direPath)) return;

                if (includingSelf)
                {
                    Directory.Delete(direPath, true);
                }
                else
                {
                    var dires = Directory.GetDirectories(direPath);
                    var fiels = Directory.GetFiles(direPath);

                    foreach (var item in dires)
                    {
                        Directory.Delete(item, true);
                    }
                    foreach (var item in fiels)
                    {
                        File.Delete(item);
                    }
                }
            }

            /// <summary>
            /// 拷贝文件到另一个文件夹下
            /// </summary>
            /// <param name="sourceName">源文件路径</param>
            /// <param name="folderPath">目标路径（目标文件夹）</param>
            /// <returns>-1 数据源错误；0 目标位置已有相同的；1 对目标位置创建拷贝数据；2 对目标位置覆盖拷贝数据</returns>
            public static int CopyToFile(string sourcePath, string folderPath, string _fileName = null)
            {
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                //当前文件如果不用新的文件名，那么就用原文件文件名
                string fileName = string.IsNullOrWhiteSpace(_fileName) ? Path.GetFileName(sourcePath) : _fileName;
                //这里可以给文件换个新名字，如下：
                //string fileName = string.Format("{0}.{1}", "newFileText", "txt");

                //目标整体路径
                string targetPath = Path.Combine(folderPath, fileName);

                //Copy到新文件下
                FileInfo file = new FileInfo(sourcePath);
                if (file.Exists)
                {
                    if (File.Exists(targetPath))
                    {
                        string reourceMD5 = UniUtility.HashCode.MD5Hash(sourcePath);
                        string targetMD5 = UniUtility.HashCode.MD5Hash(targetPath);

                        if (!reourceMD5.Equals(targetMD5))
                        {
                            file.CopyTo(targetPath, true);
                            return 2;
                        }
                        return 0;
                    }
                    else
                    {
                        file.CopyTo(targetPath, true);
                        return 1;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 获得地址所在文件的所有字符信息
            /// </summary>
            public static string GetFileText(string path)
            {
                return File.ReadAllText(path);
            }

            /// <summary>
            /// 向文件写入内容，不覆盖
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="content"></param>
            public static void WriteToFile(string filePath, string content)
            {
                string fullPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                if (!File.Exists(filePath))
                {
                    FileStream fileStream = File.Create(filePath);
                    fileStream.Close();
                }

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine(content);
                m_streamWriter.Flush();
                m_streamWriter.Close();
            }

            /// <summary>
            /// 异步向文件写入，会覆盖原有的内容
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="content"></param>
            /// <param name="finishCallback"></param>
            public static async void WriteToFileAsync(string filePath, string content, Action finishCallback)
            {
                string fullPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                StreamWriter sw = new StreamWriter(filePath);
                await sw.WriteAsync(content);
                await sw.FlushAsync();
                sw.Close();
                sw.Dispose();
                finishCallback?.Invoke();
            }

            /// <summary>
            /// 同步向文件写入内容，会覆盖原来的内容
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="content"></param>
            public static void WriteToFileCover(string filePath, string content)
            {
                string fullPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }

            /// <summary>
            /// 删除文件
            /// </summary>
            /// <param name="filePath">地址</param>
            public static void DeleteFile(string filePath)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

        }

        public static class STREAM 
        {
            /// <summary>
            /// 压缩数据流(Zip格式)
            /// </summary>
            /// <param name="sources">原数据流</param>
            /// <param name="target">写入数据流</param>
            public static void CompressStream(Stream sources, Stream target)
            {
                GZipStream compressionStream = new GZipStream(target, CompressionMode.Compress);
                if (sources.CanSeek) sources.Seek(0, SeekOrigin.Begin);
                sources.CopyTo(compressionStream);
                compressionStream.Close();
            }

            /// <summary>
            /// 解压数据流(Zip格式)
            /// </summary>
            /// <param name="sources">原数据流</param>
            /// <param name="target">写入数据流</param>
            public static void DecompressStream(Stream sources, Stream target)
            {
                var zipStream = new GZipStream(sources, CompressionMode.Decompress);
                zipStream.CopyTo(target);
                if (sources.CanSeek) target.Seek(0, SeekOrigin.Begin);
                zipStream.Close();
            }
        }

        /// <summary>
        /// 哈希工具类
        /// </summary>
        public static class HashCode {

            /// <summary>
            /// 获取文件MD5值
            /// </summary>
            /// <param name="file">文件绝对路径</param>
            /// <returns>MD5值</returns>
            public static string MD5Hash(string file)
            {
                try
                {
                    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(fileStream);
                    fileStream.Close();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("获取文件MD5值error:" + ex.Message);
                }
            }

            public static string MD5Hash(byte[] bytes)
            {
                try
                {
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(bytes);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("获取文件MD5值error:" + ex.Message);
                }
            }

            public static string MD5Hash(string text, bool isString)
            {
                return isString ? MD5Hash(System.Text.UTF8Encoding.UTF8.GetBytes(text)) : MD5Hash(text);
            }

        }

        public static class COLOR
        {
            /// <summary> color转换hex </summary>
             /// <param name="color">Color对象</param>
             /// <returns>十六进制字符串</returns>
            public static string ColorToHex(Color color)
            {
                int r = Mathf.RoundToInt(color.r * 255.0f);
                int g = Mathf.RoundToInt(color.g * 255.0f);
                int b = Mathf.RoundToInt(color.b * 255.0f);
                int a = Mathf.RoundToInt(color.a * 255.0f);
                string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
                return hex;
            }

            /// <summary> hex转换到color </summary>
            /// <param name="hex">十六进制字符串</param>
            /// <returns>Color对象</returns>
            public static Color HexToColor(string hex)
            {
                byte br = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte bg = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte bb = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte cc = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                float r = br / 255f;
                float g = bg / 255f;
                float b = bb / 255f;
                float a = cc / 255f;
                return new Color(r, g, b, a);
            }

            /*
            
            private const byte k_MaxByteForOverexposedColor = 191;

            /// <summary>
            /// HDR color 转换为 Color
            /// </summary>
            /// <param name="hdrColor">HDR Color</param>
            /// <param name="exposure">强度</param>
            /// <returns>Color</returns>
            public static Color HDRColorToRGB(Color hdrColor, out float exposure)
            {
                float r, g, b,a;
                float maxColorComponent = hdrColor.maxColorComponent;
                // replicate Photoshops's decomposition behaviour
                if (maxColorComponent == 0f || maxColorComponent <= 1f && maxColorComponent >= 1f / 255f)
                {
                    exposure = 0f;
                    r = (byte)Mathf.RoundToInt(hdrColor.r * 255f);
                    g = (byte)Mathf.RoundToInt(hdrColor.g * 255f);
                    b = (byte)Mathf.RoundToInt(hdrColor.b * 255f);
                    a = (byte)Mathf.RoundToInt(hdrColor.a * 255f);
                }
                else
                {
                    // calibrate exposure to the max float color component
                    var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
                    exposure = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);
                    // maintain maximal integrity of byte values to prevent off-by-one errors when scaling up a color one component at a time
                    r = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * hdrColor.r));
                    g = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * hdrColor.g));
                    b = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * hdrColor.b));
                    a = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * hdrColor.a));
                }
                return new Color(r, g, b, a);
            }

            /// <summary>
            /// color 转换为 HDR Color
            /// </summary>
            /// <param name="rgbColor">RGB Color</param>
            /// <param name="intensity">强度</param>
            /// <returns>HDR Color</returns>
            public static Color RGBColorToHDR(Color rgbColor, float intensity)
            {
                float factor = Mathf.Pow(2, intensity);
                return new Color(rgbColor.r * factor, rgbColor.g * factor, rgbColor.b * factor);
            }

             */
        }

        public static class TIME
        {
            private static string m_TimeFormat = "yyyy-MM-dd HH:mm:ss";
            public static string TimeFormat => m_TimeFormat;
            public static int TimeStampNow => TimeStamp(DateTime.Now);
            public static string TimeNowString => DateTime.Now.ToString(TimeFormat);
            public static DateTime nowTime => DateTime.Now;

            /// <summary>
            /// 转换为时间戳
            /// </summary>
            /// <param name="time">指定的时间</param>
            /// <returns>时间戳(1970.1.1)</returns>
            public static int TimeStamp(DateTime time)
            {
                TimeSpan ts = time - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt32(ts.TotalSeconds);
            }

            /// <summary>
            /// 转换为 TimeFormat 的 DetaTime
            /// </summary>
            /// <param name="timeStr"></param>
            /// <returns></returns>
            public static DateTime TimeStringToTime(string timeString)
            {
                DateTime dateTime;

                if (!string.IsNullOrEmpty(timeString) && DateTime.TryParse(timeString, out dateTime)) return dateTime;
                else throw new ArgumentNullException("ToTime [timeStr" + timeString + "] is IsNullOrEmpty");
            }

            /// <summary>
            /// 将 second 转换为 时 分 秒
            /// </summary>
            public static void GetHMS(int second, out int hour, out int minute, out int millisecond)
            {
                hour = second / 3600;
                minute = (second - (hour * 3600)) / 60;
                millisecond = second - (hour * 3600) - (minute * 60);
            }
        }

        public static class Object
        {
            /// <summary>
            /// 销毁所有子项 GameObject
            /// </summary>
            /// <param name="transform"></param>
            public static void DestroyAllChild(Transform transform)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i));
                }
            }

            /// <summary>
            /// 安全销毁 GameObject
            /// </summary>
            /// <param name="gameObject"></param>
            public static void Destroy(GameObject gameObject)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);

#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    GameObject.Destroy(gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(gameObject);
                }
#else
                GameObject.Destroy(gameObject);
#endif
            }

            /// <summary>
            /// 安全销毁 GameObject
            /// </summary>
            public static void Destroy(Transform transform)
            {
                Destroy(transform.gameObject);
            }

            /// <summary>
            /// 销毁 GameObject 中的<T>类型组件
            /// </summary>
            public static void Destroy<T>(GameObject selectedObject) where T : UnityEngine.Component
            {
                if (selectedObject.TryGetComponent<T>(out var obj))
                {
                    if (Application.isPlaying)
                        GameObject.Destroy(obj);
                    else
                        GameObject.DestroyImmediate(obj);
                }
            }

            /// <summary>
            /// 寻找第一个符合名字的 Transform
            /// </summary>
            public static Transform FindChild(Transform transform, string name_)
            {
                var child = transform.GetComponentsInChildren<Transform>();
                for (int i = 0; i < child.Length; i++)
                {
                    if (child[i].name.Equals(name_))
                    {
                        return child[i];
                    }
                }
                return null;
            }

            /// <summary>
            /// 尝试寻找
            /// </summary>
            public static bool TryFind(string name, out GameObject obj)
            {
                obj = GameObject.Find(name);
                return obj != null;
            }
        }

        public static class Geometry
        {
            /// <summary>
            /// 叉积
            /// </summary>
            public static float Cross(Vector2 a, Vector2 b)
            {
                return a.x * b.y - b.x * a.y;
            }

            /// <summary>
            /// 角度标准化到[-180,180]区间内
            /// </summary>
            public static float Angle_f180_180(float angle)
            {
                angle %= 360f;
                return (Mathf.Abs(angle) > 180f) ? (angle > 0 ? angle - 360f : angle + 360f) : angle;
            }

            /// <summary>
            /// 角度标准化到[0,360]区间内
            /// </summary>
            public static float Angle_0_360(float angle)
            {
                angle %= 360f;
                return angle >= 0 ? angle : angle + 360f;
            }

            /// <summary>
            /// x -> y ,y -> z ,z -> x
            /// </summary>
            /// <returns></returns>
            public static Vector3 AxisOrderNext(Vector3 axis)
            {
                return new Vector3(axis.z, axis.x, axis.y);
            }

            /// <summary>
            /// 获取世界坐标系下的轴向角度
            /// </summary>
            /// <param name="axis">单位轴向</param>
            /// <returns></returns>
            public static float GetAxisWorldAngle(Transform transform, Vector3 axis)
            {
                return Vector3.SignedAngle(UniUtility.Geometry.AxisOrderNext(axis), Vector3.Scale(transform.rotation * UniUtility.Geometry.AxisOrderNext(axis), Vector3.one - axis).normalized, axis);
            }

            /// <summary>
            /// limt.x < 0 && limt.y >=0,则从与y == 0 开始，否则则从limt.x开始
            /// </summary>
            /// <param name="limt">限制范围</param>
            /// <param name="t">弧度</param>
            /// <returns></returns>
            public static float SinLimt(Vector2 limt, float t)
            {
                if (limt.x < 0 && limt.y >= 0)
                {
                    float c = limt.y - limt.x;
                    float f = (-limt.x * 2) / c - 1;
                    return (Mathf.Sin(t + Mathf.Asin(f)) + 1f) / 2f * c + limt.x;
                }
                else
                {
                    return (Mathf.Sin(t - 1.590f) + 1f) / 2f * (limt.y - limt.x) + limt.x;
                }
            }

            /// <summary>
            /// 输入矢量逆时针旋转 a 弧度
            /// </summary>
            /// <param name="v">方向</param>
            /// <param name="a">弧度</param>
            /// <returns>逆时针旋转后的方向,保留原大小</returns>
            public static Vector2 Rotate(Vector2 v, float a)
            {
                Vector2 n = v.normalized;
                a += Mathf.Atan2(n.y, n.x);
                return new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * v.magnitude;
            }

            /// <summary>
            /// 将矢量根据指定Quat的Y轴顺时针旋转 angle
            /// </summary>
            /// <param name="v">矢量</param>
            /// <param name="angle">角度</param>
            /// <param name="quat">旋转空间</param>
            /// <returns>旋转后的矢量</returns>
            public static Vector3 Rotate(Vector3 v, float angle, Quaternion quat)
            {
                return (quat * (Quaternion.Euler(0, angle, 0) * (Quaternion.LookRotation(v.normalized) * Quaternion.Inverse(quat)))) * Vector3.back * v.magnitude;
            }

            /// <summary>
            /// 将矢量围着指定轴向进行顺时针旋转
            /// vector 与 axial 的叉积为轴的正向
            /// </summary>
            /// <param name="vector">矢量</param>
            /// <param name="axial">轴向</param>
            /// <param name="angle">角度</param>
            public static Vector3 Rotate(Vector3 vector, Vector3 axial, float angle)
            {
                return Rotate(vector, axial, Vector3.Cross(vector, axial), angle);
            }

            /// <summary>
            /// 将矢量围着指定轴向进行顺时针旋转
            /// </summary>
            /// <param name="vector">矢量</param>
            /// <param name="axial">轴向</param>
            /// <param name="forward">轴的正向</param>
            /// <param name="angle">角度</param>
            public static Vector3 Rotate(Vector3 vector, Vector3 axial, Vector3 forward, float angle)
            {
                return Rotate(vector, Quaternion.LookRotation(forward, axial), angle);
            }

            /// <summary>
            /// 将矢量围着指定轴向(Y)进行顺时针旋转
            /// </summary>
            /// <param name="vector">矢量</param>
            /// <param name="axisQuat">轴旋转</param>
            /// <param name="angle">角度</param>
            public static Vector3 Rotate(Vector3 vector, Quaternion axisQuat, float angle)
            {
                return axisQuat * (Quaternion.Euler(0, angle, 0) * (Quaternion.Inverse(axisQuat) * Quaternion.LookRotation(vector))) * Vector3.forward * vector.magnitude;
            }

            /// <summary>
            /// 点到线的垂直交点
            /// </summary>
            /// <param name="lp">线的位置</param>
            /// <param name="ld">线方向</param>
            /// <param name="p">点的世界位置</param>
            public static Vector2 PointToLinePos(Vector2 lp,Vector2 ld, Vector2 p)
            {
                return Vector2.Dot(p - lp, ld) * ld + lp;
            }

            /// <summary>
            /// 两线段是否相交
            /// </summary>
            public static bool SegmentIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
            {
                //以线段ab为准，是否c d在同一侧
                Vector2 ab = b - a;
                Vector2 ac = c - a;
                Vector2 ad = d - a;

                if (Cross(ab, ac) * Cross(ab, ad) >= 0) return false;

                //以线段cd为准，是否a b在同一侧
                Vector2 cd = d - c;
                Vector2 ca = a - c;
                Vector2 cb = b - c;

                if (Cross(cd, ca) * Cross(cd, cb) >= 0) return false;

                return true;
            }

            /// <summary>
            /// 两线交点位置
            /// </summary>
            /// <param name="a">seg0l</param>
            /// <param name="b">seg0r</param>
            /// <param name="c">seg1l</param>
            /// <param name="d">seg1r</param>
            /// <returns></returns>
            public static Vector2 LineIntersectionPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
            {
                float t = Cross(a - c, d - c) / Cross(d - c, b - a);
                float dx = t * (b.x - a.x);
                float dy = t * (b.y - a.y);
                return new Vector2(a.x + dx, a.y + dy);
            }

            /// <summary>
            /// 点是否在集合轮廓内
            /// 忽略相交等特殊情况
            /// </summary>
            /// <param name="p">点位置</param>
            /// <param name="ps">轮廓</param>
            /// <param name="maxDia">轮廓最大直径</param>
            /// <returns></returns>
            public static bool PointInContour(Vector2 p, List<Vector2> ps, float maxDia = 10000)
            {
                int count = 0;
                Vector2 r = new Vector2(maxDia, 1);
                for (int i = 0; i < ps.Count; i++)
                {
                    Vector2 a = ps[i];
                    Vector2 b = ps[(i + 1) % ps.Count];
                    //Debug.DrawLine(a, b);
                    if (SegmentIntersection(p, p + r, a, b))
                    {
                        count++;
                    }
                }
                return count % 2 == 1;
            }

            /// <summary>
            /// 点距离轮廓边缘最近的一点位置的距离
            /// </summary>
            /// <param name="p">点位置</param>
            /// <param name="ps">轮廓</param>
            /// <param name="maxDia">轮廓最大直径</param>
            /// <returns></returns>
            public static float PointInContourDis(Vector2 p, List<Vector2> ps,int maxDia = 10000)
            {
                float result = float.MaxValue;
                int count = 0;
                Vector2 r = new Vector2(maxDia, 1);
                for (int i = 0; i < ps.Count; i++)
                {
                    Vector2 a = ps[i];
                    Vector2 b = ps[(i + 1) % ps.Count];
                    //Debug.DrawLine(a, b);
                    if (SegmentIntersection(p, p + r, a, b))
                    {
                        count++;
                    }
                    float d = Vector2.Distance(p, a);
                    if (d < result)
                    {
                        result = d;
                    }
                }

                return result * (count % 2 == 1 ? 1 : -1);
            }

            /// <summary>
            /// 是否为顺时针三角
            /// </summary>
            public static bool IsRight(Vector2 a, Vector2 b, Vector2 c)
            {
                Vector2 bc = (c - b).normalized;
                Vector2 ab = (b - a).normalized;
                Vector2 aTb = new Vector2(-ab.y, ab.x);

                return Vector2.Dot(bc, aTb) < 0;
            }

            /// <summary>
            /// Bezier 插值
            /// </summary>
            public static Vector2 Bezier(Vector2 a, Vector2 a1, Vector2 b, Vector2 b1, float t)
            {
                Vector2 p = Mathf.Pow(1 - t, 3) * a;
                p += 3 * t * Mathf.Pow(1 - t, 2) * a1;
                p += 3 * t * t * (1 - t) * b1;
                p += Mathf.Pow(t, 3) * b;

                return p;
            }

            /// <summary>
            /// 点是否在 Cube 矩阵内
            /// </summary>
            /// <param name="matrix">Cube 矩阵 (位置，缩放，方向)</param>
            /// <param name="point">点位置</param>
            public static bool MatrixContains(Matrix4x4 matrix, Vector3 point)
            {
                Vector3 lp = Quaternion.Inverse(matrix.rotation) * (point - matrix.GetPosition());

                return !(Mathf.Abs(lp.x * 2f) > matrix.lossyScale.x || Mathf.Abs(lp.y * 2f) > matrix.lossyScale.y || Mathf.Abs(lp.z * 2f) > matrix.lossyScale.z);
            }

            /// <summary>
            /// 有方向的一点 是否在 Cube 矩阵内，并与Cube的夹角小于 angle
            /// </summary>
            /// <param name="matrix">Cube 矩阵 (位置，缩放，方向)</param>
            /// <param name="point">点位置</param>
            /// <param name="forward">点方向</param>
            /// <param name="angle">最小夹角</param>
            /// <returns></returns>
            public static bool MatrixContainsAngle(Matrix4x4 matrix, Vector3 point, Vector3 forward, float angle = 0)
            {
                if (angle <= 0) return MatrixContains(matrix, point);

                Vector3 lp = Quaternion.Inverse(matrix.rotation) * (point - matrix.GetPosition());

                bool contain = !(Mathf.Abs(lp.x * 2f) > matrix.lossyScale.x || Mathf.Abs(lp.y * 2f) > matrix.lossyScale.y || Mathf.Abs(lp.z * 2f) > matrix.lossyScale.z);

                if (!contain) return false;

                return Vector3.Angle(matrix.rotation * Vector3.forward, forward) < angle;
            }

            /// <summary>
            /// 射线和面是否有交点 
            /// </summary>
            /// <param name="ray">射线</param>
            /// <param name="normal">面的法线</param>
            /// <param name="Point">面上的一点</param>
            /// <param name="ret">交点</param>
            /// <returns>线和面是否相交</returns>
            public static bool IntersectionOfRayAndFace(Ray ray, Vector3 normal, Vector3 Point, out Vector3 ret)
            {
                if (Vector3.Dot(ray.direction, normal) == 0)
                {
                    //如果平面法线和射线垂直 则不会相交
                    ret = Vector3.zero;
                    return false;
                }
                Vector3 Forward = normal;
                Vector3 Offset = Point - ray.origin; //获取线的方向
                float DistanceZ = Vector3.Angle(Forward, Offset); //计算夹角
                DistanceZ = Mathf.Cos(DistanceZ / 180f * Mathf.PI) * Offset.magnitude; //算点到面的距离
                DistanceZ /= Mathf.Cos(Vector3.Angle(ray.direction, Forward) / 180f * Mathf.PI); //算点沿射线到面的距离
                ret = ray.origin + ray.direction * DistanceZ; //算得射线和面的交点
                return true;
            }
        }

        public static class Array
        {
            /// <summary>
            /// 输出字符串
            /// </summary>
            public static string ToString<T>(IEnumerable<T> collection)
            {

                string ms = "( ";
                foreach (var item in collection)
                {
                    ms += "[" + item.ToString() + "] ";
                }

                return ms + ")";
            }
        }

        public static class CLASS
        {
            /// <summary>
            /// 深拷贝
            /// </summary>
            public static T DeepCopyByReflection<T>(T obj)
            {
                if (obj is string || obj.GetType().IsValueType)
                    return obj;

                object retval = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    try
                    {
                        field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
                    }
                    catch { }
                }

                return (T)retval;
            }
        }

        public static class LOG
        {
            private static Dictionary<int, StringBuilder> _mgs;

            private static Dictionary<int, StringBuilder> Mgs
            {
                get
                {
                    if (_mgs == null) _mgs = new Dictionary<int, StringBuilder>();
                    return _mgs;
                }
            }

            /// <summary>
            /// 清楚缓存
            /// </summary>
            public static void Clear()
            {
                if (Mgs != null) Mgs.Clear(); _mgs = null;
            }

            /// <summary>
            /// 清除指定缓存
            /// </summary>
            public static void Clear(int id)
            {
                if (Mgs.ContainsKey(id))
                {
                    Mgs[id].Clear();
                    Mgs.Remove(id);

                    if (_mgs.Count == 0) _mgs = null;
                }
            }

            /// <summary>
            /// 输出指定日志集合
            /// </summary>
            public static void Pop(int id)
            {
                if (Mgs.ContainsKey(id))
                {
                    Debug.Log(Mgs[id].ToString());
                    Mgs[id].Clear();
                    Mgs.Remove(id);

                    if (_mgs.Count == 0) _mgs = null;
                }
            }

            /// <summary>
            /// 输入日志到指定集合
            /// </summary>
            public static void Push(int id, object value)
            {
                if (!Mgs.ContainsKey(id)) Mgs.Add(id, new StringBuilder());

                Mgs[id].Append(value);
            }

            /// <summary>
            /// 输入日志到指定集合，并换行
            /// </summary>
            public static void PushLine(int id, object value)
            {
                if (!Mgs.ContainsKey(id)) Mgs.Add(id, new StringBuilder());

                if (Mgs[id].Length != 0) Mgs[id].AppendLine();
                Mgs[id].Append(value);
            }
        }

        public static class REFLECTION
        {
            /// <summary>
            /// 调用私有的非静态方法
            /// </summary>
            public static object GetMethod<T>(object obj, string funName, object[] parameters)
            {

                var mInfo = typeof(T).GetMethod(funName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                return mInfo.Invoke(obj, parameters);
            }

            /// <summary>
            /// 设置私有字段与属性
            /// </summary>
            public static void SetValue<T>(object obj, string parameterName, object parameters, bool isPrivate = true)
            {
                if (isPrivate)
                {
                    typeof(T).GetField(parameterName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(obj, parameters);
                }
                else
                {
                    typeof(T).GetField(parameterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(obj, parameters);
                }
            }

            /// <summary>
            /// 获取私有字段与属性
            /// </summary>
            public static object GetValue<T>(object obj, string parameterName)
            {
                return typeof(T).GetField(parameterName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(obj);
            }

            /// <summary>
            /// 调用私有的静态方法
            /// </summary>
            /// <param name="type">类的类型</param>
            /// <param name="method">类里要调用的方法名</param>
            /// <param name="parameters">调用方法传入的参数</param>
            public static object InvokeNonPublicStaticMethod(System.Type type, string method, params object[] parameters)
            {
                var methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
                if (methodInfo == null)
                {
                    UnityEngine.Debug.LogError($"{type.FullName} not found method : {method}");
                    return null;
                }
                return methodInfo.Invoke(null, parameters);
            }

            /// <summary>
            /// 调用公开的静态方法
            /// </summary>
            /// <param name="type">类的类型</param>
            /// <param name="method">类里要调用的方法名</param>
            /// <param name="parameters">调用方法传入的参数</param>
            public static object InvokePublicStaticMethod(System.Type type, string method, params object[] parameters)
            {
                var methodInfo = type.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
                if (methodInfo == null)
                {
                    UnityEngine.Debug.LogError($"{type.FullName} not found method : {method}");
                    return null;
                }
                return methodInfo.Invoke(null, parameters);
            }
        }

        public static class Collider
        {
            /// <summary>
            ///转换为 Cube 矩阵
            /// </summary>
            public static Matrix4x4 BoxColliderMat(BoxCollider boxCollider)
            {
                return Matrix4x4.TRS(boxCollider.transform.TransformPoint(boxCollider.center), Quaternion.Euler(boxCollider.transform.eulerAngles), boxCollider.size);
            }
        }
    }
}