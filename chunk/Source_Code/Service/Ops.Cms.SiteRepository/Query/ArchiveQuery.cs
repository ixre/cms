using Ops.Cms.DAL;
using System.Data;

namespace Ops.Cms.ServiceRepository.Query
{
    public class ArchiveQuery
    {
        private ArchiveDAL dal = new ArchiveDAL();

        public DataTable GetPagedArchives(int siteId, int lft,int rgt , 
            string author, string[,] flags, string orderByField, bool orderAsc, 
            int pageSize, int currentPageIndex, 
            out int recordCount, out int pages)
        {
            return dal.GetPagedArchives(siteId,-1,
            lft, rgt, author,
            flags, orderByField,
            orderAsc,pageSize,
            currentPageIndex,
            out recordCount,
            out pages);

        }

        public DataTable GetPagedArchives(
            int siteId,
            int categoryLft,
            int categoryRgt, int pageSize,
            ref int pageIndex,
            out int records,
            out int pages)
        {
            return dal.GetPagedArchives(siteId, categoryLft, categoryRgt,
                    pageSize, ref pageIndex, out records, out pages);
        }
    }
}
