/*
 * Copyright 2010 OPS,All rights reseved!
 * 
 * name     : ModuleType enum
 * author_id   : newmin
 * date     : 2013/05/16 11:12
 */

namespace T2.Cms.Domain.Interface.Site.Category
{
    /// <summary>
    /// 节点包含选项
    /// </summary>
    public enum CategoryContainerOption : int
    {
        /// <summary>
        /// 未指定
        /// </summary>
        NoSet = 0,

        /// <summary>
        /// 所有子类
        /// </summary>
        Childs = 1,

        /// <summary>
        /// 所有子类且包含自己
        /// </summary>
        ChildsAndSelf = 2,

        /// <summary>
        /// 所有父类
        /// </summary>
        Parents = 3,

        /// <summary>
        /// 所有父类包含自己
        /// </summary>
        ParentsAndSelf = 4,

        /// <summary>
        /// 上一级父节点
        /// </summary>
        PreviousLevel = 5,

        /// <summary>
        /// 子节点第一级集合
        /// </summary>
        NextLevel = 6,

        /// <summary>
        /// 同一级
        /// </summary>
        SameLevel = 7,

        /// <summary>
        /// 同一级后面的节点
        /// </summary>
        SameLevelNext=8,

        /// <summary>
        /// 同一级前面的节点
        /// </summary>
        SameLevelPrevious=9
    }
}