using System.Data;
using J6.Cms.Dal;

namespace J6.Cms.ServiceRepository.Query
{
    public class ArchiveQuery
    {
        private readonly ArchiveDal _dal = new ArchiveDal();

        public DataTable GetPagedArchives(int siteId, int lft, int rgt, int publisherId, bool includeChild,
            string[,] flags, string orderByField, bool orderAsc, int pageSize, int currentPageIndex,
            out int recordCount, out int pages)
        {
            return _dal.GetPagedArchives(siteId, -1,
                lft, rgt, publisherId, includeChild,
                flags, orderByField,
                orderAsc, pageSize,
                currentPageIndex,
                out recordCount,
                out pages);
        }

        public DataTable GetPagedArchives(
            int siteId,
            int categoryLft,
            int categoryRgt, int pageSize,
            int skipSize,
            ref int pageIndex,
            out int records,
            out int pages)
        {
            return _dal.GetPagedArchives(siteId, categoryLft, categoryRgt,
                    pageSize, skipSize,ref pageIndex, out records, out pages);
        }
    }
}
