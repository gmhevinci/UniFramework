using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Network
{
    /// <summary>
    /// 网络包解码器
    /// </summary>
    public class DefaultNetPackageDecoder : INetPackageDecoder
    {
        private HandleErrorDelegate _handleErrorCallback;
        private const int HeaderMsgIDFiledSize = 4; //包头里的协议ID（int类型）
        private const int HeaderMsgBodyLengthFiledSize = 4; //包头里的包体长度（int类型）

        /// <summary>
        /// 获取包头的尺寸
        /// </summary>
        public int GetPackageHeaderSize()
        {
            return HeaderMsgIDFiledSize + HeaderMsgBodyLengthFiledSize;
        }

        /// <summary>
        /// 注册异常错误回调方法
        /// </summary>
        /// <param name="callback"></param>
        public void RigistHandleErrorCallback(HandleErrorDelegate callback)
        {
            _handleErrorCallback = callback;
        }

        /// <summary>
        /// 网络消息解码
        /// </summary>
        /// <param name="packageBodyMaxSize">包体的最大尺寸</param>
        /// <param name="ringBuffer">解码需要的字节缓冲区</param>
        /// <param name="outputPackages">接收的包裹列表</param>
        public void Decode(int packageBodyMaxSize, RingBuffer ringBuffer, List<INetPackage> outputPackages)
        {
            // 循环解包
            while (true)
            {
                // 如果数据不够一个包头
                if (ringBuffer.ReadableBytes < GetPackageHeaderSize())
                    break;
                ringBuffer.MarkReaderIndex();

                // 读取包头数据
                int msgID = ringBuffer.ReadInt();
                int msgBodyLength = ringBuffer.ReadInt();

                // 如果剩余可读数据小于包体长度
                if (ringBuffer.ReadableBytes < msgBodyLength)
                {
                    ringBuffer.ResetReaderIndex();
                    break; //需要退出读够数据再解包
                }

                DefaultNetPackage package = new DefaultNetPackage();
                package.MsgID = msgID;

                // 检测包体长度
                if (msgBodyLength > packageBodyMaxSize)
                {
                    _handleErrorCallback(true, $"The decode package {package.MsgID} body size is larger than {packageBodyMaxSize} !");
                    break;
                }

                // 读取包体
                {
                    package.BodyBytes = ringBuffer.ReadBytes(msgBodyLength);
                    outputPackages.Add(package);
                }
            }

            // 注意：将剩余数据移至起始
            ringBuffer.DiscardReadBytes();
        }
    }
}