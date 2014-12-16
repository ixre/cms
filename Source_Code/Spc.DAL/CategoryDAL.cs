//
// ------------------------------------
// CategoryDAL
// Copyright 2011 @ OPS
// author:newmin
// date:2010/02/20 14:05
// --------------------------------------
// 2011/02/20  newmin:修改GetJson(CategoryType type)生成的json多了','的Bug
//
namespace Ops.Cms.DAL
{
    using Ops.Data;

    public class CategoryDAL : DALBase
    {
        /// <summary>
        /// 从数据库读取栏目
        /// </summary>
        /// <returns></returns>
        public void GetAllCategories(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Category_GetAllCategories)), func);
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        public bool Update(int id,int siteId,int moduleId, string name, 
            string tag,string icon, string pagetitle, string keywords, 
            string description,string location, int orderIndex)
        {
            return base.ExecuteNonQuery(
                SqlQueryHelper.Create(SP.Category_Update,
                    new object[,]{
                {"@siteId", siteId},
                {"@moduleId", moduleId},
               {"@name", name},
                {"@tag", tag},
                {"@icon", icon},
                {"@pagetitle", pagetitle},
                {"@keywords", keywords},
                {"@description", description},
                {"@location", location},
                {"@orderindex", orderIndex},
                {"@id", id}
                    })) == 1;
        }

        public int Insert(int siteId,int left, int right, int moduleID, 
            string name, string tag, string icon,string pagetitle, 
            string keywords, string description,
            string location, int orderIndex
            )
        {

            base.ExecuteNonQuery(
                SqlQueryHelper.Create(SP.Category_Insert,
                    new object[,]{
                {"@siteid",siteId},
                {"@lft", left},
                {"@rgt", right},
                {"@moduleId", moduleID},
               {"@name", name},
                 {"@tag", tag},
                 {"@icon", icon},
                {"@pagetitle", pagetitle},
                {"@keywords", keywords},
                {"@description", description},
                {"@orderindex", orderIndex},
                {"@location", location}
                    })
                );

            return int.Parse(base.ExecuteScalar(SqlQueryHelper.Create(
                "SELECT MAX(id) FROM $PREFIX_category WHERE siteid=@siteId",
                new object[,]{
                    {"@siteid",siteId}
                })).ToString());
        }

        /// <summary>
        /// 删除栏目包含子栏目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteSelfAndChildCategoy(int siteId, int lft,int rgt)
        {
            base.ExecuteNonQuery(
                 SqlQueryHelper.Create(SP.Category_DeleteByLft,
                    new object[,]{
               {"@siteId", siteId},
               {"@lft", lft},
               {"@rgt",rgt}
                    }));
            return true;
        }



        public int GetMaxRight(int siteId)
        {
            object obj = base.ExecuteScalar(
                SqlQueryHelper.Create(SP.Category_GetMaxRight,
                new object[,] {  
                    {"@siteId", siteId} }
                ));
            if (obj == null) return 1;
            return int.Parse(obj.ToString());
        }
       
        /*
        public void UpdateMoveLftRgt(int toLeft, int left, int right)
        {

            
            base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeRight),
                   {"@lft", left},
                     {"@rgt", right},
                      {"@tolft", toLeft});

            base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeLeft),
                    {"@lft", left},
                     {"@rgt", right},
                      {"@tolft", toLeft});
            
            base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeChildNodes),
                   {"@lft", left},
                     {"@rgt", right},
                      {"@tolft", toLeft});
            
        }*/
      
       public void UpdateMoveLftRgt(int siteId,int toRgt, int left, int right)
       {

           object[,] pa = new object[,]{
                    {"@siteId", siteId},
                   {"@lft", left},
                    {"@rgt", right},
                     {"@torgt", toRgt}
                       };


           base.ExecuteNonQuery(
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeLeft),pa),
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeRight),pa),
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeChildNodes),pa)
                      );

       }

       public void UpdateMoveLftRgt2(int siteId,int toRgt, int left, int right)
       {
           object[,] pa = new object[,]{
                    {"@siteId", siteId},
                    {"@lft", left},
                    {"@rgt", right},
                     {"@torgt", toRgt}
            };

           base.ExecuteNonQuery(
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeLeft2), pa),
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeRight2), pa),
                   new SqlQuery(base.OptimizeSQL(SP.Category_ChangeUpdateTreeBettown2), pa)
                );

       }


       public void UpdateInsertLftRgt(int siteId,int left)
       {
           object[,] pa = new object[,]{
                {"@siteId",siteId},
                {"@lft", left}
            };
           base.ExecuteNonQuery(
               SqlQueryHelper.Create(SP.Category_UpdateInsertLeft, pa),
               SqlQueryHelper.Create(SP.Category_UpdateInsertRight, pa)
            );
       }


       public void UpdateDeleteLftRgt(int siteId,int lft, int rgt)
       {
           int val = rgt - lft + 1;

           object[,] pa = new object[,]{
                {"@siteId", siteId},
                {"@lft",lft},
                {"@rgt",rgt},
                {"@val",val}
            };

           base.ExecuteNonQuery(new SqlQuery(base.OptimizeSQL(SP.Category_UpdateDeleteLft), pa),
              new SqlQuery(base.OptimizeSQL(SP.Category_UpdateDeleteRgt), pa)
              );
       }
       public int GetCategoryArchivesCount(int siteId, int lft,int rgt)
       {
           return int.Parse(base.ExecuteScalar(
               new SqlQuery(base.OptimizeSQL(SP.Archive_GetCategoryArchivesCount),
                   new object[,]{
                       {"@siteId",siteId},
                       {"@lft",lft},
                       {"@rgt",rgt}
                   })).ToString());

       }
    }
}