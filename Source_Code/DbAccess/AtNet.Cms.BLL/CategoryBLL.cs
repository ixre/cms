//
// ------------------------------------
// CategoryDBLL
// Copyright 2011 @ OPS
// author:newmin
// date:2010/04/08 14:05
// --------------------------------------
//
namespace Spc.BLL
{
    using DAL;
    using IDAL;
    using Models;
    using Ops.Cms.Domain.Interface.Site.Category;
    using Ops.Cms.Infrastructure;
    using Spc.Logic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    /// <summary>
    /// 栏目商务逻辑层
    /// </summary>
    public class CategoryBLL : ICategoryModel
    {
    	
        private static CategoryDAL _dal;
        private static ArchiveDAL _adal;
        
        private static CategoryDAL dal
        {
        	get{return _dal??(_dal=new CategoryDAL());}
        	
        }
        private static ArchiveDAL adal
        {
        	get{return _adal??(_adal=new ArchiveDAL());}
        	
        }
        
        private static IList<Category> categories = new List<Category>();

        //栏目的弱引用,保证释放资源时回收
        private static WeakReference cateRef;

        /// <summary>
        /// 从内存中清除栏目
        /// </summary>
        /// <returns></returns>
        internal static void Clear()
        {
            categories = null;
        }

        /// <summary>
        /// 重建栏目缓存
        /// </summary>
        public void RebuildCache()
        {
            //释放资源
            cateRef = null;
            
            //重新赋值
            if (categories != null)
            {
                while (categories.Count != 0)
                {
                    categories.Remove(categories[0]);
                }
            }

            int readerCount = 0;
            int categoryCount = categories.Count;

            dal.GetAllCategories(reader =>
            {

                while (reader.Read())
                {
                    ++readerCount;

                    categories.Add(new Category
                    {
                        ID = reader.GetInt32(0),
                        Name = reader["name"].ToString(),
                        Lft = int.Parse(reader["lft"].ToString()),
                        Rgt = int.Parse(reader["rgt"].ToString()),
                        Keywords = reader["keywords"].ToString(),
                        Description = reader["description"].ToString(),
                        ModuleID = int.Parse(reader["moduleid"].ToString()),
                        OrderIndex = int.Parse(reader["orderindex"].ToString()),
                        PageTitle = reader["pagetitle"].ToString(),
                        Tag = reader["tag"].ToString(),
                        SiteID=int.Parse(reader["siteid"].ToString())
                    });

                }
                //if (reader.HasRows)
                //{
                //    categories = reader.ToEntityList<Category>();
                //}
            });


            Category rootCategory = categories.FirstOrDefault(a => a.Lft == 1);

            if (rootCategory == null)
            {
                throw new Exception("根节点已经被破坏!");
            }



            //对分级的子级赋值
            ForeachSetChildCategories(rootCategory);


            //指定弱引用
            cateRef = new WeakReference(categories);
        }
    

        /// <summary>
        /// 根节点
        /// </summary>
        public Category Root
        {
            get
            { //如果在初始化,则直接用静态变量
                if (cateRef == null)
                {
                    if (categories == null)
                    {
                        this.RebuildCache();
                    }
                    return categories.FirstOrDefault(a => a.Lft == 1);
                }
                else
                {
                    return GetCategories().FirstOrDefault(a => a.Lft == 1);
                }

            }
        }

        /// <summary>
        /// 获取所有栏目
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetCategories()
        {
            if (cateRef==null || cateRef.Target == null)
            {
                this.RebuildCache();
            }
            return cateRef.Target as IList<Category>;
        }


        /// <summary>
        /// 对分级的子级赋值
        /// </summary>
        /// <param name="category"></param>
        private void ForeachSetChildCategories(Category category)
        {

            //对分级的子级赋值
            category.NextLevelCategories = this.GetCategories(category.Lft, category.Rgt, CategoryContainerOption.NextLevel);

            foreach (Category c in category.NextLevelCategories)
            {
                ForeachSetChildCategories(c);
            }
            
        }



