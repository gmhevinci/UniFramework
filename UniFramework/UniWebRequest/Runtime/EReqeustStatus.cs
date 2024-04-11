
namespace UniFramework.WebRequest
{
    public enum EReqeustStatus
    {
        None,

        /// <summary>
        /// 进行中
        /// </summary>
        InProgress,

        /// <summary>
        /// 成功
        /// </summary>
        Succeed,

        /// <summary>
        /// 与服务器通信失败。
        /// 例如：请求无法连接或无法建立安全通道。
        /// </summary>
        ConnectionError,

        /// <summary>
        /// 服务器返回一个错误响应。
        /// 说明：与服务器通信成功，但收到连接协议定义的错误，具体原因查看ResponseCode。
        /// </summary>
        ProtocolError,

        /// <summary>
        /// 数据处理错误。
        /// 说明：与服务器通信成功，但是在处理接收到的数据时遇到了错误。例如，数据损坏或格式不正确。
        /// </summary>
        DataProcessingError,
    }
}