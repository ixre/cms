
using JR.Cms.Library.DataAccess.SQL;

namespace JR.Cms.Sql
{
    public class SqlServerSqlPack : SqlPack
    {
        internal SqlServerSqlPack()
        {
        }

        public override string Archive_GetAllArchive
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cat_id],title,$PREFIX_archive.location,
                        small_title,$PREFIX_archive.flag,thumbnail,author_id,
                        author_id,view_count,[update_time],[Tags],[Outline],[Content],create_time FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cat_id] WHERE "
                    + SqlConst.Archive_NotSystemAndHidden + "  ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetArchiveList
        {
            get
            {
                return @"SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum,
                    $PREFIX_archive.id,str_id,[alias],cat_id,title,$PREFIX_archive.location,$PREFIX_archive.flag,
                        [Outline],author_id,tags,source,
                        thumbnail,[Content],update_time,[create_time],$PREFIX_category.[Name],$PREFIX_category.[Tag]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.[cat_id]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND $PREFIX_archive.cat_id IN($[catIdArray]) 
                         AND $PREFIX_arhive.site_id=@siteId) t  WHERE rowNum BETWEEN {0} AND {0}+{1}";
            }
        }
        public override string Archive_GetSelfAndChildArchiveExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,field_id as extendFieldId,f.name as fieldName,field_value as extendFieldValue,relation_id
	                    FROM $PREFIX_extend_value v INNER JOIN $PREFIX_extend_field f ON v.field_id=f.id
	                    WHERE relation_type=@relationType AND relation_id IN (
                        SELECT id FROM (SELECT $PREFIX_archive.id, ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cat_id
                        WHERE $PREFIX_archive.cat_id IN($[catIdArray]) AND $PREFIX_archive.site_id=@siteId  AND " + SqlConst.Archive_NotSystemAndHidden
                        + @") t  WHERE rowNum BETWEEN {0} AND {1}+{0})";
            }
        }

        public override string Archive_GetArchivesExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,field_id as extendFieldId,f.name as fieldName,field_value as extendFieldValue,relation_id
	                    FROM $PREFIX_extend_value v INNER JOIN $PREFIX_extend_field f ON v.field_id=f.id
	                    WHERE relation_type=@relationType AND relation_id IN (
                        SELECT id FROM (SELECT $PREFIX_archive.id, ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cat_id
                        WHERE tag=@Tag AND $PREFIX_category.site_id=@siteId AND " + SqlConst.Archive_NotSystemAndHidden
                        + @")t  WHERE rowNum BETWEEN {0} AND {1}+{0})";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT * FROM (SELECT  ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum,
                        $PREFIX_archive.id,str_id,[alias],cat_id,$PREFIX_archive.flag,title,$PREFIX_archive.location,
                        outline,thumbnail,author_id,update_time,source,tags,
                        [content],view_count,[create_time] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.cat_id WHERE site_id=@siteId AND  tag=@tag AND " +
                        SqlConst.Archive_NotSystemAndHidden + @") t WHERE rowNum BETWEEN {0} AND {1}+{0}";
            }
        }

        public override string Archive_GetArchivesByModuleId
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archive.id,str_id,[alias],$PREFIX_archive.flag,title,$PREFIX_archive.location,[Outline],
                        thumbnail,[Content],view_count,[create_time]
                        $PREFIX_category.[Name],$PREFIX_category.[Tag],view_count,[create_time],[update_time]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.ID=$PREFIX_archive.cat_id
                        AND $PREFIX_category.site_id=@siteId
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND ModuleID=@ModuleID ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archive.id,$PREFIX_category.id as cat_id,
                        $PREFIX_archive.flag,str_id,[alias],title,$PREFIX_archive.location,[Outline],[Content],thumbnail,
                        $PREFIX_category.[name],$PREFIX_category.[tag]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.[cat_id]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND $PREFIX_archive.site_id=@siteId AND $PREFIX_archive.cat_id IN($[catIdArray])
                        ORDER BY view_count DESC";
            }
        }
        

        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archive.id,[cat_id],$PREFIX_archive.flag,str_id,[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],
                    $PREFIX_category.[Name],$PREFIX_category.[Tag] FROM $PREFIX_archive
				    INNER JOIN $PREFIX_category ON $PREFIX_category.ID=$PREFIX_archive.cat_id
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND ModuleID=@ModuleID ORDER BY view_count DESC";
            }
        }


        public override string Archive_GetSpecialArchiveList
        {
            get
            {
                return @"SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum,
                            $PREFIX_archive.id,$PREFIX_archive.flag,str_id,[alias],[cat_id],title,
                        $PREFIX_archive.location,[content],[outline],thumbnail,[tags],[create_time],[update_time]
                        ,view_count,[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                    $PREFIX_category.id=$PREFIX_archive.[cat_id] WHERE $PREFIX_archive.site_id=@siteId AND $PREFIX_archive.cat_id IN($[catIdArray]) AND "
                       + SqlConst.Archive_Special + @") t WHERE rowNum BETWEEN {0} AND {0}+{1}";
            }
        }

        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archive.id,str_id,[alias],[cat_id],$PREFIX_archive.flag,
                    title,$PREFIX_archive.location,[content],[outline],thumbnail,[tags],[create_time],[update_time]
                        ,view_count,[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                    $PREFIX_category.id=$PREFIX_archive.[cat_id] WHERE $PREFIX_category.[ModuleID]=@moduleID AND site_id=@siteId AND "
                    + SqlConst.Archive_Special + @" ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get
            {
                return "SELECT TOP 1 * FROM $PREFIX_archive WHERE [cat_id]=@CategoryId AND site_id=@siteId AND "
                + SqlConst.Archive_Special + @" ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetPreviousArchive
        {
            get
            {
                return @"SELECT TOP 1 [ID],str_id,[alias],a.[cat_id],title,a.flag,a.location,thumbnail,a.[create_time],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT TOP 1 [cat_id],sort_number FROM $PREFIX_archive WHERE ID=@id) as t
                                 WHERE  (@sameCategory <>1 OR a.[cat_id]=t.[cat_id]) AND a.sort_number>t.sort_number AND 
                                 (@special = 1 OR " + SqlConst.ArchiveNotSystemAndHiddenAlias + ")" +
                                 " ORDER BY a.sort_number";
            }
        }

        public override string Archive_GetNextArchive
        {
            get
            {
                return @"SELECT TOP 1 [ID],str_id,[alias],a.[cat_id],title,a.flag,a.location,thumbnail,a.[create_time],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT TOP 1 [cat_id],sort_number FROM $PREFIX_archive WHERE [ID]=@id) as t
                                 WHERE (@sameCategory <>1 OR a.[cat_id]=t.[cat_id]) AND a.sort_number<t.sort_number AND
                                 (@special = 1 OR " + SqlConst.ArchiveNotSystemAndHiddenAlias + ")" +
                                 " ORDER BY a.sort_number DESC";
            }
        }

        public override string ArchiveGetPagedArchivesByCategoryIdPagerquery
        {
            get
            {
                /*
                return @"SELECT TOP $[pagesize] $PREFIX_archive.id AS ID,* FROM $PREFIX_archive 
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                        WHERE $[condition] AND $PREFIX_archive.id NOT IN 
                        (SELECT TOP $[skipsize] $PREFIX_archive.ID  FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                        WHERE $[condition] ORDER BY sort_number DESC) ORDER BY sort_number DESC";
                */
                return @"SELECT * FROM (SELECT $PREFIX_archive.*,
                        ROW_NUMBER()OVER(ORDER BY $PREFIX_archive.sort_number DESC) as rowNum
                        FROM $PREFIX_archive 
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                        WHERE $PREFIX_category.site_id=@siteId AND $PREFIX_archive.cat_id IN ($[catIdArray]) 
                         AND " + SqlConst.Archive_NotSystemAndHidden + @") _t 
						WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string ArchiveGetpagedArchivesCountSql
        {
            get
            {
                return @"SELECT COUNT(a.id) FROM $PREFIX_archive a
                        Left JOIN $PREFIX_category c ON
                        a.cat_id=c.id Where {0}";
            }
        }


        public override string Archive_GetPagedArchivesByCategoryId
        {
            get
            {
                /*return @"SELECT TOP $[pagesize] a.[ID] AS [ID],str_id,[alias],title,$PREFIX_archive.location,thumbnail,
                        c.[Name] as [CategoryName],[cat_id],author_id,[Content],[Source],
                        create_time,view_count FROM $PREFIX_archive a LEFT JOIN $PREFIX_category c
                        ON a.cat_id=c.ID INNER JOIN $PREFIX_modules m ON c.[module_id]=m.[id]
                        WHERE $[condition] AND a.[ID] NOT IN 
                        (SELECT TOP $[skipsize] a1.[ID] FROM $PREFIX_archive a1
                         LEFT JOIN $PREFIX_category c1 ON a1.cat_id=c1.ID
                        INNER JOIN $PREFIX_modules ON c1.[module_id]=m1.[id]
                        WHERE $[condition] ORDER BY [$[orderByField]] $[orderASC]) ORDER BY [$[orderByField]] $[orderASC]";*/

                return @"SELECT * FROM (SELECT a.id AS id,str_id,alias,title,
                        a.location,thumbnail,c.name as categoryName,[cat_id],a.flag,author_id,[content],
                        [source],create_time,view_count,
						ROW_NUMBER()OVER(ORDER BY $[orderByField] $[orderASC]) as rowNum
						FROM $PREFIX_archive a LEFT JOIN $PREFIX_category c
                        ON a.cat_id=c.ID WHERE $[condition]) _t
						WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }


        public override string Archive_GetPagedOperations
        {
            get
            {
                //return "SELECT TOP $[pagesize] * FROM $PREFIX_operation WHERE ID NOT IN (SELECT TOP $[skipsize] ID FROM $PREFIX_operation)"; 
                return @"SELECT * FROM (SELECT *,
        			ROW_NUMBER()OVER(ORDER BY id) as rowNum
			 		FROM $PREFIX_operation) _t WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string Message_GetPagedMessages
        {
            get
            {
                return @"SELECT * FROM (SELECT *,
        			ROW_NUMBER()OVER(ORDER BY [SendDate] DESC) as rowNum FROM $PREFIX_Message
				    WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC) _t
					WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])";
            }
        }
        public override string Member_GetPagedMembers
        {
            get
            {
                //return @"SELECT TOP $[pagesize] [id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime] FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                //        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID] WHERE [ID] NOT IN (SELECT TOP $[skipsize] [ID] FROM $PREFIX_member ORDER BY [ID] DESC) ORDER BY [ID] DESC";
                return @"SELECT * FROM (SELECT $PREFIX_member.[id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime],
						 ROW_NUMBER()OVER(ORDER BY $PREFIX_member.id) as rowNum
 						 FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID] ORDER BY $PREFIX_member.[ID] DESC) _t
						WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                // $PREFIX_category.name as cname,
                //        $PREFIX_category.tag,

                return @"SELECT *  FROM (SELECT $PREFIX_archive.*,
                     ROW_NUMBER() OVER($[orderby]) as rowNum
					 FROM $PREFIX_archive INNER JOIN $PREFIX_category
                    ON $PREFIX_archive.cat_id=$PREFIX_category.id
                    WHERE $[condition]) _t WHERE rowNum BETWEEN $[skipsize]+1 AND
                    $[skipsize]+$[pagesize] ORDER BY rowNum";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByModuleId
        {
            get
            {
                /*
                return @"SELECT TOP $[pagesize] $PREFIX_archive.id AS ID,* FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                    WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[module_id] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') AND
                    $PREFIX_archive.id NOT IN (SELECT TOP $[skipsize] $PREFIX_archive.id FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                   WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[module_id] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby]) $[orderby]";
                */
                return @"SELECT * FROM (SELECT $PREFIX_archive.*,
                     ROW_NUMBER()OVER($[orderby]) as rowNum
					 FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                    WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[module_id] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby]) _t
					WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByCategoryId
        {
            get
            {
                return @"SELECT * FROM (SELECT $PREFIX_archive.*,
                         ROW_NUMBER()OVER($[orderby]) as rowNum
					     FROM  $PREFIX_archive INNER JOIN $PREFIX_category 
                         ON $PREFIX_archive.cat_id=$PREFIX_category.id
                        WHERE $[condition] AND $PREFIX_archive.path LIKE @catPath AND $PREFIX_archive.site_id=@siteId
                        AND (title LIKE '%$[keyword]%' OR Outline LIKE '%$[keyword]%'
                        OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
					) _t WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize]) ORDER BY rowNum";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get { return "SELECT * FROM (SELECT *,ROW_NUMBER()OVER(ORDER BY id) as rowNum FROM $PREFIX_operation WHERE $[condition]) _t  WHERE rowNum BETWEEN $skipsize AND ($skipsize+$pagesize)"; }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cat_id],title,$PREFIX_archive.location,[Tags],[Outline],thumbnail,[Content],[IsSystem],[IsSpecial],[Visible],create_time FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                    $PREFIX_category.id=$PREFIX_archive.[cat_id] WHERE {0} ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get
            {
                return "SELECT * FROM $PREFIX_comment LEFT JOIN $PREFIX_member ON [MemberID]=$PREFIX_member.[ID]" +
                       " WHERE [archiveID]=@archiveId";
            }
        }


        public override string Link_AddSiteLink
        {

            get
            {
                return @"INSERT INTO $PREFIX_link (site_id,pid,[type],[text],[uri],
                    img_url,[target],bind, visible,sort_number)VALUES(@siteId,@pid,@TypeID,
                    @Text,@Uri,@imgurl,@Target,@bind,@visible,@sortNumber)";
            }
        }

        public override string Link_UpdateSiteLink
        {
            get
            {
                return @"UPDATE $PREFIX_link SET pid=@pid,[type]=@TypeID,[text]=@Text,
                            visible=@visible,[uri]=@Uri,img_url=@imgurl,[target]=@Target,
                            bind=@bind,sort_number=@sortNumber WHERE [ID]=@LinkId AND site_id=@siteId";
            }
        }

        public override string ArchiveAdd
        {
            get
            {
                return @"INSERT INTO $PREFIX_archive(site_id,str_id,alias,cat_id,author_id,path,flag,
                                    title,small_title,location,sort_number,
                                    [source],thumbnail,[Outline],[Content],[Tags],[Agree],[Disagree],view_count,
                                    create_time,update_time)
                                    VALUES(@siteId,@strId,@alias,@catId,@authorId,@path,@flag,@title,
                                    @smallTitle,@location,@sortNumber,
                                    @source,@thumbnail,@outline, @content,@tags,0,0,1,@createTime,
                                    @updateTime)";
            }
        }

        public override string Comment_GetCommentDetailsListForArchive
        {
            get
            {
                return @"SELECT $PREFIX_comment.ID as cat_id,[IP],[content],[create_time],
                       $PREFIX_member.ID as uid,[Avatar],[NickName] FROM $PREFIX_comment INNER JOIN $PREFIX_member ON
                       $PREFIX_comment.[memberID]=$PREFIX_member.[ID] WHERE [archiveID]=@archiveID ORDER BY [create_time] DESC";
            }
        }

        public override string ArchiveUpdate
        {
            get
            {
                return @"UPDATE $PREFIX_archive SET [cat_id]=@catId,path=@path,flag=@flag,[Title]=@Title,small_title=@smallTitle,sort_number=@sortNumber,
                                    [Alias]=@Alias,location=@location,[Source]=@Source,update_time=@updateTime,
                                    thumbnail=@thumbnail,[Outline]=@Outline,[Content]=@Content,[Tags]=@Tags WHERE id=@id";
            }
        }

        public override string ArchiveGetSearchRecordCountByModuleId
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cat_id]=$PREFIX_category.id
                        WHERE {2} AND $PREFIX_category.module_id={0} AND ([Title] LIKE '%{1}%'
                        OR [Outline] LIKE '%{1}%' OR [Content] LIKE '%{1}%' OR [Tags] LIKE '%{1}%')";
            }
        }

        public override string ArchiveGetSearchRecordCountByCategoryId
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive
                         WHERE {1} AND $PREFIX_archive.path LIKE @catPath AND $PREFIX_archive.site_id=@siteId
                         AND ([Title] LIKE '%{0}%' OR [Outline] LIKE '%{0}%' OR [Content] LIKE '%{0}%' OR [Tags] LIKE '%{0}%')";
            }
        }

        public override string Comment_AddComment
        {
            get { return "INSERT INTO $PREFIX_comment ([ArchiveID],[MemberID],[IP],[Content],[Recycle],create_time)VALUES(@ArchiveId,@MemberID,@IP,@Content,@Recycle,@create_time)"; }
        }

        public override string Member_RegisterMember
        {
            get { return "INSERT INTO $PREFIX_member([Username],[Password],[Avatar],[Sex],[Nickname],[Note],[Email],[Telephone])values(@username,@password,@Avatar,@sex,@nickname,@note,@email,@Telephone)"; }
        }

        public override string Member_Update
        {
            get { return "UPDATE $PREFIX_member SET [Password]=@Password,[Avatar]=@Avatar,[Sex]=@Sex,[Nickname]=@Nickname,[Email]=@Email,[Telephone]=@Telephone,[Note]=@Note WHERE [ID]=@id"; }
        }

        public override string TableGetLastedRowId
        {
            get { return "SELECT TOP 1 id FROM $PREFIX_table_row ORDER BY id DESC"; }
        }
        public override string TableInsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_record (row_id,col_id,[value]) VALUES(@rowId,@columnId,@value)"; }
        }

        public override string TableGetPagedRows
        {
            get
            {
                return @"SELECT * FROM (SELECT *,
                        ROW_NUMBER()OVER(ORDER BY submit_time DESC) as rowNum
						FROM $PREFIX_table_row WHERE table_id=$[tableid]
                        ORDER BY submit_time DESC) _t
						WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])";
            }
        }
    }
}
