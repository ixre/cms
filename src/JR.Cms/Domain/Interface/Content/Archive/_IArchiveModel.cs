namespace JR.Cms.Domain.Interface.Content.Archive
{
    public interface IArchiveModel
    {
        void AddViewCount(string archiveID, int count);

        bool AliasIsExist(int siteID, string alias);

        // string Create(Spc.Models.Archive archive);
        //bool Delete(string archiveID);
        void DeleteMemberArchive(int id);

        // Spc.Models.Archive Get(int siteID, string idOrAlias);
        //Spc.Models.Archive Get(string idOrAlias);
        System.Data.DataTable GetAllArchives();

        //Spc.Models.Archive GetArchiveByID(string id);
        System.Data.DataTable GetArchives(string sqlcondition);

        string GetCommentDetailsJSON(string archiveId);
        //Spc.Models.Archive GetFirstSpecialArchive(int categoryId);
        //Spc.Models.Archive GetFirstSpecialArchive(string categoryTag);
        //System.Data.DataTable GetPagedArchives(int categoryID, int pageSize, ref int currentPageIndex, out int recordCount, out int pages);
        //System.Data.DataTable GetPagedArchives(int? siteid, int? moduleID, int? categoryID, string author, string[,] flags, string orderByField, bool orderASC, int pageSize, int currentPageIndex, out int recordCount, out int pages);


        //void RePublish(string archiveID);
        //System.Data.DataTable Search(int siteId, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);
        //System.Data.DataTable SearchByCategory(int siteId, int categoryLft,int categoryRgt, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);
        //System.Data.DataTable SearchByModule(string keyword, int moduleID, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);
        int TransferAuthor(string username, string anotherUsername);
        //void Update(Spc.Models.Archive archive);

        /// <summary>
        /// 获取文档自动生成的ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetArchiveAutoID(string id);
    }
}