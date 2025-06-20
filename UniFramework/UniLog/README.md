# UniFramework.Log

一个轻量级的日志本地存储。



- 日志每行会自动分割为：[type]	[date]	[log]	at [stack]
- stack 信息只输出单行位置，定位于日志输出位置
- 手动执行写入流时，遇到非 *Log*|*Assert* 类型的日志会直接进行流写入，防止崩溃时的数据丢失
- 打包的软件在运行时会自动压缩日志为zip 格式(可用压缩软件打开阅读)，编辑环境不会
- 日志文件将以日期(yyyyMMddHHmmss) 命名进行保存，文件大于 **BREAKDISKLENGTH** 将会自动分卷，每次软件运行将会以新时间点创建日志文件
- 会自动删除 **MAXRETENTIONDAYS** 天数之外的日志



#### 范例

```
public class Boot : MonoBehaviour
{
    private void Awake()
    {
		//使用默认的启动配置(每次Debug.Log 都会执行写入流操作)，开始存储日志
        UniLog.Initalize();
        
        //或者利用UniLogDriver进行初始化(将会在FixedUpdate 中调用手动 ManualFlush() )
        //UniLogDriver.Initalize();
		
		//或者自动配置参数
		//UniLog.Instance.SetLogOptions(logEnable,filterLogType,editorCreate,isManualFlush);
		
		//在isManualFlush为true时，可手动将执行写入流操作（可在自定的Update 中调用）
		//UniLog.Instance.ManualFlush();
    }

    private void OnDestroy()
    {
        UniLog.Destroy();
    }
}
```

