using System.Data;
using AtNet.Cms.DAL;

namespace AtNet.Cms.ServiceRepository.Query
{
    public class ArchiveQuery
    {
        private ArchiveDal dal = new ArchiveDal();

        public DataTable GetPagedArchives(int siteId, int lft,int rgt , 
            int publishId, string[,] flags, string orderByField, bool orderAsc, 
            int pageSize, int currentPageIndex, 
            out int recordCount, out int pages)
        {
            return dal.GetPagedArchives(siteId,-1,
            lft, rgt, publishId,
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
