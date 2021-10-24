/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: MultiThreadProcess.cs
* Author	: Administrator
* Create	: 2012/11/11 20:02:29
* Description	:
*
*/


using System;
using System.Collections.Generic;
using System.Threading;

namespace JR.Stand.Toolkit.Crawl.Impl
{
    ///
    /// 多线程处理事件
    ///
    public delegate void ThreadProcessHandler<T>(T t);

    ///
    /// 多线程处理
    ///
    public class MultiThreadProcess
    {
        ///
        /// 线程数量
        ///
        private int threadCount;

        ///
        ///  线程堆栈，用于轮询线程
        ///
        private Stack<int> threadStack;

        ///
        /// 线程集合
        ///
        private Thread[] threads;

        private bool isAlive = true;

        ///
        /// 是否活动中
        ///
        public bool IsAlive
        {
            get { return isAlive; }
        }

        public MultiThreadProcess(int threads, int processTimes)
        {
            this.threadCount = threads;
            this.threads = new Thread[threads];

            //初始化线程堆栈
            this.threadStack = new Stack<int>(processTimes);
            for (int i = processTimes; i > 0; i--)
            {
                this.threadStack.Push(i);
            }
        }

        /// <summary>
        /// 开始执行线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="t"></param>
        public void Start<T>(ThreadProcessHandler<T> handler, T t)
        {
            while (this.isAlive)
            {
                for (int i = 0; i < threadCount && this.threadStack.Count > 0; i++)
                {
                    if (threads[i] == null || threads[i].ThreadState == ThreadState.Stopped)
                    {
                        threads[i] = new Thread(() =>
                        {
                            lock (this.threadStack)
                            {
                                if (this.threadStack.Count > 0)
                                {
                                    this.threadStack.Pop();
                                    handler(t);
                                }
                            }
                        });

                        threads[i].Name = String.Format("thread{0}", i.ToString());
                        threads[i].Start();
                    }
                }

                if (this.threadStack.Count == 0)
                {
                    do
                    {
                        bool hasThreadRunning = false;
                        for (int i = 0; i < threadCount; i++)
                        {
                            if (this.threads[i] != null && this.threads[i].ThreadState == ThreadState.Running)
                                hasThreadRunning = true;
                        }
                        if (!hasThreadRunning)
                        {
                            this.isAlive = false;
                        }
                    } while (this.isAlive);

                    //设置线程任务完成
                    this.isAlive = false;
                }
            }
        }
    }
}