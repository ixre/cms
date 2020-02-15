using System.Data;
using JR.Cms.Dal;

namespace JR.Cms.ServiceRepository.Query
{
    public class ArchiveQuery
    {
        private readonly ArchiveDal _dal = new ArchiveDal();

        public DataTable GetPagedArchives(int siteId,int[] catIdArray, int publisherId, bool includeChild,
            int flag, string keyword, string orderByField, bool orderAsc, int pageSize, int currentPageIndex,
            out int recordCount, out int pages)
        {
            return _dal.GetPagedArchives(siteId, -1,
                catIdArray, publisherId, includeChild,
                flag, keyword,orderByField,
                orderAsc, pageSize,
                currentPageIndex,
                out recordCount,
                out pages);
        }

        public DataTable GetPagedArchives(
            int siteId,
            int[] catIdArray, int pageSize,
            int skipSize,
            ref int pageIndex,
            out int records,
            out int pages)
        {
            return _dal.GetPagedArchives(siteId,catIdArray,
                    pageSize, skipSize,ref pageIndex, out records, out pages);
        }
    }
}