        /// <summary>
        /// 迭代遍历栏目及子栏目
        /// </summary>
        /// <param name="category"></param>
        /// <param name="func"></param>
        public void HandleCategoryTree(string categoryName,CategoryTreeHandler func)
        {
            /*
             * 好吧，现在整个树都在一个查询中了。现在就要像前面的递归函数那样显示这个树，
             * 我们要加入一个ORDER BY子句在这个查询中。如果你从表中添加和删除行，你的表可能就顺序不对了，
             * 我们因此需要按照他们的左值来进行排序。
             * 
             * SELECT * FROM tree WHERE lft BETWEEN 2 AND 11 ORDER BY lft ASC;
             * 
             * 就只剩下缩进的问题了。要显示树状结构，子节点应该比他们的父节点稍微缩进一些。
             * 我们可以通过保存一个右值的一个栈。每次你从一个节点的子节点开始时，
             * 你把这个节点的右值 添加到栈中。你也知道子节点的右值都比父节点的右值小，
             * 这样通过比较当前节点和栈中的前一个节点的右值，你可以判断你是不是在
             * 显示这个父节点的子节点。当你显示完这个节点，你就要把他的右值从栈中删除。
             * 要获得当前节点的层数，只要数一下栈中的元素。
             * 
             * 
             */

            int rootLft,
                rootRgt;

            IList<int> arr = new List<int>();

           // 获得root节点的左边和右边的值
            Category root = this.Get(a => a.Name == categoryName);
            if (root == null) return;

            rootLft = root.Lft;
            rootRgt = root.Rgt;

            //获取root节点的所有子节点
            var childNodes = this.GetCategories(a => a.Lft > rootLft && a.Lft < rootRgt).OrderBy(a=>a.Lft);
            /* SELECT * FROM tree WHERE lft BETWEEN @rootLft AND @rootRgt ORDER BY lft ASC'); */

            foreach (Category c in childNodes)
            {
                if (arr.Count > 0)
                {
                    //判断最后一个值是否小于
                    int i;
                    while ((i = arr[arr.Count - 1]) < c.Rgt)
                    {
                        arr.Remove(i);
                        if (arr.Count == 0) break;
                    }
                }


                //树的层级= 列表arr的数量
                func(c, arr.Count);

                //把所有栏目的右值,再加入到列表中
                arr.Add(c.Rgt);
            }


           // int right = left + 1;

            //func(category);
           // foreach (Category c in this.GetCategories(a => a.PID == category.ID))
           // {
            //    Foreach(c, func);
           // }
        }
        
        

        /// <summary>
        /// 按栏目列表生成树,list[0]默认为根节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="func"></param>
        public void HandleCategoryTree(IEnumerable<Category> list, CategoryTreeHandler func)
        {
            int rootLft,
               rootRgt;

            IList<int> arr = new List<int>();

            // 获得root节点的左边和右边的值
            Category root =new List<Category>(list)[0];
            if (root == null) return;

            rootLft = root.Lft;
            rootRgt = root.Rgt;

            //获取root节点的所有子节点
            var childNodes = list.Where(a => a.Lft > rootLft && a.Rgt < rootRgt);
            /* SELECT * FROM tree WHERE lft BETWEEN @rootLft AND @rootRgt ORDER BY lft ASC'); */




            foreach (Category c in childNodes)
            {
                if (arr.Count > 0)
                {
                    //判断最后一个值是否小于
                    int i;
                    while ((i=arr[arr.Count-1])< c.Rgt)
                    {
                        arr.Remove(i);
                    }
                }

                
                //树的层级= 列表arr的数量
                func(c, arr.Count);

                arr.Clear();
                foreach (Category tc in childNodes)
                {
                    arr.Add(tc.Rgt);
                }
            }

        }

        /// <summary>
        /// 获取栏目
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategories(Func<Category, bool> func)
        {
            foreach (Category category in GetCategories())
            {
                if (func(category)) yield return category;
            }
        }

        /// <summary>
        /// 获取单个栏目
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Category Get(Func<Category, bool> func)
        {
            foreach (Category category in GetCategories())
            {
                if (func(category)) return category;
            }
            return null;
        }

