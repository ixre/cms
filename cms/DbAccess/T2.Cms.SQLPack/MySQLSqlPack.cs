namespace T2.Cms.Sql
{
    public class MySQLSqlPack : SqlPack
    {
        internal MySQLSqlPack()
        {
        }


        public override string Archive_GetAllArchive
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`title`,$PREFIX_archive.`location`,
                        small_title,`flags`,`author_id`,`thumbnail`,view_count,`tags`,`outline`,
                        `content`,`createdate`,`update_time` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON  $PREFIX_category.`id`=$PREFIX_archive.`cat_id` WHERE " +
                       SqlConst.Archive_NotSystemAndHidden + " ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Archive_GetArchiveList
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`title`,$PREFIX_archive.`location`,
                        small_title,`flags`,`thumbnail`,`outline`,`content`,view_count,
                        update_time,author_id,tags,source,`createdate`,
                        $PREFIX_category.`name`,$PREFIX_category.`tag` FROM $PREFIX_archive 
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        WHERE " + SqlConst.Archive_NotSystemAndHidden +
                        @" AND $PREFIX_archive.cat_id IN($[catIdArray]) AND $PREFIX_archive.site_id=@siteId 
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
	                    WHERE relation_type=@relationType AND relation_id IN (SELECT id FROM (
                        SELECT $PREFIX_archive.id
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON 
                        $PREFIX_category.id=$PREFIX_archive.cat_id
                        WHERE " + SqlConst.Archive_NotSystemAndHidden
                                + @" AND $PREFIX_archive.cat_id IN($[catIdArray]) AND $PREFIX_archive.site_id=@siteId 
                        ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}
                         ) as t)";
            }
        }

        public override string Archive_GetArchivesExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,field_id as extendFieldId,f.name as fieldName,field_value as extendFieldValue,relation_id
	                    FROM $PREFIX_extend_value v INNER JOIN $PREFIX_extend_field f ON v.field_id=f.id
	                    WHERE relation_type=@relationType AND relation_id IN (SELECT id FROM (
                        SELECT $PREFIX_archive.id FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cat_id
                        WHERE tag=@Tag AND $PREFIX_category.site_id=@siteId AND " + SqlConst.Archive_NotSystemAndHidden
                        + @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}
                      
                          ) as t)";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`title`,$PREFIX_archive.`location`,`thumbnail`,`flags`,`outline`,`content`,view_count,
                        author_id,update_time,source,tags,`createdate`,$PREFIX_category.`name`,$PREFIX_category.`tag` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        WHERE site_id=@siteId AND  `tag`=@tag AND " + SqlConst.Archive_NotSystemAndHidden +
                        @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}";
            }
        }

        public override string Archive_GetArchivesByModuleId
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`cat_id`,`flags`,`str_id`,`alias`,`title`,$PREFIX_archive.`location`,`thumbnail`,`outline`,`content`,`source`,`tags`,
                        $PREFIX_category.`name`,$PREFIX_category.`tag`,view_count,`createdate`,`update_time`
                        FROM $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        AND $PREFIX_category.site_id=@siteId
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND `module_id`=@ModuleID
                        ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,$PREFIX_category.`id` as 'cat_id',`flags`,
                        `str_id`,`alias`,`title`,$PREFIX_archive.`location`,`outline`,`content`,`thumbnail`,
                        $PREFIX_category.`name`,$PREFIX_category.`tag` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND $PREFIX_archive.site_id=@siteId AND $PREFIX_archive.cat_id IN($[catIdArray])
                        ORDER BY view_count DESC LIMIT 0,{0}";
            }
        }




        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`cat_id`,`flags`,`str_id`,`alias`,`title`,$PREFIX_archive.`location`,
                        `outline`,`content`,`thumbnail`,
                        $PREFIX_category.`name`,$PREFIX_category.`tag` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id` 
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND site_id=@siteId AND `module_id`=@ModuleID
                        ORDER BY view_count DESC LIMIT 0,{0}";
            }
        }



        public override string Archive_GetSpecialArchiveList
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`flags`,`title`,$PREFIX_archive.`location`,`thumbnail`,
                        `content`,`outline`,`tags`,`createdate`,`update_time`,view_count,`source` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        WHERE " + SqlConst.Archive_Special +
                        @" AND $PREFIX_archive.site_id=@siteId AND $PREFIX_archive.cat_id IN($[catIdArray])
                        ORDER BY $PREFIX_archive.sort_number DESC LIMIT {0},{1}";
            }
        }
        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`flags`,`title`,$PREFIX_archive.`location`,`content`,
                        `thumbnail`,`outline`,`tags`,`createdate`,`update_time`
                        ,view_count,`source` FROM $PREFIX_archive
                            INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                            WHERE " + SqlConst.Archive_Special + @" AND site_id=@siteId AND $PREFIX_category.`module_id`=@moduleID
                            ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,{0}";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get
            {
                return @"SELECT * FROM $PREFIX_archive WHERE `cat_id`=@CategoryId AND site_id=@siteId AND "
                    + SqlConst.Archive_Special + @" ORDER BY $PREFIX_archive.sort_number DESC LIMIT 0,1";
            }
        }


        public override string Archive_GetPreviousArchive
        {
            get
            {
                return @"SELECT `id`,a.`cat_id`,`str_id`,`alias`,`title`,a.`location`,`thumbnail`,a.`createdate`,a.sort_number FROM $PREFIX_archive a,
                                 (SELECT `cat_id`,`sort_number` FROM $PREFIX_archive WHERE `id`=@id LIMIT 0,1) as t
                                 WHERE (@sameCategory <>1 OR a.`cat_id`=t.`cat_id`) AND a.`sort_number`>t.`sort_number` AND 
                                 (@special = 1 OR " + SqlConst.Archive_NotSystemAndHidden + ")" +
                                 @" ORDER BY a.`sort_number` limit 0,1";
            }
        }

        public override string Archive_GetNextArchive
        {
            get
            {
                return @"SELECT `id`,a.`cat_id`,`str_id`,`alias`,`title`,a.`location`,`thumbnail`,a.`createdate`,a.sort_number FROM $PREFIX_archive a,
                                 (SELECT `cat_id`,`sort_number` FROM $PREFIX_archive WHERE `id`=@id LIMIT 0,1) as t
                                 WHERE  (@sameCategory <>1 OR a.`cat_id`=t.`cat_id`) AND a.`sort_number`<t.`sort_number` AND
                                 (@special = 1 OR " + SqlConst.Archive_NotSystemAndHidden + ")" +
                                 " ORDER BY a.`sort_number` DESC limit 0,1";
            }
        }

        public override string ArchiveGetPagedArchivesByCategoryIdPagerquery
        {
            get
            {
                return @"SELECT `$PREFIX_archive`.`ID` AS `id`,`$PREFIX_archive`.* FROM $PREFIX_archive
                         INNER JOIN $PREFIX_category ON cat_id=$PREFIX_category.id
                          WHERE $PREFIX_archive.id IN (SELECT id FROM (
						 SELECT $PREFIX_archive.id FROM $PREFIX_archive
                         INNER JOIN $PREFIX_category ON cat_id=$PREFIX_category.id
                         WHERE $PREFIX_category.site_id=@siteId AND $PREFIX_archive.cat_id IN ($[catIdArray])  AND " + SqlConst.Archive_NotSystemAndHidden + @" 
                         ORDER BY $PREFIX_archive.sort_number DESC LIMIT $[skipsize],$[pagesize]) as _t) ORDER BY $PREFIX_archive.sort_number DESC";

                //INNER JOIN $PREFIX_modules ON $PREFIX_category.`module_id`=$PREFIX_modules.`id`
            }
        }


        public override string ArchiveGetpagedArchivesCountSql
        {
            get
            {
//                return @"SELECT COUNT(a.id) FROM $PREFIX_archive a
//                        Left JOIN ($PREFIX_category c INNER JOIN $PREFIX_modules m) ON
//                        a.cat_id=c.id AND c.module_id=m.id
//                        Where {0}";

                return @"SELECT COUNT(a.id) FROM $PREFIX_archive a
                        INNER JOIN $PREFIX_category c
                        ON a.cat_id=c.id Where {0}";
            }
        }

        public override string Archive_GetPagedArchivesByCategoryId
        {
            get
            {
                return @" SELECT a.`id` AS `id`,`str_id`,`alias`,`title`,a.`location`,`thumbnail`,
                        c.`name` as categoryName,`cat_id`,`flags`,`author_id`,`content`,`source`,
                        `createDate`,view_count FROM $PREFIX_archive a
                        INNER JOIN $PREFIX_category c ON c.id=a.cat_id
                        WHERE a.id IN (SELECT id FROM (SELECT a.`id` AS `id` FROM $PREFIX_archive a
                        INNER JOIN $PREFIX_category c ON a.cat_id=c.ID
                        WHERE $[condition] ORDER BY $[orderByField] $[orderASC] LIMIT $[skipsize],$[pagesize]) _t)
                         ORDER BY $[orderByField] $[orderASC]";
            }
        }


        public override string Archive_GetPagedOperations
        {
            get
            {
                return @"SELECT * FROM $PREFIX_operation,(SELECT id FROM $PREFIX_operation LIMIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_operation.id=_t.id";
            }
        }

        public override string Message_GetPagedMessages
        {
            get
            {
                return @"SELECT * FROM $PREFIX_Message,
						(SELECT id FROM $PREFIX_Message WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC LIMIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_Message.id=_t.id";
            }

        }

        public override string Member_GetPagedMembers
        {
            get
            {
                return @"SELECT $PREFIX_member.`id`,`username`,`avatar`,`nickname`,`RegIp`,`regtime`,`lastlogintime`
                        FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails ON `id`=$PREFIX_memberdetails.uid,
						(SELECT $PREFIX_member.`id` FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails ON $PREFIX_member.`id`=$PREFIX_memberdetails.uid
                         ORDER BY $PREFIX_member.`id` DESC LIMIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_member.id=_t.id";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                return @"SELECT $PREFIX_archive.* FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=
                        $PREFIX_category.id WHERE $[condition] $[orderby] LIMIT $[skipsize],$[pagesize]";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByModuleId
        {
            get
            {
                return @"SELECT *,$PREFIX_archive.`ID` AS `id` FROM  $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`,
						(SELECT $PREFIX_archive.`ID` AS `id` FROM  $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`
                        WHERE $[condition] AND $PREFIX_category.module_id=$[module_id] AND
                        (`title` LIKE '%$[keyword]%' OR `outline` LIKE '%$[keyword]%'
                        OR `content` LIKE '%$[keyword]%' OR `tags` LIKE '%$[keyword]%')
                        $[orderby] LIMIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_archive.id=_t.id";
            }
        }

        public override string ArchiveGetPagedSearchArchivesByCategoryId
        {
            get
            {
                return @"SELECT *,$PREFIX_archive.`id` AS `id` FROM  $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`
                        WHERE $PREFIX_archive.id IN (SELECT id FROM 
						(SELECT $PREFIX_archive.id FROM  $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`
                        WHERE $[condition]  AND $PREFIX_category.site_id=@siteId AND ($PREFIX_category.lft>=@lft 
                        AND $PREFIX_category.rgt<=@rgt)
                        AND (`title` LIKE '%$[keyword]%' OR `outline` LIKE '%$[keyword]%'
                        OR `content` LIKE '%$[keyword]%' OR `tags` LIKE '%$[keyword]%')
                        $[orderby] LIMIT $[skipsize],$[pagesize]) _t)";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get
            {
                return @"SELECT * FROM $PREFIX_operation,
						 (SELECT id FROM $PREFIX_operation WHERE $[condition] LIMIT  $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_operation.id=_t.id";
            }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archive.`id`,`str_id`,`alias`,`cat_id`,`title`,$PREFIX_archive.`location`,
                        small_title,sort_number,`tags`,`outline`,`thumbnail`,
                        `content`,`issystem`,`isspecial`,`visible`,`createdate` FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_category.`id`=$PREFIX_archive.`cat_id`
                        WHERE {0} ORDER BY $PREFIX_archive.sort_number DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get { return "SELECT * FROM $PREFIX_comment LEFT JOIN $PREFIX_member ON memberid=$PREFIX_member.id WHERE archiveid=@archiveId"; }
        }



        public override string Link_AddSiteLink
        {
            get { return @"INSERT INTO $PREFIX_link (`site_id`,`pid`,`type`,`text`,`uri`,
                        `img_url`,`target`,`bind`,`visible`,`sort_number`)VALUES(@siteId,@pid,
                        @TypeID,@Text,@Uri,@imgurl,@Target,@bind,@visible,@sortNumber)"; }
        }

        public override string Link_UpdateSiteLink
        {
            get { return @"UPDATE $PREFIX_link SET `pid`=@pid,`type`=@TypeID,`text`=@Text,
                          `uri`=@Uri,`img_url`=@imgurl,`target`=@Target,`bind`=@bind,
                           visible=@visible,`sort_number`=@sortNumber WHERE `ID`=@LinkId AND site_id=@siteId";
            }
        }

        public override string ArchiveAdd
        {
            get
            {
                return @"INSERT INTO $PREFIX_archive(site_id,str_id,`alias`,`cat_id`,`author_id`,`path`,`flag`,`title`,small_title,`flags`,
           `location`,sort_number,`source`,`thumbnail`,`outline`,`content`,`tags`,
            `agree`,`disagree`,view_count,`create_time`,`update_time`)VALUES(@siteId,@strId,@alias,@catId,@authorId,@path,@flag,@title,
                                    @smallTitle,@flags,@location,@sortNumber,
                                    @source,@thumbnail,@outline, @content,@tags,0,0,1,@createTime,
                                    @updateTime)";
            }
        }

        public override string Comment_GetCommentDetailsListForArchive
        {
            get
            {
                return @"SELECT $PREFIX_comment.`id` as cat_id,`IP`,`content`,`createDate`,
                       $PREFIX_member.`id` as uid,`avatar`,`nickname` FROM $PREFIX_comment 
                        INNER JOIN $PREFIX_member ON $PREFIX_comment.`memberID`=$PREFIX_member.`id`
                        WHERE `archiveid`=@archiveID ORDER BY `createdate` DESC";
            }
        }

        public override string ArchiveUpdate
        {
            get
            {
                return @"UPDATE $PREFIX_archive SET `cat_id`=@catId,path=@path,flag=@flag,`title`=@Title,small_title=@smallTitle,sort_number=@sortNumber,`flags`=@flags,
                   `alias`=@Alias,`location`=@location,`source`=@Source,
`thumbnail`=@thumbnail,update_time=@updateTime,
                        `outline`=@Outline,`content`=@Content,`tags`=@Tags WHERE $PREFIX_archive.id=@id";
            }
        }


        public override string ArchiveGetSearchRecordCountByModuleId
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archive
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`
                        WHERE {2} AND $PREFIX_category.`module_id`={0} AND 
                        (`title` LIKE '%{1}%' OR `outline` LIKE '%{1}%' OR `content` LIKE '%{1}%' OR `tags` LIKE '%{1}%')";
            }
        }

        public override string ArchiveGetSearchRecordCountByCategoryId
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archive.`id`) FROM $PREFIX_archive
                         INNER JOIN $PREFIX_category ON $PREFIX_archive.`cat_id`=$PREFIX_category.`id`
                         WHERE {1} AND $PREFIX_category.site_id=@siteId AND ($PREFIX_category.lft>=@lft AND $PREFIX_category.rgt<=@rgt)
                         AND (`title` LIKE '%{0}%' OR `outline` LIKE '%{0}%' 
                         OR `content` LIKE '%{0}%' OR `tags` LIKE '%{0}%')";
            }
        }

        public override string Comment_AddComment
        {
            get { return "INSERT INTO $PREFIX_comment (`archiveid`,`memberid`,`ip`,`content`,`recycle`,`createdate`)VALUES(@ArchiveId,@MemberID,@IP,@Content,@Recycle,@CreateDate)"; }
        }

        public override string Member_RegisterMember
        {
            get { return "INSERT INTO $PREFIX_member(`username`,`password`,`avatar`,`sex`,`nickname`,`note`,`email`,`telephone`)values(@username,@password,@Avatar,@sex,@nickname,@note,@email,@Telephone)"; }
        }

        public override string Member_Update
        {
            get { return "UPDATE $PREFIX_member SET `password`=@Password,`avatar`=@Avatar,`sex`=@Sex,`nickname`=@Nickname,`email`=@Email,`telephone`=@Telephone,`note`=@Note WHERE `id`=@Id"; }
        }


        public override string TableGetLastedRowId
        {
            get { return "SELECT `id` FROM $PREFIX_table_row ORDER BY `id` DESC LIMIT 0,1"; }
        }
        public override string TableInsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_record (row_id,col_id,`value`) VALUES(@rowId,@columnId,@value)"; }
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
