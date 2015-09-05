namespace J6.Cms.Sql
{
    public class SQLiteSqlPack : SqlPack
    {
        internal SQLiteSqlPack()
        {
        }

        public override string Archive_GetAllArchive
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],title,$PREFIX_archive.location,thumbnail,publisher_id,
                        [flags],publisher_id,view_count,[lastmodifydate],[tags],[outline],[content],small_title,
                        [createDate] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cid] WHERE  "
                        + SqlConst.Archive_NotSystemAndHidden + " ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetSelfAndChildArchives
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],cid,title,$PREFIX_archive.location,[flags],[outline],thumbnail,publisher_id,source,tags,
                        [content],lastmodifydate,[createDate],view_count,$PREFIX_category.[name],$PREFIX_category.[tag]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.[cid]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND (lft>=@lft AND rgt<=@rgt) AND $PREFIX_category.site_id=@siteId 
                        ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}";
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
                        SELECT $PREFIX_archive.id
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON 
                        $PREFIX_category.id=$PREFIX_archive.cid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden 
                                + @" AND (lft>=@lft AND rgt<=@rgt) 
                        ORDER BY $PREFIX_archive.sort_number DESC,createdate DESC LIMIT {0},{1}
                        
                        )";
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
                        SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cid
                        WHERE tag=@Tag AND " + SqlConst.Archive_NotSystemAndHidden
                        + @" ORDER BY $PREFIX_archive.sort_number DESC,createdate DESC LIMIT {0},{1}
                        
                        )";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,strId,alias,cid,title,$PREFIX_archive.location,flags,outline,
                        thumbnail,publisher_id,lastmodifydate,source,tags,
                        [content],createdate,view_count,$PREFIX_category.name,$PREFIX_category.tag
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.cid WHERE site_id=@siteId AND tag=@tag AND " +
                        SqlConst.Archive_NotSystemAndHidden + @" ORDER BY createDate DESC LIMIT {0},{1}";
            }
        }

        public override string Archive_GetArchivesByModuleId
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],[flags],title,$PREFIX_archive.location,[Outline],thumbnail,
                        [Content],[source],[tags],$PREFIX_category.[Name],$PREFIX_category.[Tag],[view_count],[createdate],[lastmodifydate]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.ID=$PREFIX_archive.cid
                    AND $PREFIX_category.site_id=@siteId
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND ModuleID=@ModuleID ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,{0}";
            }
        }
        
        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,$PREFIX_category.id as cid,flags,str_id,[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.[cid]
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND  (lft>=@lft AND rgt<=@rgt)
                    ORDER BY view_count DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByViewCountDesc_Tag
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,$PREFIX_category.id as cid,flags,str_id,[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.[cid]
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND tag=@tag
                    ORDER BY view_count DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,[cid],flags,str_id,[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],
					$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category 
					ON $PREFIX_category.ID=$PREFIX_archive.cid WHERE "
                    + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND
					ModuleID=@ModuleID ORDER BY view_count DESC LIMIT 0,{0}";
            }
        }



        public override string Archive_GetSpecialArchivesByCategoryId
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],[flags],thumbnail,
                        title,$PREFIX_archive.location,[content],[outline],[tags],[createdate],[lastmodifydate]
                        ,view_count,[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cid] WHERE " + SqlConst.Archive_Special
                        + @" AND site_id=@siteId AND (lft>=@lft AND rgt<=@rgt) ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}";
            }
        }
          public override string Archive_GetSpecialArchivesByCategoryTag
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],[flags],title,$PREFIX_archive.location,
                        [content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,view_count,[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cid] WHERE " + SqlConst.Archive_Special
                        + @" AND site_id=@siteId AND $PREFIX_category.[tag]=@CategoryTag ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}";
            }
        }
        
        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],[flags],title,$PREFIX_archive.location,
                        [content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,view_count,[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cid] WHERE $PREFIX_category.[ModuleID]=@moduleID AND site_id=@siteId AND "
                        + SqlConst.Archive_Special + @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get { return "SELECT * FROM $PREFIX_archive WHERE [cid]=@CategoryId AND site_id=@siteId AND "
                + SqlConst.Archive_Special + @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,1";
            }
        }

        public override string Archive_GetPreviousArchive
        {
            get
            {
                return @"SELECT [ID],str_id,[alias],a.[cid],title,a.location,thumbnail,a.[createdate],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT [cid],sort_number FROM $PREFIX_archive WHERE ID=@id LIMIT 0,1) as t
                                 WHERE  (@sameCategory <>1 OR a.[cid]=t.[cid]) AND a.sort_number>t.sort_number AND 
                                 (@special = 1 OR " + SqlConst.Archive_NotSystemAndHidden + ")" +
                                 " ORDER BY a.sort_number LIMIT 0,1";
            }
        }

        public override string Archive_GetNextArchive
        {
            get
            {
                return @"SELECT [ID],str_id,[alias],a.[cid],title,a.location,thumbnail,a.[createdate],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT [cid],sort_number FROM $PREFIX_archive WHERE [ID]=@id LIMIT 0,1) as t
                                 WHERE  (@sameCategory <>1 OR a.[cid]=t.[cid]) AND a.sort_number<t.sort_number AND 
                                 (@special = 1 OR "+ SqlConst.Archive_NotSystemAndHidden +")" +
                                " ORDER BY a.sort_number DESC LIMIT 0,1";
            }
        }

        public override string ArchiveGetPagedArchivesByCategoryIdPagerquery
        {
            get
            {
                return @"SELECT $PREFIX_archive.id AS id,* FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                        WHERE $PREFIX_archive.id IN (SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                        WHERE $PREFIX_category.site_id=@siteId AND (lft>=@lft AND rgt<=@rgt) AND "
                        + SqlConst.Archive_NotSystemAndHidden + @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT $[skipsize],$[pagesize])
                        ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string ArchiveGetpagedArchivesCountSql
        {
            get
            {
                return @"SELECT COUNT(a.id) FROM $PREFIX_archive a
                        INNER JOIN $PREFIX_category c ON
                        a.cid=c.id Where {0}";
            }
        }


        public override string Archive_GetPagedArchivesByCategoryId
        {
            get
            {
                return @"SELECT a.id AS id,str_id,alias,title,a.location,thumbnail,
                         c.[name] as [categoryName],[cid],[flags],publisher_id,[content],[source],
                         [createDate],view_count FROM $PREFIX_archive a
                         INNER JOIN $PREFIX_category c ON a.cid=c.ID
                         WHERE a.id IN
                         (SELECT id FROM (SELECT a.id AS id FROM $PREFIX_archive a
                         INNER JOIN $PREFIX_category c ON a.cid=c.ID
                         WHERE $[condition] ORDER BY $[orderByField] $[orderASC] LIMIT $[skipsize],$[pagesize]) _t)
                         ORDER BY $[orderByField] $[orderASC]";
            }
        }

        public override string Archive_GetPagedOperations
        {
            get { return "SELECT * FROM $PREFIX_operation LIMIT $[skipsize],$[pagesize]"; }
        }

        public override string Message_GetPagedMessages
        {
            get { return "SELECT * FROM $PREFIX_Message WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC LIMIT $[skipsize],$[pagesize]"; }
       
        }

        public override string Member_GetPagedMembers
        {
            get
            {
                return @"SELECT $PREFIX_member.[id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime] FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID],
						(SELECT $PREFIX_member.[id] FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID] ORDER BY $PREFIX_member.[ID] DESC LIMIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_member.id=_t.id";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                return @"SELECT $PREFIX_archive.id AS ID,* FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=
                        $PREFIX_category.id WHERE $PREFIX_archive.id IN
					    (SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                        WHERE $[condition]  AND $PREFIX_category.site_id=$[siteid]
                        AND ([Title] LIKE '%$[keyword]%'
                        OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' 
                        OR [Tags] LIKE '$[keyword]%')
					    $[orderby] LIMIT $[skipsize],$[pagesize]) $[orderby]";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByModuleId
        {
            get
            {
                return @"SELECT $PREFIX_archive.id AS id,* FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id,
					(SELECT $PREFIX_archive.id AS ID FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                    WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[moduleid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
					 $[orderby] LIMIT $[skipsize],$[pagesize]) _t
						 $PREFIX_archive a.ID=_t.Id";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByCategoryId
        {
            get
            {
                return @"SELECT $PREFIX_archive.id AS id,* FROM  $PREFIX_archive INNER JOIN 
                         $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                         WHERE $PREFIX_archive.id IN (SELECT $PREFIX_archive.id
                         FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                         $PREFIX_archive.[cid]=$PREFIX_category.id
                         WHERE $[condition] AND $PREFIX_category.site_id=@siteId AND ($PREFIX_category.lft>=@lft AND
                         $PREFIX_category.rgt<=@rgt) AND ([Title] LIKE '%$[keyword]%' OR 
                         [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
				         $[orderby] LIMIT $[skipsize],$[pagesize])";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get { return "SELECT * FROM $PREFIX_operation WHERE $[condition] LIMIT  $[skipsize],$[pagesize]"; }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,str_id,[alias],[cid],title,$PREFIX_archive.location,[Tags],
                        [Outline],thumbnail,[Content],[IsSystem],[IsSpecial],[Visible],[CreateDate]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.[cid] WHERE {0} ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get { return "SELECT * FROM $PREFIX_comment LEFT JOIN $PREFIX_member ON [MemberID]=$PREFIX_member.[ID] WHERE [archiveID]=@archiveId"; }
        }


        public override string Link_AddSiteLink
        {
            get { return @"INSERT INTO $PREFIX_link (site_id,pid,[type],[text],[uri],
                            img_url,target,bind,visible,sort_number)VALUES(@siteId,@pid,@TypeID,
                            @Text,@Uri,@imgurl,@Target,@bind,@visible,@sortNumber)";
            }
        }

        public override string Link_UpdateSiteLink
        {
            get { return @"UPDATE $PREFIX_link SET pid=@pid,[type]=@TypeID,[text]=@Text,
                            [uri]=@Uri,img_url=@imgurl,[target]=@Target,bind=@bind,
                            visible=@visible,sort_number=@sortNumber WHERE [ID]=@LinkId AND site_id=@siteId";
            }
        }

        public override string ArchiveAdd
        {
            get
            {
                return @"INSERT INTO $PREFIX_archive(str_id,[alias],[cid],publisher_id,title,small_title,[flags],location,sort_number,
                                    [Source],[thumbnail],[Outline],[Content],[Tags],[Agree],[Disagree],view_count,
                                    [CreateDate],[LastModifyDate])
                                    VALUES(@strId,@alias,@CategoryId,@publisherId,@Title,@smallTitle,@Flags,@location,@sortNumber,
                                    @Source,@thumbnail,@Outline,@Content,@Tags,0,0,1,@CreateDate,
                                    @LastModifyDate)";
            }
        }

        public override string Comment_GetCommentDetailsListForArchive
        {
            get
            {
                return @"SELECT $PREFIX_comment.ID as cid,[IP],[content],[createDate],
                       $PREFIX_member.ID as uid,[Avatar],[NickName] FROM $PREFIX_comment INNER JOIN $PREFIX_member ON
                       $PREFIX_comment.[memberID]=$PREFIX_member.[ID] WHERE [archiveID]=@archiveID ORDER BY [createDate] DESC";
            }
        }

        public override string ArchiveUpdate
        {
            get
            {
                return @"UPDATE $PREFIX_archive SET [cid]=@CategoryId,[Title]=@Title,small_title=@smallTitle,sort_number=@sortNumber,flags=@flags,
                                    [Alias]=@Alias,location=@location,[Source]=@Source,thumbnail=@thumbnail,lastmodifydate=@lastmodifyDate,
                                    [Outline]=@Outline,[Content]=@Content,[Tags]=@Tags WHERE id=@id";
            }
        }

        public override string ArchiveGetSearchRecordCountByModuleId
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                        WHERE {2} AND $PREFIX_category.moduleid={0} AND ([Title] LIKE '%{1}%' OR [Outline] LIKE '%{1}%' OR [Content] LIKE '%{1}%' OR [Tags] LIKE '%{1}%')";
            }
        }

        public override string ArchiveGetSearchRecordCountByCategoryId
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive
                         INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=
                        $PREFIX_category.id WHERE {1} AND $PREFIX_category.site_id=@siteId
                        AND ($PREFIX_category.lft>=@lft AND 
                        $PREFIX_category.rgt<=@rgt) AND ([Title] LIKE '%{0}%' 
                        OR [Outline] LIKE '%{0}%' OR [Content] LIKE '%{0}%' 
                        OR [Tags] LIKE '%{0}%')";
            }
        }

        public override string Comment_AddComment
        {
            get { return "INSERT INTO $PREFIX_comment ([ArchiveID],[MemberID],[IP],[Content],[Recycle],[CreateDate])VALUES(@ArchiveId,@MemberID,@IP,@Content,@Recycle,@CreateDate)"; }
        }

        public override string Member_RegisterMember
        {
            get { return "INSERT INTO $PREFIX_member([Username],[Password],[Avatar],[Sex],[Nickname],[Note],[Email],[Telephone])values(@username,@password,@Avatar,@sex,@nickname,@note,@email,@Telephone)"; }
        }

        public override string Member_Update
        {
            get { return "UPDATE $PREFIX_member SET [Password]=@Password,[Avatar]=@Avatar,[Sex]=@Sex,[Nickname]=@Nickname,[Email]=@Email,[Telephone]=@Telephone,[Note]=@Note WHERE [ID]=@Id"; }
        }

        public override string TableGetLastedRowId
        {
            get { return "SELECT id FROM $PREFIX_table_row ORDER BY id DESC LIMIT 0,1"; }
        }

        public override string TableInsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_record (row_id,col_id,value) VALUES(@rowId,@columnId,@value)"; }
        }

        public override string TableGetPagedRows
        {
            get
            {
                return @"SELECT * FROM $PREFIX_table_row,
						(SELECT id FROM $PREFIX_table_row WHERE table_id=$[tableid] ORDER BY submit_time DESC LIMIT $[skipsize],$[pagesize]) _t
						WHERE $PREFIX_table_row.id=_t.id";
            }
        }

    }
}