        /// <summary>
        /// 获取父栏目
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Category GetParent(int left,int right)
        {
            Category category=null;

            var list = this.GetCategories(a => a.Lft < left && a.Rgt > right);
            foreach (Category c in list)
            {
                if (category == null || c.Lft > category.Lft) { category = c; }
            }
            return category;
        }

        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Category GetNext(int left, int right)
        {
            var list = new List<Category>(this.GetCategories(left, right, CategoryContainerOption.SameLevelNext));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        /// <summary>
        /// 获取上一个节点
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Category GetPrevious(int left, int right)
        {
            var list = new List<Category>(this.GetCategories(left, right, CategoryContainerOption.SameLevelPrevious));
            if (list.Count == 0)
            {
                return null;
            }
            return list[list.Count - 1];
        }


        /// <summary>
        /// 新增栏目
        /// </summary>
        /// <param name="c"></param>
        [Obsolete]
        public void Insert(int siteId,string parentName, int moduleID, string categoryName, string tag,string pagetitle, string keywords, string description, int orderIndex)
        {

            #region SQL
            /*
            * 添加同一层次的节点的方法如下：
            * 
            * Sql代码
            * LOCK TABLE nested_category WRITE;   
            * SELECT @myRight := rgt FROM nested_category   
            * WHERE name = 'Cherry';   
            * UPDATE nested_category SET rgt = rgt + 2 WHERE rgt > @myRight;   
            * UPDATE nested_category SET lft = lft + 2 WHERE lft > @myRight;   
            * INSERT INTO nested_category(name, lft, rgt) VALUES('Strawberry', @myRight + 1, @myRight + 2);   
            * UNLOCK TABLES;  
            * 
            * 添加树的子节点的方法如下：
            * 
            * Sql代码
            * LOCK TABLE nested_category WRITE;   
            * SELECT @myLeft := lft FROM nested_category   
            * WHERE name = 'Beef';   
            * UPDATE nested_category SET rgt = rgt + 2 WHERE rgt > @myLeft;   
            * UPDATE nested_category SET lft = lft + 2 WHERE lft > @myLeft;   
            * INSERT INTO nested_category(name, lft, rgt) VALUES('charqui', @myLeft + 1, @myLeft + 2);   
            * UNLOCK TABLES; 
            * 
            * 
            * 每次插入节点之后都可以用以下SQL进行查看验证：
            * SELECT CONCAT( REPEAT( ' ', (COUNT(parent.name) - 1) ), node.name) AS name  
            * FROM nested_category AS node,   
            * nested_category AS parent   
            * WHERE node.lft BETWEEN parent.lft AND parent.rgt   
            * GROUP BY node.name  
            * ORDER BY node.lft;  
            * 
            */
            #endregion

            //* 树状结构,只有一个根节点

            int left;
            //添加树的子节点方法
            if (String.IsNullOrEmpty(parentName))
            {
                parentName = this.GetCategories()[0].Name;
            }

            left = this.Get(a => a.Name == parentName).Lft;

            //更新
            dal.UpdateInsertLftRgt(siteId,left);

            dal.Insert(siteId,left + 1, left + 2, moduleID, categoryName, tag, pagetitle, keywords, description,"", orderIndex);


            //更新缓存
            this.RebuildCache();
        }
       
        /// <summary>
        /// 更新栏目
        /// </summary>
        public void Update(Category entity, int parentLft)
        {

            /*
             * refence url: http://wizardmin.com/2012/08/left-right-code-tree/
             * 
             * 移动操作
             * 主要是改变父节点和同层节点调换操作
             * 移动操作基本基于一个公式：任何树所占的数字数目 = 根的右值 – 根的左值 + 1。
             * 1.改变父节点
             * 先看下移动PHP到客户端，即PHP的父节点换成客户端，这种情况下树节点上的变化
             * 以PHP为根的树（虽然只有一个节点）的节点值变化是以新父节点的原右值为依据，
             * 如上图，新父节点的原右值为6，那PHP新的左值就是6，这样重新遍历PHP为根的树，
             * 就相当于这棵子树上的每个节点更新为：原右值 – (原左值 – 新左值)，
             * 
             * 所以PHP的右值就等于7 = 9 – (8 – 6)。
             * 
             * 而除了PHP为根的树，其他节点更新的有一定范围，范围就是新父节点的右值和
             * PHP原先的左值（或旧父节点的左值），因为PHP是向前移，范围内的节点值是往后移，
             * 
             * 需要加上PHP树的所占的数字数目，见上面公式，这里可以总结一个定律：
             * 新父节点_right <= ([left, right] + (移动_right – 移动_left + 1)) < 移动_left
             * 
             * 再看下PHP向后移的情况，与前移类似，也是更新一定范围的节点，变成减定律：
             * 移动_right < ([left, right] – (移动_right – 移动_left + 1)) < 新父节点_left
             * 
             */
            bool leftRightChanged = false;
            Category parentCategory = null;
            Category newCategory = null;
            int siteId = entity.SiteID;

            //更新
            dal.Update(entity.ID,entity.SiteID, entity.ModuleID, entity.Name, entity.Tag, entity.PageTitle, entity.Keywords, entity.Description, "",entity.OrderIndex);

            if (parentLft > 0)
            {
                //获取父类
                /* SELECT TOP 1 * FROM 'tree' WHERE lft<@lft AND rgt>@rgt ORDER BY lft DESC  */
                parentCategory = this.GetParent(entity.Lft, entity.Rgt);
                leftRightChanged = parentCategory.Lft != parentLft;




                //维护左右值
                if (leftRightChanged)
                {
                    int lft,
                        rgt,
                        parentRgt;

                    //要移动到的新类
                    newCategory = this.Get(a => a.Lft == parentLft);

                    //原左值
                    lft = entity.Lft;
                    rgt = entity.Rgt;
                    parentRgt = newCategory.Rgt;



                    //栏目的左值为新父节点的右值
                    // entity.Lft = newCategory.Rgt;

                    //栏目的右值为原右值-(原左值-新左值)
                    // entity.Rgt = entity.Rgt - (orgialLft - entity.Lft);

                    /*
                     * 其子节点的数目为$count = ($right_node - $left_node -1 )/2 , 节点A左值为$A_left_node ,
                     * UPDATE `tree` SET `left_node`=`left_node`-$right_node-$left_node-1 WHERE `left_node`>$right_node
                     * AND `left_node`<=$A_left_node

                     * UPDATE `tree` SET `right_node`=`right_node`-$right_node-$left_node-1 WHERE `right_node`>$right_node
                     * AND `right_node`<$A_left_node
                 
                     * UPDATE `tree` SET `left_node`=`left_node`+$A_left_node-$right_node , 
                     * `right_node`=`right_node`+$A_left_node-$right_node WHERE `left_node`>=$left_node
                     * AND `right_node`<=$right_node
                     * 
                     */
                    /*
            public string Category_ChangeUpdateTreeLeft = 
                "UPDATE $PREFIX_categories SET lft=lft-@rgt-@lft-1 WHERE lft>@rgt AND lft<=@tolft";
                     * 
            public string Category_ChangeUpdateTreeRight=
                "UPDATE $PREFIX_categories SET rgt=rgt-@rgt-@lft-1 WHERE rgt>@rgt AND rgt<@tolft";
                     * 
            public string Category_ChangeUpdateTreeBettown =
                "UPDATE $PREFIX_categories SET lft=lft+@tolft-@rgt,
                     * rgt=rgt+@tolft-@rgt WHERE lft>=@lft AND rgt<=@rgt"
                    */


                    /*
                    Array ( [lft] => 5 [rgt] => 10 [width] => 6 )
                    Array2 ( [id] => 5 [lft] => 2 [rgt] => 17 [width] => 16 )
                    Array ( [id] => 5 [lft] => 2 [rgt] => 17 [width] => 16 )

  
                    mysql_query("UPDATE entryCategory SET rgt = rgt + %d - %d, lft = lft + %d - %d
                                  WHERE rgt <= %d and lft >= %d;",$row2["rgt"],$row["lft"],
                                  $row2["rgt"],$row["lft"],$row["rgt"],$row["lft"]);
  
  
                    mysql_query("UPDATE entryCategory SET rgt = rgt + %d
                                WHERE id=%d;",$row["width"],$row2["id"]);
                     * 
                    mysql_query("UPDATE entryCategory SET rgt = rgt - %d,
                                lft = lft - %d  WHERE rgt > %d and lft > %d;",
                                $row["width"],$row["width"],$row["rgt"],$row["rgt"]);
                     * 
                    */

                    /*
                    this.dal.UpdateMoveLftRgt(parentLft, entity.Lft, entity.Rgt);
                    */


                    /*
                    Array ( [lft] => 17 [rgt] => 22 [width] => 6 )
                    Array2 ( [id] => 5 [lft] => 3 [rgt] => 18 [width] => 16 )
                    Array ( [id] => 5 [lft] => 2 [rgt] => 23 [width] => 16 )

                     */

                    /*
                    var _dal = new CategoryDAL();
                    int width = rgt - lft + 1;

                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt + " + (parentRgt - lft)
                        + ", lft = lft +" + (parentRgt - lft) + " WHERE rgt <= " + rgt + " and lft >= " + lft);

                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt + " + width
                        + " WHERE id=" + newCategory.ID);


                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt - " + width
                        + ", lft = lft -" + (width) + " WHERE rgt > " + rgt + " and lft > " + rgt);
                    */


                    if (parentRgt > rgt)
                    {
                        dal.UpdateMoveLftRgt(siteId,parentRgt, entity.Lft, entity.Rgt);
                    }
                    else
                    {
                        dal.UpdateMoveLftRgt2(siteId,parentRgt, entity.Lft, entity.Rgt);
                    }


                }
                /*
                goto planover;

            
            plan2:

                //维护左右值
                if (leftRightChanged)
                {
                     newCategory = this.Get(a => a.Lft == parentLft);

                    //原左值
                    int orgialLft = entity.Lft;

                    //栏目的左值为新父节点的右值
                    entity.Lft = newCategory.Rgt;

                    //栏目的右值为原右值-(原左值-新左值)
                    entity.Rgt = entity.Rgt - (orgialLft - entity.Lft);

                    /*
                     * 其子节点的数目为$count = ($right_node - $left_node -1 )/2 , 节点A左值为$A_left_node ,
                     * UPDATE `tree` SET `left_node`=`left_node`-$right_node-$left_node-1 WHERE `left_node`>$right_node
                     * AND `left_node`<=$A_left_node

                     * UPDATE `tree` SET `right_node`=`right_node`-$right_node-$left_node-1 WHERE `right_node`>$right_node
                     * AND `right_node`<$A_left_node
                 
                     * UPDATE `tree` SET `left_node`=`left_node`+$A_left_node-$right_node , 
                     * `right_node`=`right_node`+$A_left_node-$right_node WHERE `left_node`>=$left_node
                     * AND `right_node`<=$right_node
                     * 
                     */
                /*
                            public string Category_ChangeUpdateTreeLeft = 
                                "UPDATE $PREFIX_categories SET lft=lft-@rgt-@lft-1 WHERE lft>@rgt AND lft<=@tolft";
                                     * 
                            public string Category_ChangeUpdateTreeRight=
                                "UPDATE $PREFIX_categories SET rgt=rgt-@rgt-@lft-1 WHERE rgt>@rgt AND rgt<@tolft";
                                     * 
                            public string Category_ChangeUpdateTreeBettown =
                                "UPDATE $PREFIX_categories SET lft=lft+@tolft-@rgt,
                                     * rgt=rgt+@tolft-@rgt WHERE lft>=@lft AND rgt<=@rgt"
                */

                //this.dal.ChangeUpdateTreeLeft(parentLft, entity.Lft, entity.Rgt);
                //this.dal.ChangeUpdateTreeRight(parentLft, entity.Lft, entity.Rgt);
                //this.dal.ChangeUpdateTreeBettown(parentLft, entity.Lft, entity.Rgt);

                // }

                //更新
                //this.dal.Update(entity.ID, entity.Lft, entity.Rgt, entity.ModuleID, entity.Name, entity.Tag, entity.Keywords, entity.Description, entity.OrderIndex);


                //清除缓存
                //planover:

            }

            //重建缓存
            this.RebuildCache();

        }


        /// <summary>
        /// 删除栏目,如果有小栏目未删除则返回false
        /// </summary>
        /// <param name="lft"></param>
        /// <returns></returns>
        public bool Delete(int lft)
        {
            //用于拼接栏目ID
            StringBuilder sb = new StringBuilder();

            //获取栏目
            Category category = this.Get(a => a.Lft == lft);

            if (category == null)
            {
                return false;
            }

            int siteId = category.SiteID;

            sb.Append(category.ID.ToString());
            this.HandleCategoryTree(category.Name, (c, level) =>
            {
                sb.Append(",").Append(c.ID.ToString());
            });


            if (adal.GetCategoryArchivesCount(sb.ToString()) != 0)
            {
                return false;
            }
            else
            {

                /*
                 * 删除所有子节点？
                 * DELETE FROM `tree` WHERE `left_node`>父节点的左值 AND `right_node`>父节点的右值
                 * 9、删除一个节点及其子节点？
                 * 在上例中的<号>号后面各加一个=号
                 */
                int rgt=category.Rgt;

                bool result = dal.Delete(0,lft);
                if (result)
                {
                    //更新删除的左右值
                    dal.UpdateDeleteLftRgt(siteId,lft, rgt);

                    //删除视图设置
                    TemplateBindBLL tb = new TemplateBindBLL();
                    tb.RemoveErrorCategoryBind();
                    //tb.RemoveBind(TemplateBindType.CategoryTemplate, lft.ToString());
                    //tb.RemoveBind(TemplateBindType.CategoryArchiveTemplate, lft.ToString());

                    //重建缓存
                    this.RebuildCache();
                }
                return result;
            }
        }


        /// <summary>
        /// 获取栏目
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategories(int lft,int rgt,CategoryContainerOption option)
        {
            //如果在初始化,则直接用静态变量
            IEnumerable<Category> _categories = (cateRef == null ?
                                                    categories :
                                                    GetCategories())
                                                    .OrderBy(a => a.Lft);
            switch (option)
            {
                case CategoryContainerOption.Childs: if (rgt - lft == 1)
                    {
                        return new List<Category>();
                    }
                    else
                    {
                        return _categories.Where(a => a.Lft > lft && a.Rgt < lft);
                    }

                case CategoryContainerOption.ChildsAndSelf:
                    return _categories.Where(a => a.Lft >= lft && a.Rgt <= lft);

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
                        return new List<Category>();
                    }
                    else
                    {
                        return this.GetNextLevelCategories(_categories.Where(a => a.Lft > lft && a.Rgt < rgt));
                    }
                case CategoryContainerOption.SameLevel:
                    return this.GetSameLevelCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelNext:
                    return this.GetSameLevelNextCategories(lft, rgt, _categories);

                case CategoryContainerOption.SameLevelPrevious:
                    return this.GetSameLevelPreviousCategories(lft, rgt, _categories);

            }
            return null;
        }


