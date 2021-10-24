//
//
//  Copyright 2011 @ S1N1.COM.all rights reserved.
//
//  Project : Untitled
//  File Name : State.cs
//  Date : 2011/8/25
//  Author : 
//
//


namespace JR.Stand.Toolkit.Crawl.Impl
{
    /// <summary>
    /// �ɼ�״̬
    /// </summary>
    public class State
    {
        /// <summary>
        /// ����
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// ʧ����
        /// </summary>
        public int FailCount { get; internal set; }

        /// <summary>
        /// �ɹ���
        /// </summary>
        public int SuccessCount { get; internal set; }
    }
}