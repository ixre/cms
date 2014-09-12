
namespace Spc.Sql
{
    public class SqlServerSqlPack:SqlPack
    {
        internal SqlServerSqlPack()
        {
        }

        public override string Archive_GetAllArchive
        {
            get
            {
                return @"SELECT $PREFIX_archives.[ID],[strid],[alias],[cid],[Title],[flags],thumbnail,author,
                        [author],[viewcount],[lastmodifydate],[Tags],[Outline],[Content],[CreateDate] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                        $PREFIX_categories.[ID]=$PREFIX_archives.[cid] WHERE " +SqlConst.Archive_NotSystemAndHidden + "  ORDER BY [CreateDate] DESC";
            }
        }

        public override string Archive_GetSelfAndChildArchives
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[strid],[alias],cid,[Title],[flags],[Outline],author,tags,source,
                        thumbnail,[Content],lastmodifydate,[CreateDate],$PREFIX_categories.[Name],$PREFIX_categories.[Tag]
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_categories.[ID]=$PREFIX_archives.[cid]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND (lft>=@lft AND rgt<=@rgt) 
                         AND $PREFIX_categories.siteId=@siteId 
                        ORDER BY [CreateDate] DESC";
            }
        }
        public override string Archive_GetSelfAndChildArchiveExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,fieldId as extendFieldId,f.name as fieldName,fieldValue as extendFieldValue,relationId
	                    FROM $PREFIX_extendValue v INNER JOIN $PREFIX_extendField f ON v.fieldId=f.id
	                    WHERE relationType=@relationType AND relationId IN (

                        SELECT TOP {0} $PREFIX_archives.id
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON 
                        $PREFIX_categories.id=$PREFIX_archives.cid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden
                                + @" AND (lft>=@lft AND rgt<=@rgt) 
                        ORDER BY createdate DESC
                        
                        )";
            }
        }

        public override string Archive_GetArchivesExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,fieldId as extendFieldId,f.name as fieldName,fieldValue as extendFieldValue,relationId
	                    FROM $PREFIX_extendValue v INNER JOIN $PREFIX_extendField f ON v.fieldId=f.id
	                    WHERE relationType=@relationType AND relationId IN (

                        SELECT TOP {0} $PREFIX_archives.id FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.id=$PREFIX_archives.cid
                        WHERE tag=@Tag AND " + SqlConst.Archive_NotSystemAndHidden
                        + @" ORDER BY createdate DESC,$PREFIX_archives.id
                        
                        )";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[strid],[alias],cid,[flags],[Title],[Outline],thumbnail,author,lastmodifydate,source,tags,
                        [Content],[ViewCount],[CreateDate] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                        $PREFIX_categories.[ID]=$PREFIX_archives.cid WHERE [Tag]=@Tag AND " +
                        SqlConst.Archive_NotSystemAndHidden + @" ORDER BY [CreateDate] DESC";
            }
        }

        public override string Archive_GetArchivesByModuleID
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[strid],[alias],[flags],[Title],[Outline],
                        thumbnail,[Content],[ViewCount],[CreateDate]
                        $PREFIX_categories.[Name],$PREFIX_categories.[Tag],[viewcount],[createdate],[lastmodifydate]
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_categories.ID=$PREFIX_archives.cid
                        AND $PREFIX_categories.siteid=@siteid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND ModuleID=@ModuleID ORDER BY [CreateDate] DESC";
            }
        }
       
        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],$PREFIX_categories.[ID] as cid,
                        flags,[strid],[alias],[Title],[Outline],[Content],thumbnail,
                        $PREFIX_categories.[Name],$PREFIX_categories.[Tag]
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_categories.[ID]=$PREFIX_archives.[cid]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND  (lft>=@lft AND rgt<=@rgt)
                        ORDER BY [ViewCount] DESC";
            }
        }

         public override string Archive_GetArchivesByViewCountDesc_Tag
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],$PREFIX_categories.[ID] as cid,flags,
                        [strid],[alias],[Title],[Outline],thumbnail,[Content]
                        ,$PREFIX_categories.[Name],$PREFIX_categories.[Tag]
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_categories.[ID]=$PREFIX_archives.[cid]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND tag=@tag
                        ORDER BY [ViewCount] DESC";
            }
        }

        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[cid],flags,[strid],[alias],[Title],[Outline],thumbnail,[Content],
                    $PREFIX_categories.[Name],$PREFIX_categories.[Tag] FROM $PREFIX_archives
				    INNER JOIN $PREFIX_categories ON $PREFIX_categories.ID=$PREFIX_archives.cid
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND ModuleID=@ModuleID ORDER BY [ViewCount] DESC";
            }
        }


        public override string Archive_GetSpecialArchivesByCategoryID
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],flags,[strid],[alias],[cid],[flags],[title],[content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                    $PREFIX_categories.[ID]=$PREFIX_archives.[cid] WHERE (lft>=@lft AND rgt<=@rgt) AND "
                    + SqlConst.Archive_Special + @" ORDER BY [CreateDate] DESC";
            }
        } 
        public override string Archive_GetSpecialArchivesByCategoryTag
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[strid],[alias],[cid],[flags],[title],[content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                    $PREFIX_categories.[ID]=$PREFIX_archives.[cid] WHERE $PREFIX_categories.[tag]=@CategoryTag AND "
                    + SqlConst.Archive_Special + @" ORDER BY [CreateDate] DESC";
            }
        }
        
        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT TOP {0} $PREFIX_archives.[ID],[strid],[alias],[cid],[flags],[title],[content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                    $PREFIX_categories.[ID]=$PREFIX_archives.[cid] WHERE $PREFIX_categories.[ModuleID]=@moduleID AND " 
                    + SqlConst.Archive_Special + @" ORDER BY [CreateDate] DESC";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get { return "SELECT TOP 1 * FROM $PREFIX_archives WHERE [cid]=@CategoryId AND " 
                + SqlConst.Archive_Special + @" ORDER BY [CreateDate] DESC"; }
        }

        public override string Archive_GetPreviousSameCategoryArchive
        {
            get
            {
                return @"SELECT TOP 1 [ID],[strid],[alias],a.[cid],[Title],thumbnail,a.[createdate] FROM $PREFIX_archives a,
                                 (SELECT TOP 1 [cid],[CreateDate] FROM $PREFIX_archives WHERE ID=@id) as t
                                 WHERE a.[cid]=t.[cid] AND a.[CreateDate]<t.[CreateDate] AND " + SqlConst.Archive_NotSystemAndHidden +
                                 " ORDER BY a.[CreateDate] DESC ";
            }
        }

        public override string Archive_GetNextSameCategoryArchive
        {
            get
            {
                return @"SELECT TOP 1 [ID],[strid],[alias],a.[cid],[Title],thumbnail,a.[createdate] FROM $PREFIX_archives a,
                                 (SELECT TOP 1 [cid],[CreateDate] FROM $PREFIX_archives WHERE [ID]=@id) as t
                                 WHERE a.[cid]=t.[cid] AND a.[CreateDate]>t.[CreateDate] AND " + SqlConst.Archive_NotSystemAndHidden +
                                 " ORDER BY a.[CreateDate]";
            }
        }

        public override string Archive_GetPagedArchivesByCategoryID_pagerquery
        {
            get
            {
            	/*
                return @"SELECT TOP $[pagesize] $PREFIX_archives.[ID] AS ID,* FROM $PREFIX_archives 
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                        WHERE $[condition] AND $PREFIX_archives.[id] NOT IN 
                        (SELECT TOP $[skipsize] $PREFIX_archives.ID  FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                        WHERE $[condition] ORDER BY [CreateDate] DESC) ORDER BY [CreateDate] DESC";
                */
                return @"SELECT * FROM (SELECT $PREFIX_archives.*,
                        ROW_NUMBER()OVER(ORDER BY [CreateDate] DESC) as rowNum
                        FROM $PREFIX_archives 
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                        WHERE $PREFIX_categories.siteId=@siteId AND (lft>=@lft AND rgt<=@rgt) 
                         AND " + SqlConst.Archive_NotSystemAndHidden + @") _t 
						WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string Archive_GetpagedArchivesCountSql
        {
            get
            {
                return @"SELECT COUNT(a.id) FROM $PREFIX_archives a
                        Left JOIN $PREFIX_categories c ON
                        a.cid=c.id Where {0}";
            }
        }


        public override string Archive_GetPagedArchivesByCategoryID
        {
            get
            {
                /*return @"SELECT TOP $[pagesize] a.[ID] AS [ID],[strid],[alias],[Title],thumbnail,
                        c.[Name] as [CategoryName],[cid],[flags],[Author],[Content],[Source],
                        [CreateDate],[ViewCount] FROM $PREFIX_archives a LEFT JOIN $PREFIX_categories c
                        ON a.cid=c.ID INNER JOIN $PREFIX_modules m ON c.[moduleid]=m.[id]
                        WHERE $[condition] AND a.[ID] NOT IN 
                        (SELECT TOP $[skipsize] a1.[ID] FROM $PREFIX_archives a1
                         LEFT JOIN $PREFIX_categories c1 ON a1.cid=c1.ID
                        INNER JOIN $PREFIX_modules ON c1.[moduleid]=m1.[id]
                        WHERE $[condition] ORDER BY [$[orderByField]] $[orderASC]) ORDER BY [$[orderByField]] $[orderASC]";*/
             
                return @"SELECT * FROM (SELECT a.[ID] AS id,[strid],[alias],[title],thumbnail,
                        c.[name] as [categoryName],[cid],[flags],[author],[content],[source],[createDate],[viewCount],
						ROW_NUMBER()OVER(ORDER BY [$[orderByField]] $[orderASC]) as rowNum
						FROM $PREFIX_archives a LEFT JOIN $PREFIX_categories c
                        ON a.cid=c.ID WHERE $[condition]) _t
						WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }


        public override string Archive_GetPagedOperations
        {
            get { 
        		//return "SELECT TOP $[pagesize] * FROM $PREFIX_operations WHERE ID NOT IN (SELECT TOP $[skipsize] ID FROM $PREFIX_operations)"; 
        		return @"SELECT * FROM (SELECT *,
        			ROW_NUMBER()OVER(ORDER BY id) as rowNum
			 		FROM $PREFIX_operations) _t WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
        	}
        }

        public override string Message_GetPagedMessages
        {
            get { return @"SELECT * FROM (SELECT *,
        			ROW_NUMBER()OVER(ORDER BY [SendDate] DESC) as rowNum FROM $PREFIX_Message
				    WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC) _t
					WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])"; }
        }
        public override string Member_GetPagedMembers
        {
            get
            {
                //return @"SELECT TOP $[pagesize] [id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime] FROM $PREFIX_members INNER JOIN $PREFIX_memberdetails
                //        ON $PREFIX_members.[ID]=$PREFIX_memberdetails.[UID] WHERE [ID] NOT IN (SELECT TOP $[skipsize] [ID] FROM $PREFIX_members ORDER BY [ID] DESC) ORDER BY [ID] DESC";
                return @"SELECT * FROM (SELECT $PREFIX_members.[id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime],
						 ROW_NUMBER()OVER(ORDER BY $PREFIX_members.id) as rowNum
 						 FROM $PREFIX_members INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_members.[ID]=$PREFIX_memberdetails.[UID] ORDER BY $PREFIX_members.[ID] DESC) _t
						WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                /*return @"SELECT TOP $[pagesize] $PREFIX_archives.[ID] AS ID,* FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                    WHERE $[condition] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '$[keyword]%') AND
                    $PREFIX_archives.[ID] NOT IN (SELECT TOP $[skipsize] $PREFIX_archives.[ID] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                    WHERE $[condition] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby]) $[orderby]";
				*/
 				return @"SELECT * FROM (SELECT $PREFIX_archives.[ID] AS ID,*,
                     ROW_NUMBER() OVER($[orderby]) as rowNum
					 FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                    WHERE $[condition] AND $PREFIX_categories.siteid=$[siteid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%'
				   OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '$[keyword]%')
                    $[orderby]) _t WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
			}
        }

        public override string Archive_GetPagedSearchArchivesByModuleID
        {
            get
            {
            	/*
                return @"SELECT TOP $[pagesize] $PREFIX_archives.[ID] AS ID,* FROM  $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                    WHERE $[condition] AND $PREFIX_categories.[ModuleID]=$[moduleid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') AND
                    $PREFIX_archives.[ID] NOT IN (SELECT TOP $[skipsize] $PREFIX_archives.[ID] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                   WHERE $[condition] AND $PREFIX_categories.[ModuleID]=$[moduleid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby]) $[orderby]";
            	*/
            	 return @"SELECT * FROM (SELECT $PREFIX_archives.[ID] AS ID,*,
                     ROW_NUMBER()OVER($[orderby]) as rowNum
					 FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                    WHERE $[condition] AND $PREFIX_categories.[ModuleID]=$[moduleid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby]) _t
					WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
			}
        }

        public override string Archive_GetPagedSearchArchivesByCategoryID
        {
            get
            {
                return @"SELECT * FROM (SELECT $PREFIX_archives.[ID] AS ID,*,
                         ROW_NUMBER()OVER($[orderby]) as rowNum
					     FROM  $PREFIX_archives INNER JOIN $PREFIX_categories 
                         ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                        WHERE $[condition] AND $PREFIX_categories.siteid=@siteId AND ($PREFIX_categories.lft>=@lft AND 
                        $PREFIX_categories.rgt<=@rgt) AND ([Title] LIKE '%$[keyword]%'
                        OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%'
                        OR [Tags] LIKE '%$[keyword]%') $[orderby]
					) _t WHERE rowNum BETWEEN $[skipsize]+1 AND ($[skipsize]+$[pagesize])";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get { return "SELECT * FROM (SELECT *,ROW_NUMBER()OVER(ORDER BY id) as rowNum FROM $PREFIX_operations WHERE $[condition]) _t  WHERE rowNum BETWEEN $skipsize AND ($skipsize+$pagesize)"; }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archives.[ID],[strid],[alias],[cid],[Title],[Tags],[Outline],thumbnail,[Content],[IsSystem],[IsSpecial],[Visible],[CreateDate] FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON
                    $PREFIX_categories.[ID]=$PREFIX_archives.[cid] WHERE {0} ORDER BY [CreateDate] DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get { return "SELECT * FROM $PREFIX_comments LEFT JOIN $PREFIX_members ON [MemberID]=$PREFIX_members.[ID] WHERE [archiveID]=@archiveId"; }
        }


        public override string Link_AddSiteLink
        {
            get { return @"INSERT INTO $PREFIX_links (siteid,pid,[type],[text],[uri],
                    imgurl,[target],bind, visible,orderIndex)VALUES(@siteid,@pid,@TypeID,
                    @Text,@Uri,@imgurl,@Target,@bind,visible=@visible,@orderIndex)";
            }
        }

        public override string Link_UpdateSiteLink
        {
            get { return @"UPDATE $PREFIX_links SET pid=@pid,[type]=@TypeID,[text]=@Text,
                            visible=@visible,[uri]=@Uri,imgurl=@imgurl,[target]=@Target,
                            bind=@bind,orderIndex=@orderIndex WHERE [ID]=@LinkId AND siteid=@siteId";
            }
        }

        public override string Archive_Add
        {
            get
            {
                return @"INSERT INTO $PREFIX_archives(strId,[alias],[cid],[Author],[Title],[flags],
                                    [Source],thumbnail,[Outline],[Content],[Tags],[Agree],[Disagree],[ViewCount],
                                    [CreateDate],[LastModifyDate])
                                    VALUES(@strId,@alias,@CategoryId,@Author,@Title,@Flags,@Source,@thumbnail
                                    ,@Outline,@Content,@Tags,0,0,0,@CreateDate,@LastModifyDate)";
            }
        }

        public override string Comment_GetCommentDetailsListForArchive
        {
            get
            {
                return @"SELECT $PREFIX_comments.ID as cid,[IP],[content],[createDate],
                       $PREFIX_members.ID as uid,[Avatar],[NickName] FROM $PREFIX_comments INNER JOIN $PREFIX_members ON
                       $PREFIX_comments.[memberID]=$PREFIX_members.[ID] WHERE [archiveID]=@archiveID ORDER BY [createDate] DESC";
            }
        }

        public override string Archive_Update
        {
            get
            {
                return @"UPDATE $PREFIX_archives SET [cid]=@CategoryId,[Title]=@Title,flags=@flags,
                                    [Alias]=@Alias,[Source]=@Source,lastmodifydate=@lastmodifyDate,
                                    thumbnail=@thumbnail,[Outline]=@Outline,[Content]=@Content,[Tags]=@Tags WHERE id=@id";
            }
        }

        public override string Archive_GetSearchRecordCountByModuleID
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                        WHERE {2} AND $PREFIX_categories.moduleid={0} AND ([Title] LIKE '%{1}%'
                        OR [Outline] LIKE '%{1}%' OR [Content] LIKE '%{1}%' OR [Tags] LIKE '%{1}%')";
            }
        }

        public override string Archive_GetSearchRecordCountByCategoryID
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archives.id) FROM $PREFIX_archives
                         INNER JOIN $PREFIX_categories ON $PREFIX_archives.[cid]=$PREFIX_categories.[ID]
                         WHERE {1} AND $PREFIX_categories.siteid=@siteId 
                            AND ($PREFIX_categories.lft>=@lft AND $PREFIX_categories.rgt<=@rgt)
                         AND ([Title] LIKE '%{0}%' OR [Outline] LIKE '%{0}%' OR [Content] LIKE '%{0}%' OR [Tags] LIKE '%{0}%')";
            }
        }

        public override string Comment_AddComment
        {
            get { return "INSERT INTO $PREFIX_comments ([ArchiveID],[MemberID],[IP],[Content],[Recycle],[CreateDate])VALUES(@ArchiveId,@MemberID,@IP,@Content,@Recycle,@CreateDate)"; }
        }

        public override string Member_RegisterMember
        {
            get { return "INSERT INTO $PREFIX_members([Username],[Password],[Avatar],[Sex],[Nickname],[Note],[Email],[Telephone])values(@username,@password,@Avatar,@sex,@nickname,@note,@email,@Telephone)"; }
        }

        public override string Member_Update
        {
            get { return "UPDATE $PREFIX_members SET [Password]=@Password,[Avatar]=@Avatar,[Sex]=@Sex,[Nickname]=@Nickname,[Email]=@Email,[Telephone]=@Telephone,[Note]=@Note WHERE [ID]=@id"; }
        }

        public override string Table_GetLastedRowID
        {
            get { return "SELECT TOP 1 id FROM $PREFIX_table_rows ORDER BY id DESC"; }
        }
        public override string Table_InsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_rowsdata (rid,cid,[value]) VALUES(@rowid,@columnid,@value)"; }
        }

        public override string Table_GetPagedRows
        {
            get
            {
                return @"SELECT * FROM (SELECT *,
                        ROW_NUMBER()OVER(ORDER BY submittime DESC) as rowNum
						FROM $PREFIX_table_rows WHERE tableid=$[tableid]
                        ORDER BY submittime DESC) _t
						WHERE rowNum BETWEEN $skipsize+1 AND ($[skipsize]+$[pagesize])";
            }
        }
    }
}
