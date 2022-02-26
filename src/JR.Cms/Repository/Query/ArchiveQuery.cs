using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.DataAccess.DAL;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Repository.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public ArchiveQuery(IDbProvider provider)
        {
            this._provider = provider;
        }

        private readonly ArchiveDal _dal = new ArchiveDal();
        private readonly IDbProvider _provider;

        public DataTable GetPagedArchives(int siteId, int[] catIdArray, int publisherId, bool includeChild,
            int flag, string keyword, string orderByField, bool orderAsc, int pageSize, int currentPageIndex,
            out int recordCount, out int pages)
        {
            return _dal.GetPagedArchives(siteId, -1,
                catIdArray, publisherId, includeChild,
                flag, keyword, orderByField,
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
            return _dal.GetPagedArchives(siteId, catIdArray,
                pageSize, skipSize, ref pageIndex, out records, out pages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="unix"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IEnumerable<CmsArchiveEntity> GetArchiveByTimeAgo(int siteId,long unix, int size)
        {
            using (IDbConnection db = _provider.GetConnection())
            {
                return db.Query<CmsArchiveEntity>(_provider.FormatQuery($@"SELECT 
                  id as Id,
                  str_id as StrId,
                  site_id as SiteId,
                  alias as Alias,
                  cat_id as CatId,
                  path as Path,
                  flag as Flag,
                  author_id as AuthorId,
                  title as Title,
                  small_title as SmallTitle,
                  location as Location,
                  sort_number as SortNumber,
                  source as Source,
                  tags as Tags,
                  outline as Outline,
                  content as Content,
                  view_count as ViewCount,
                  agree as Agree,
                  disagree as Disagree,
                  thumbnail as Thumbnail,
                  create_time as CreateTime,
                  update_time as UpdateTime
                  FROM $PREFIX_archive
                  WHERE site_id=@SiteId
                    AND update_time > ${unix}"
                ), new CmsArchiveEntity
                {
                    SiteId = siteId
                }).Take(size);
            }
        }
    }
}