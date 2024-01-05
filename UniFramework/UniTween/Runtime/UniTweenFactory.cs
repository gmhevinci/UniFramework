using System.Diagnostics;

namespace UniFramework.Tween
{
    internal static class UniTweenFactory
    {
        /// <summary>
        /// 执行节点
        /// </summary>
        /// <param name="execute">执行方法</param>
        public static ExecuteNode Execute(System.Action execute)
        {
            ExecuteNode node = new ExecuteNode();
            node.SetExecute(execute);
            return node;
        }

        /// <summary>
        /// 条件等待节点
        /// </summary>
        /// <param name="condition">条件方法</param>
        public static UntilNode Until(System.Func<bool> condition)
        {
            UntilNode node = new UntilNode();
            node.SetCondition(condition);
            return node;
        }


        /// <summary>
        /// 并行执行的复合节点
        /// </summary>
        /// <param name="nodes">成员节点列表</param>
        public static ParallelNode Parallel(params ITweenNode[] nodes)
        {
            ParallelNode node = new ParallelNode();
            node.AddNode(nodes);
            return node;
        }

        /// <summary>
        /// 顺序执行的复合节点
        /// </summary>
        /// <param name="nodes">成员节点列表</param>
        public static SequenceNode Sequence(params ITweenNode[] nodes)
        {
            SequenceNode node = new SequenceNode();
            node.AddNode(nodes);
            return node;
        }

        /// <summary>
        /// 随机执行的复合节点
        /// </summary>
        /// <param name="nodes">成员节点列表</param>
        public static SelectorNode Selector(params ITweenNode[] nodes)
        {
            SelectorNode node = new SelectorNode();
            node.AddNode(nodes);
            return node;
        }


        /// <summary>
        /// 延迟计时节点
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="trigger">触发事件</param>
        public static TimerNode Delay(float delay, System.Action trigger = null)
        {
            UniTimer timer = UniTimer.CreateOnceTimer(delay);
            TimerNode node = new TimerNode(timer);
            node.SetTrigger(trigger);
            return node;
        }

        /// <summary>
        /// 持续计时节点
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="duration">持续时间</param>
        /// <param name="trigger">触发事件</param>
        public static TimerNode Duration(float delay, float duration, System.Action trigger = null)
        {
            UniTimer timer = UniTimer.CreateDurationTimer(delay, duration);
            TimerNode node = new TimerNode(timer);
            node.SetTrigger(trigger);
            return node;
        }

        /// <summary>
        /// 重复计时节点
        /// 注意：该节点为无限时长
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="trigger">触发事件</param>
        public static TimerNode Repeat(float delay, float interval, System.Action trigger = null)
        {
            UniTimer timer = UniTimer.CreatePepeatTimer(delay, interval);
            TimerNode node = new TimerNode(timer);
            node.SetTrigger(trigger);
            return node;
        }

        /// <summary>
        /// 重复计时节点
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="duration">持续时间</param>
        /// <param name="trigger">触发事件</param>
        public static TimerNode Repeat(float delay, float interval, float duration, System.Action trigger = null)
        {
            UniTimer timer = UniTimer.CreatePepeatTimer(delay, interval, duration);
            TimerNode node = new TimerNode(timer);
            node.SetTrigger(trigger);
            return node;
        }

        /// <summary>
        /// 重复计时节点
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="maxRepeatCount">最大触发次数</param>
        /// <param name="trigger">触发事件</param>
        public static TimerNode Repeat(float delay, float interval, long maxRepeatCount, System.Action trigger = null)
        {
            UniTimer timer = UniTimer.CreatePepeatTimer(delay, interval, maxRepeatCount);
            TimerNode node = new TimerNode(timer);
            node.SetTrigger(trigger);
            return node;
        }
    }
}