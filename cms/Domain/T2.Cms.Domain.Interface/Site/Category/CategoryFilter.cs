using System;
using System.Collections.Generic;
using System.Linq;

namespace T2.Cms.Domain.Interface.Site.Category
{
    public static class CategoryFilter
    {

        /// <summary>
        /// 在集合中获取条件的分类
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="_categories"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<ICategory> GetCategories(int lft, int rgt, IEnumerable<ICategory> _categories,CategoryContainerOption option)
        {
            //如果在初始化,则直接用静态变量


            switch (option)
            {
                case CategoryContainerOption.Childs: if (rgt - lft == 1)
                    {
                        return new List<ICategory>();
                    }
                    else
                    {
                        // return _categories.Where(a => a.Lft > lft && a.Rgt < lft);
                         return _categories.Where(a => a.Lft > lft && a.Rgt < rgt);
                    }

                case CategoryContainerOption.ChildsAndSelf:
                //return _categories.Where(a => a.Lft >= lft && a.Rgt <= lft);
                return _categories.Where(a => a.Lft >= lft && a.Rgt <= rgt);

                case CategoryContainerOption.Parents:
                    return _categories.Where(a => a.Lft < lft && a.Rgt > rgt && a.Lft != 1);

                case CategoryContainerOption.ParentsAndSelf:
                    return _categories.Where(a => a.Lft <= lft && a.Rgt >= rgt && a.Lft != 1);

                case CategoryContainerOption.NextLevel:
                    /*无子节点
                     *当左值+1==右值时，该节点没有子节点，则下一节点不为其子节点
                     *若下一节点的左值==上一节点右值+1，则2个节点是同级关系
                     */
                    if (rgt - lft == 1)
                    {
                        return new List<ICategory>();
                    }
                    else
                    {
                        return GetNextLevelCategories(_categories.Where(a => a.Lft > lft && a.Rgt < rgt));
                    }
                case CategoryContainerOption.SameLevel:
                    return GetSameLevelCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelNext:
                    return GetSameLevelNextCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelPrevious:
                    return GetSameLevelPreviousCategories(lft, rgt, _categories);

            }
            return null;
        }


        /// <summary>
        /// 获取相同节点的接点集合
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static IEnumerable<ICategory> GetSameLevelCategories(int lft, int rgt, IEnumerable<ICategory> _categories)
        {
            //
            //TODO:算法需重新实现，有误，不能获取前面的同级别栏目
            //

            /*无子节点
             *当左值+1==右值时，该节点没有子节点，则下一节点不为其子节点
             *若下一节点的左值==上一节点右值+1，则2个节点是同级关系
             */
            //ICategory c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            //if (c == null) yield return null;

            //ICategory c1 = c;

            //foreach (ICategory category in _categories)
            //{
            //    if (c1.Rgt + 1 == category.Lft || c1.Lft - 1 == category.Rgt || category.Lft == c.Lft)
            //    {
            //        c1 = category;
            //        yield return category;
            //    }
            //}


            ICategory category = GetCategories(lft, rgt, _categories, CategoryContainerOption.Parents).LastOrDefault();
            if (category == null) category = _categories.FirstOrDefault();
            return GetCategories(category.Lft, category.Rgt, _categories, CategoryContainerOption.NextLevel);

        }

        /// <summary>
        /// 获取同级上面的所有节点
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="_categories"></param>
        /// <returns></returns>
        public static IEnumerable<ICategory> GetSameLevelNextCategories(int lft, int rgt, IEnumerable<ICategory> _categories)
        {
            ICategory c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            if (c == null) yield return null;

            ICategory c1 = c;

            foreach (ICategory category in _categories)
            {
                if (c1.Rgt + 1 == category.Lft)
                {
                    c1 = category;
                    yield return category;
                }
            }
        }

        /// <summary>
        /// 获取同级下面的所有节点
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="_categories"></param>
        /// <returns></returns>
        public static IEnumerable<ICategory> GetSameLevelPreviousCategories(int lft, int rgt, IEnumerable<ICategory> _categories)
        {
            ICategory c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            if (c == null) yield return null;

            ICategory c1 = c;

            foreach (ICategory category in _categories)
            {
                if (c1.Lft - 1 == category.Rgt)
                {
                    c1 = category;
                    yield return category;
                }
            }
        }

        /// <summary>
        /// 获取下级的节点
        /// </summary>
        /// <param name="left"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        private static IEnumerable<ICategory> GetNextLevelCategories(IEnumerable<ICategory> categories)
        {
            /*无子节点
             *当左值+1==右值时，该节点没有子节点，则下一节点不为其子节点
             *若下一节点的左值==上一节点右值+1，则2个节点是同级关系
             */

            ICategory c = null;
            IList<ICategory> category1 = new List<ICategory>(categories);

            // 如果出现不连续，则可能出现少于实际栏目的情况
            foreach (ICategory c1 in categories)
            {
                if (c == null || c1.Lft - 1 == c.Rgt)
                {
                    c = c1;
                    yield return c1;
                }
            }
        }

        public static IEnumerable<ICategory> GetCategories(int catId, IList<ICategory> list, CategoryContainerOption option)
        {
            return list;
            /*
            //如果在初始化,则直接用静态变量
            switch (option)
            {
                case CategoryContainerOption.Childs:
                    if (rgt - lft == 1)
                    {
                        return new List<ICategory>();
                    }
                    else
                    {
                        // return _categories.Where(a => a.Lft > lft && a.Rgt < lft);
                        return _categories.Where(a => a.Lft > lft && a.Rgt < rgt);
                    }

                case CategoryContainerOption.ChildsAndSelf:
                    //return _categories.Where(a => a.Lft >= lft && a.Rgt <= lft);
                    return _categories.Where(a => a.Lft >= lft && a.Rgt <= rgt);

                case CategoryContainerOption.Parents:
                    return _categories.Where(a => a.Lft < lft && a.Rgt > rgt && a.Lft != 1);

                case CategoryContainerOption.ParentsAndSelf:
                    return _categories.Where(a => a.Lft <= lft && a.Rgt >= rgt && a.Lft != 1);

                case CategoryContainerOption.NextLevel:
             
                    if (rgt - lft == 1)
                    {
                        return new List<ICategory>();
                    }
                    else
                    {
                        return GetNextLevelCategories(_categories.Where(a => a.Lft > lft && a.Rgt < rgt));
                    }
                case CategoryContainerOption.SameLevel:
                    return GetSameLevelCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelNext:
                    return GetSameLevelNextCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelPrevious:
                    return GetSameLevelPreviousCategories(lft, rgt, _categories);

            }
            return null;*/
        }
    }
}
