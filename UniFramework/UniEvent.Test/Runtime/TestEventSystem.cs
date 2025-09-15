using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UniFramework.Event;

public class TestEventSystem : IPrebuildSetup, IPostBuildCleanup
{
    private class TestEvent : IEventMessage
    {
        public List<string> Logs = new List<string>();
        public void OnRecycle()
        {
            Logs.Clear();
        }
    }

    void IPrebuildSetup.Setup()
    {
    }
    void IPostBuildCleanup.Cleanup()
    {
    }

    [Test]
    public void A_InitializeYooAssets()
    {
        UniEvent.Initalize();
        UniEvent.BroadcastOrder = UniEvent.EBroadcastOrder.Normal;
    }

    [Test]
    public void B1_RemoveOtherListenersDuringBroadcast()
    {
        // 监听 A
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");

        // 监听 B（会在执行时移除 A 或 C）
        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
            UniEvent.RemoveListener<TestEvent>(ListenerA); // 移除 A
            UniEvent.RemoveListener<TestEvent>(ListenerC); // 移除 C
        }

        // 监听 C
        void ListenerC(IEventMessage msg) => ((TestEvent)msg).Logs.Add("C");

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);
        UniEvent.AddListener<TestEvent>(ListenerC);

        // 执行广播
        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 验证
        // 预期: A -> B，C 被移除未触发
        Assert.AreEqual(new[] { "A", "B" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void B2_RemoveSelfDuringBroadcast()
    {
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");

        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
            UniEvent.RemoveListener<TestEvent>(ListenerB); // 移除自己
        }

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);

        // 执行广播
        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 第一次广播: A -> B
        Assert.AreEqual(new[] { "A", "B" }, testEvent.Logs.ToArray());

        // 清空日志，第二次广播
        testEvent.Logs.Clear();
        UniEvent.SendMessage(testEvent);

        // 第二次广播: 只有 A 执行（B 已经被移除）
        Assert.AreEqual(new[] { "A" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void B3_AddNewListenerDuringBroadcast()
    {
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");

        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
            UniEvent.AddListener<TestEvent>(ListenerC); // 广播过程中新增
        }

        void ListenerC(IEventMessage msg) => ((TestEvent)msg).Logs.Add("C");

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);

        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 第一次广播: A -> B，C 不会立即生效
        Assert.AreEqual(new[] { "A", "B" }, testEvent.Logs.ToArray());

        // 清空日志，第二次广播
        testEvent.Logs.Clear();
        UniEvent.SendMessage(testEvent);

        // 第二次广播: A -> B -> C
        Assert.AreEqual(new[] { "A", "B", "C" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void B4_Reentrancy_PublishInsideListener()
    {
        void ListenerA(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("A");

            // 尝试在广播过程中再次触发同一事件
            UniEvent.SendMessage(new TestEvent { Logs = ((TestEvent)msg).Logs });
        }

        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
        }

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);

        // 监听日志输出
        var logMessages = new List<string>();
        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            if (type == LogType.Error)
                logMessages.Add(condition);
        };

        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;

        // 移除日志监听
        Application.logMessageReceived -= (condition, stackTrace, type) =>
        {
            if (type == LogType.Error)
                logMessages.Add(condition);
        };

        // 日志验证
        Assert.AreEqual(new[] { "A", "B" }, testEvent.Logs.ToArray());

        // 警告验证
        Assert.IsTrue(logMessages.Exists(msg => msg.Contains("Recursive Publish is not allowed")));

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void C1_ChangeBroadcastOrder()
    {
        UniEvent.ClearAll();
        UniEvent.BroadcastOrder = UniEvent.EBroadcastOrder.Reverse;
    }

    [Test]
    public void C2_ReverseOrder_Basic()
    {
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");
        void ListenerB(IEventMessage msg) => ((TestEvent)msg).Logs.Add("B");
        void ListenerC(IEventMessage msg) => ((TestEvent)msg).Logs.Add("C");

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);
        UniEvent.AddListener<TestEvent>(ListenerC);

        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 倒序执行: C -> B -> A
        Assert.AreEqual(new[] { "C", "B", "A" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void C3_ReverseOrder_RemoveLaterListener()
    {
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");

        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
            UniEvent.RemoveListener<TestEvent>(ListenerA); // 移除 A（后面才会执行）
        }

        void ListenerC(IEventMessage msg) => ((TestEvent)msg).Logs.Add("C");

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);
        UniEvent.AddListener<TestEvent>(ListenerC);

        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 倒序: C -> B，A 被移除未触发
        Assert.AreEqual(new[] { "C", "B" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }

    [Test]
    public void C4_ReverseOrder_AddNewListener()
    {
        void ListenerA(IEventMessage msg) => ((TestEvent)msg).Logs.Add("A");

        void ListenerB(IEventMessage msg)
        {
            ((TestEvent)msg).Logs.Add("B");
            UniEvent.AddListener<TestEvent>(ListenerD); // 广播过程中新增 D
        }

        void ListenerC(IEventMessage msg) => ((TestEvent)msg).Logs.Add("C");
        void ListenerD(IEventMessage msg) => ((TestEvent)msg).Logs.Add("D");

        UniEvent.AddListener<TestEvent>(ListenerA);
        UniEvent.AddListener<TestEvent>(ListenerB);
        UniEvent.AddListener<TestEvent>(ListenerC);

        var testEvent = new TestEvent();
        UniEvent.SendMessage(testEvent);

        // 第一次广播倒序: C -> B -> A，D 不会立即生效
        Assert.AreEqual(new[] { "C", "B", "A" }, testEvent.Logs.ToArray());

        // 清空日志，第二次广播
        testEvent.Logs.Clear();
        UniEvent.SendMessage(testEvent);

        // 第二次广播倒序: D -> C -> B -> A
        Assert.AreEqual(new[] { "D", "C", "B", "A" }, testEvent.Logs.ToArray());

        // 清理
        UniEvent.ClearAll();
    }


}