        #region 
        /// <summary>
        /// 获取相同节点的接点集合
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private IEnumerable<Category> GetSameLevelCategories(int lft, int rgt, IEnumerable<Category> _categories)
        {
            /*无子节点
             *当左值+1==右值时，该节点没有子节点，则下一节点不为其子节点
             *若下一节点的左值==上一节点右值+1，则2个节点是同级关系
             */
            Category c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            if (c == null) yield return null;

            Category c1 = c;

            foreach (Category category in _categories)
            {
                if ( c1.Rgt + 1 == category.Lft || c1.Lft-1==category.Rgt || category.Lft==c.Lft )
                {
                    c1 = category;
                    yield return category;
                }
            }
        }

        /// <summary>
        /// 获取同级上面的所有节点
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="_categories"></param>
        /// <returns></returns>
        private IEnumerable<Category> GetSameLevelNextCategories(int lft, int rgt, IEnumerable<Category> _categories)
        {
            Category c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            if (c == null) yield return null;

            Category c1 = c;

            foreach (Category category in _categories)
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
        private IEnumerable<Category> GetSameLevelPreviousCategories(int lft, int rgt, IEnumerable<Category> _categories)
        {
            Category c = _categories.FirstOrDefault(a => a.Lft == lft && a.Rgt == rgt);
            if (c == null) yield return null;

            Category c1 = c;

            foreach (Category category in _categories)
            {
                if (c1.Lft - 1 == category.Rgt)
                {
                    c1 = category;
                    yield return category;
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取下级的节点
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        private IEnumerable<Category> GetNextLevelCategories(IEnumerable<Category> _categories)
        {
            /*无子节点
             *当左值+1==右值时，该节点没有子节点，则下一节点不为其子节点
             *若下一节点的左值==上一节点右值+1，则2个节点是同级关系
             */

            Category c = null;
            IList<Category> category1 = new List<Category>(_categories);

            foreach (Category c1 in _categories)
            {
                if (c == null || c1.Lft - 1 == c.Rgt)
                {
                    c = c1;
                    yield return c1;
                }
            }
        }

    }
}