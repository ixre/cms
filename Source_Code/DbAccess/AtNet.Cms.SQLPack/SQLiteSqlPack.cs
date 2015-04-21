namespace AtNet.Cms.Sql
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
                return @"SELECT $PREFIX_archive.[id],[strid],[alias],[cid],title,$PREFIX_archive.location,thumbnail,author,
                        [flags],[author],[viewcount],[lastmodifydate],[tags],[outline],[content],small_title,sort_number,
                        [createDate] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.[ID]=$PREFIX_archive.[cid] WHERE  "
                        + SqlConst.Archive_NotSystemAndHidden + " ORDER BY sort_number DESC";
            }
        }

        public override string Archive_GetSelfAndChildArchives
        {
            get
            {
                return @"SELECT $PREFIX_archive.[id],[strid],[alias],cid,title,$PREFIX_archive.location,[flags],[outline],thumbnail,author,source,tags,
                        [content],lastmodifydate,[createDate],[viewCount],$PREFIX_category.[name],$PREFIX_category.[tag]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.[ID]=$PREFIX_archive.[cid]
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND (lft>=@lft AND rgt<=@rgt) AND $PREFIX_category.siteId=@siteId 
                        ORDER BY sort_number DESC LImIT 0,{0}";
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
                        SELECT $PREFIX_archive.id
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON 
                        $PREFIX_category.id=$PREFIX_archive.cid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden 
                                + @" AND (lft>=@lft AND rgt<=@rgt) 
                        ORDER BY createdate DESC LImIT 0,{0}
                        
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
                        SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cid
                        WHERE tag=@Tag AND " + SqlConst.Archive_NotSystemAndHidden
                        + @" ORDER BY createdate DESC LImIT 0,{0}
                        
                        )";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT $PREFIX_archive.id,strId,alias,cid,title,$PREFIX_archive.location,flags,outline,
                        thumbnail,author,lastmodifydate,source,tags,
                        [content],createdate,viewcount,$PREFIX_category.name,$PREFIX_category.tag
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.id=$PREFIX_archive.cid WHERE siteid=@siteid AND tag=@tag AND " +
                        SqlConst.Archive_NotSystemAndHidden + @" ORDER BY createDate DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[strid],[alias],[cid],[flags],title,$PREFIX_archive.location,[Outline],thumbnail,
                        [Content],[source],[tags],$PREFIX_category.[Name],$PREFIX_category.[Tag],[viewcount],[createdate],[lastmodifydate]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.ID=$PREFIX_archive.cid
                    AND $PREFIX_category.siteid=@siteid
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND siteid=@siteId AND ModuleID=@ModuleID ORDER BY sort_number DESC LImIT 0,{0}";
            }
        }
        
        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],$PREFIX_category.[ID] as cid,flags,[strid],[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.[ID]=$PREFIX_archive.[cid]
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND siteid=@siteId AND  (lft>=@lft AND rgt<=@rgt)
                    ORDER BY [ViewCount] DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByViewCountDesc_Tag
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],$PREFIX_category.[ID] as cid,flags,[strid],[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.[ID]=$PREFIX_archive.[cid]
                    WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND siteid=@siteId AND tag=@tag
                    ORDER BY [ViewCount] DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[cid],flags,[strid],[alias],title,$PREFIX_archive.location,[Outline],thumbnail,[Content],
					$PREFIX_category.[Name],$PREFIX_category.[Tag]
                    FROM $PREFIX_archive INNER JOIN $PREFIX_category 
					ON $PREFIX_category.ID=$PREFIX_archive.cid WHERE "
                    + SqlConst.Archive_NotSystemAndHidden + @" AND siteid=@siteId AND
					ModuleID=@ModuleID ORDER BY [ViewCount] DESC LImIT 0,{0}";
            }
        }



        public override string Archive_GetSpecialArchivesByCategoryID
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[strid],[alias],[cid],[flags],thumbnail,
                        title,$PREFIX_archive.location,[content],[outline],[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.[ID]=$PREFIX_archive.[cid] WHERE " + SqlConst.Archive_Special
                        + @" AND siteid=@siteId AND (lft>=@lft AND rgt<=@rgt) ORDER BY sort_number DESC LImIT 0,{0}";
            }
        }
          public override string Archive_GetSpecialArchivesByCategoryTag
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[strid],[alias],[cid],[flags],title,$PREFIX_archive.location,
                        [content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.[ID]=$PREFIX_archive.[cid] WHERE " + SqlConst.Archive_Special
                        + @" AND siteid=@siteId AND $PREFIX_category.[tag]=@CategoryTag ORDER BY sort_number DESC LImIT 0,{0}";
            }
        }
        
        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[strid],[alias],[cid],[flags],title,$PREFIX_archive.location,
                        [content],[outline],thumbnail,[tags],[createdate],[lastmodifydate]
                        ,[viewcount],[source] FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.[ID]=$PREFIX_archive.[cid] WHERE $PREFIX_category.[ModuleID]=@moduleID AND siteid=@siteId AND "
                        + SqlConst.Archive_Special + @" ORDER BY sort_number DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get { return "SELECT * FROM $PREFIX_archive WHERE [cid]=@CategoryId AND siteid=@siteId AND " 
                + SqlConst.Archive_Special + @" ORDER BY sort_number DESC LImIT 0,1"; }
        }

        public override string Archive_GetPreviousArchive
        {
            get
            {
                return @"SELECT [ID],[strid],[alias],a.[cid],title,a.location,thumbnail,a.[createdate],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT [cid],sort_number FROM $PREFIX_archive WHERE ID=@id LImIT 0,1) as t
                                 WHERE  (@sameCategory <>1 OR a.[cid]=t.[cid]) AND a.sort_number>t.sort_number AND 
                                 (@special = 1 OR " + SqlConst.Archive_NotSystemAndHidden + ")" +
                                 " ORDER BY a.sort_number LIMIT 0,1";
            }
        }

        public override string Archive_GetNextArchive
        {
            get
            {
                return @"SELECT [ID],[strid],[alias],a.[cid],title,a.location,thumbnail,a.[createdate],a.sort_number FROM $PREFIX_archive a,
                                 (SELECT [cid],sort_number FROM $PREFIX_archive WHERE [ID]=@id LImIT 0,1) as t
                                 WHERE  (@sameCategory <>1 OR a.[cid]=t.[cid]) AND a.sort_number<t.sort_number AND 
                                 (@special = 1 OR "+ SqlConst.Archive_NotSystemAndHidden +")" +
                                " ORDER BY a.sort_number DESC LIMIT 0,1";
            }
        }

        public override string Archive_GetPagedArchivesByCategoryID_pagerquery
        {
            get
            {
                return @"SELECT $PREFIX_archive.[id] AS id,* FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID]
                        WHERE $PREFIX_archive.id IN (SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.id
                        WHERE $PREFIX_category.siteId=@siteId AND (lft>=@lft AND rgt<=@rgt) AND "
                        + SqlConst.Archive_NotSystemAndHidden + @" ORDER BY sort_number DESC LIMIT $[skipsize],$[pagesize])
                        ORDER BY sort_number DESC";
            }
        }

        public override string Archive_GetpagedArchivesCountSql
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
                return @"SELECT a.id AS id,strid,alias,title,a.location,thumbnail,
                         c.[name] as [categoryName],[cid],[flags],[author],[content],[source],
                         [createDate],[viewCount] FROM $PREFIX_archive a
                         INNER JOIN $PREFIX_category c ON a.cid=c.ID
                         WHERE a.id IN
                         (SELECT id FROM (SELECT a.id AS id FROM $PREFIX_archive a
                         INNER JOIN $PREFIX_category c ON a.cid=c.ID
                         WHERE $[condition] ORDER BY [$[orderByField]] $[orderASC] LImIT $[skipsize],$[pagesize]) _t)
                         ORDER BY [$[orderByField]] $[orderASC]";
            }
        }

        public override string Archive_GetPagedOperations
        {
            get { return "SELECT * FROM $PREFIX_operation LImIT $[skipsize],$[pagesize]"; }
        }

        public override string Message_GetPagedMessages
        {
            get { return "SELECT * FROM $PREFIX_Message WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC LImIT $[skipsize],$[pagesize]"; }
       
        }

        public override string Member_GetPagedMembers
        {
            get
            {
                return @"SELECT $PREFIX_member.[id],[username],[avatar],[nickname],[RegIp],[RegTime],[LastLoginTime] FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID],
						(SELECT $PREFIX_member.[id] FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails
                        ON $PREFIX_member.[ID]=$PREFIX_memberdetails.[UID] ORDER BY $PREFIX_member.[ID] DESC LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_member.id=_t.id";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID] AS ID,* FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=
                        $PREFIX_category.[ID] WHERE $PREFIX_archive.id IN
					    (SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID]
                        WHERE $[condition]  AND $PREFIX_category.siteid=$[siteid]
                        AND ([Title] LIKE '%$[keyword]%'
                        OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' 
                        OR [Tags] LIKE '$[keyword]%')
					    $[orderby] LIMIT $[skipsize],$[pagesize]) $[orderby]";
            }
        }

        public override string Archive_GetPagedSearchArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID] AS id,* FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID],
					(SELECT $PREFIX_archive.[ID] AS ID FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID]
                    WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[moduleid] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
					 $[orderby] LImIT $[skipsize],$[pagesize]) _t
						 $PREFIX_archive a.ID=_t.Id";
            }
        }

        public override string Archive_GetPagedSearchArchivesByCategoryID
        {
            get
            {
                return @"SELECT $PREFIX_archive.id AS id,* FROM  $PREFIX_archive INNER JOIN 
                         $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID]
                         WHERE $PREFIX_archive.id IN (SELECT $PREFIX_archive.id
                         FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                         $PREFIX_archive.[cid]=$PREFIX_category.id
                         WHERE $[condition] AND $PREFIX_category.siteid=@siteId AND ($PREFIX_category.lft>=@lft AND
                         $PREFIX_category.rgt<=@rgt) AND ([Title] LIKE '%$[keyword]%' OR 
                         [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
				         $[orderby] LImIT $[skipsize],$[pagesize])";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get { return "SELECT * FROM $PREFIX_operation WHERE $[condition] LImIT  $[skipsize],$[pagesize]"; }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archive.[ID],[strid],[alias],[cid],title,$PREFIX_archive.location,[Tags],
                        [Outline],thumbnail,[Content],[IsSystem],[IsSpecial],[Visible],[CreateDate]
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON
                        $PREFIX_category.[ID]=$PREFIX_archive.[cid] WHERE {0} ORDER BY sort_number DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get { return "SELECT * FROM $PREFIX_comment LEFT JOIN $PREFIX_member ON [MemberID]=$PREFIX_member.[ID] WHERE [archiveID]=@archiveId"; }
        }


        public override string Link_AddSiteLink
        {
            get { return @"INSERT INTO $PREFIX_link (siteid,pid,[type],[text],[uri],
                            imgurl,target,bind,visible,orderIndex)VALUES(@siteid,@pid,@TypeID,
                            @Text,@Uri,@imgurl,@Target,@bind,@visible,@orderIndex)";
            }
        }

        public override string Link_UpdateSiteLink
        {
            get { return @"UPDATE $PREFIX_link SET pid=@pid,[type]=@TypeID,[text]=@Text,
                            [uri]=@Uri,imgurl=@imgurl,[target]=@Target,bind=@bind,
                            visible=@visible,orderIndex=@orderIndex WHERE [ID]=@LinkId AND siteid=@siteId";
            }
        }

        public override string Archive_Add
        {
            get
            {
                return @"INSERT INTO $PREFIX_archive(strid,[alias],[cid],[Author],title,small_title,[flags],location,sort_number,
                                    [Source],[thumbnail],[Outline],[Content],[Tags],[Agree],[Disagree],[ViewCount],
                                    [CreateDate],[LastModifyDate])
                                    VALUES(@strId,@alias,@CategoryId,@Author,@Title,@smallTitle,@Flags,@location,@sortNumber,
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

        public override string Archive_Update
        {
            get
            {
                return @"UPDATE $PREFIX_archive SET [cid]=@CategoryId,[Title]=@Title,small_title=@smallTitle,sort_number=@sortNumber,flags=@flags,
                                    [Alias]=@Alias,location=@location,[Source]=@Source,thumbnail=@thumbnail,lastmodifydate=@lastmodifyDate,
                                    [Outline]=@Outline,[Content]=@Content,[Tags]=@Tags WHERE id=@id";
            }
        }

        public override string Archive_GetSearchRecordCountByModuleID
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=$PREFIX_category.[ID]
                        WHERE {2} AND $PREFIX_category.moduleid={0} AND ([Title] LIKE '%{1}%' OR [Outline] LIKE '%{1}%' OR [Content] LIKE '%{1}%' OR [Tags] LIKE '%{1}%')";
            }
        }

        public override string Archive_GetSearchRecordCountByCategoryID
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive
                         INNER JOIN $PREFIX_category ON $PREFIX_archive.[cid]=
                        $PREFIX_category.id WHERE {1} AND $PREFIX_category.siteid=@siteId
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

        public override string Table_GetLastedRowID
        {
            get { return "SELECT id FROM $PREFIX_table_row ORDER BY id DESC LImIT 0,1"; }
        }

        public override string Table_InsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_rowdata (rid,cid,value) VALUES(@rowid,@columnid,@value)"; }
        }

        public override string Table_GetPagedRows
        {
            get
            {
                return @"SELECT * FROM $PREFIX_table_row,
						(SELECT id FROM $PREFIX_table_row WHERE tableid=$[tableid] ORDER BY submittime DESC LImIT $[skipsize],$[pagesize]) _t
						WHERE $PREFIX_table_row.id=_t.id";
            }
        }

    }
}
