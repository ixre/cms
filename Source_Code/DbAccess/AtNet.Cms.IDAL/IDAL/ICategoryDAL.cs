//
//
//  Copyright 2011 (C) OPSoft INC.All rights reseved.
//
//  Project : OSite
//  File Name : ICategoryDAL.cs
//  Date : 2011/8/22
//  Author : 
//
//

namespace Spc.IDAL
{
    using System.Data;
    using Ops.Data;

    //栏目数据接口
    public interface ICategoryDAL
    {
        void Insert(int siteID,int lft, int rgt, int moduleID, string name, string alias, string pagetitle, string keywords, string description, int orderIndex);
       

        /// <summary>
        /// 更新栏目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="moduleID"></param>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="keywords"></param>
        /// <param name="description"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        bool Update(int id, int lft, int rgt, int moduleID, string name, string alias, string pagetitle, string keywords, string description, int orderIndex);

        /// <summary>
        /// 删除指定lft,rgt的几以下的栏目
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <returns></returns>
        bool Delete(int lft,int rgt);

        /// <summary>
        /// 获取所有栏目
        /// </summary>
        /// <param name="func"></param>
        void GetAllCategories(DataReaderFunc func);

        
        /// <summary>
        /// 添加子节点时更新左右值
        /// </summary>
        /// <param name="left"></param>
        void UpdateInsertLftRgt(int left);

        /// <summary>
        /// 删除节点时更新左右值
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        void UpdateDeleteLftRgt(int lft, int rgt);


        /// <summary>
        /// 转移目标栏目更新左右值
        /// </summary>
        /// <param name="toLeft"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        void UpdateMoveLftRgt(int toLeft, int left, int right);

        void UpdateMoveLftRgt2(int toRgt, int left, int right);

        /// <summary>
        /// 获取最大的右值
        /// </summary>
        /// <returns></returns>
        int GetMaxRight();

    }
}
