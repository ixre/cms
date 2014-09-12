namespace Spc.Sql
{
    using System;
    public class MySQLSqlPack : SqlPack
    {
        internal MySQLSqlPack()
        {
        }


        public override string Archive_GetAllArchive
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`title`,
                        `flags`,`author`,`thumbnail`,`viewcount`,`tags`,`outline`,
                        `content`,`createdate`,`lastmodifydate` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON  $PREFIX_categories.`id`=$PREFIX_archives.`cid` WHERE " +
                       SqlConst.Archive_NotSystemAndHidden + " ORDER BY `createdate` DESC";
            }
        }

        public override string Archive_GetSelfAndChildArchives
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`title`,`flags`
                        ,`thumbnail`,`outline`,`content`,`viewcount`,
                        lastmodifydate,author,tags,source,`createdate`,
                        $PREFIX_categories.`name`,$PREFIX_categories.`tag` FROM $PREFIX_archives 
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE " + SqlConst.Archive_NotSystemAndHidden +
                        @" AND (`lft`>=@lft AND `rgt`<=@rgt) AND $PREFIX_categories.siteId=@siteId 
                        ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetSelfAndChildArchiveExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,fieldId as extendFieldId,f.name as fieldName,fieldValue as extendFieldValue,relationId
	                    FROM $PREFIX_extendValue v INNER JOIN $PREFIX_extendField f ON v.fieldId=f.id
	                    WHERE relationType=@relationType AND relationId IN (SELECT id FROM (
                        SELECT $PREFIX_archives.id
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON 
                        $PREFIX_categories.id=$PREFIX_archives.cid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden
                                + @" AND (lft>=@lft AND rgt<=@rgt) 
                        ORDER BY createdate DESC LImIT 0,{0}
                        
                         ) as t)";
            }
        }

        public override string Archive_GetArchivesExtendValues
        {
            get
            {
                return @"
                        SELECT v.id as id,fieldId as extendFieldId,f.name as fieldName,fieldValue as extendFieldValue,relationId
	                    FROM $PREFIX_extendValue v INNER JOIN $PREFIX_extendField f ON v.fieldId=f.id
	                    WHERE relationType=@relationType AND relationId IN (SELECT id FROM (
                        SELECT $PREFIX_archives.id FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.id=$PREFIX_archives.cid
                        WHERE tag=@Tag AND " + SqlConst.Archive_NotSystemAndHidden
                        + @" ORDER BY createdate DESC LImIT 0,{0}
                      
                          ) as t)";
            }
        }

        public override string Archive_GetArchivesByCategoryAlias
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`title`,`thumbnail`,`flags`,`outline`,`content`,`viewcount`,
                        author,lastmodifydate,source,tags,`createdate`,$PREFIX_categories.`name`,$PREFIX_categories.`tag` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE `tag`=@Tag AND " + SqlConst.Archive_NotSystemAndHidden + @" ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`cid`,`flags`,`strid`,`alias`,`title`,`thumbnail`,`outline`,`content`,`source`,`tags`,
                        $PREFIX_categories.`name`,$PREFIX_categories.`tag`,`viewcount`,`createdate`,`lastmodifydate`
                        FROM $PREFIX_archives INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        AND $PREFIX_categories.siteid=@siteid
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND `moduleid`=@ModuleID
                        ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,$PREFIX_categories.`id` as 'cid',`flags`,
                        `strid`,`alias`,`title`,`outline`,`content`,`thumbnail`,
                        $PREFIX_categories.`name`,$PREFIX_categories.`tag` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND (`lft`>=@lft AND `rgt`<=@rgt)
                        ORDER BY `viewcount` DESC LImIT 0,{0}";
            }
        }



        public override string Archive_GetArchivesByViewCountDesc_Tag
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`cid`,`flags`,`strid`,
                        `alias`,`title`,`outline`,`content`,`thumbnail`,
                        $PREFIX_categories.`name`,$PREFIX_categories.`tag` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND `tag`=@tag
                        ORDER BY `viewcount` DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetArchivesByModuleIDAndViewCountDesc
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`cid`,`flags`,`strid`,`alias`,`title`,
                        `outline`,`content`,`thumbnail`,
                        $PREFIX_categories.`name`,$PREFIX_categories.`tag` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid` 
                        WHERE " + SqlConst.Archive_NotSystemAndHidden + @" AND `moduleid`=@ModuleID
                        ORDER BY `viewcount` DESC LImIT 0,{0}";
            }
        }



        public override string Archive_GetSpecialArchivesByCategoryID
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`flags`,`title`,`thumbnail`,
                        `content`,`outline`,`tags`,`createdate`,`lastmodifydate`,`viewcount`,`source` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE " + SqlConst.Archive_Special +
                        @" AND (`lft`>=@lft AND `rgt`<=@rgt)
                        ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }
        public override string Archive_GetSpecialArchivesByCategoryTag
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`flags`,`title`,`thumbnail`,
                        `content`,`outline`,`tags`,`createdate`,`lastmodifydate`
                        ,`viewcount`,`source` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE " + SqlConst.Archive_Special + @" AND $PREFIX_categories.`tag`=@CategoryTag
                        ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }
        public override string Archive_GetSpecialArchivesByModuleID
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`flags`,`title`,`content`,
                        `thumbnail`,`outline`,`tags`,`createdate`,`lastmodifydate`
                        ,`viewcount`,`source` FROM $PREFIX_archives
                            INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                            WHERE " + SqlConst.Archive_Special + @" AND $PREFIX_categories.`moduleid`=@moduleID
                            ORDER BY `createdate` DESC LImIT 0,{0}";
            }
        }

        public override string Archive_GetFirstSpecialArchiveByCategoryID
        {
            get
            {
                return @"SELECT * FROM $PREFIX_archives WHERE `cid`=@CategoryId AND " + SqlConst.Archive_Special + @" ORDER BY `createdate` DESC LImIT 0,1";
            }
        }

        public override string Archive_GetPreviousSameCategoryArchive
        {
            get
            {
                return @"SELECT `id`,a.`cid`,`strid`,`alias`,`title`,`thumbnail`,a.`createdate` FROM $PREFIX_archives a,
                                 (SELECT `cid`,`createdate` FROM $PREFIX_archives WHERE `id`=@id LImIT 0,1) as t
                                 WHERE a.`cid`=t.`cid` AND a.`createdate`<t.`createdate` AND "
                                 + SqlConst.Archive_NotSystemAndHidden +
                                 @" ORDER BY a.`createdate`  DESC LImIT 0,1";
            }
        }

        public override string Archive_GetNextSameCategoryArchive
        {
            get
            {
                return @"SELECT `id`,a.`cid`,`strid`,`alias`,`title`,`thumbnail`,a.`createdate` FROM $PREFIX_archives a,
                                 (SELECT `cid`,`createdate` FROM $PREFIX_archives WHERE `id`=@id LImIT 0,1) as t
                                 WHERE a.`cid`=t.`cid` AND a.`createdate`>t.`createdate` AND "
                                 + SqlConst.Archive_NotSystemAndHidden +
                                 @" ORDER BY a.`createdate`,a.`id` LImIT 0,1";
            }
        }

        public override string Archive_GetPagedArchivesByCategoryID_pagerquery
        {
            get
            {
                return @"SELECT `$PREFIX_archives`.`ID` AS `id`,`$PREFIX_archives`.* FROM $PREFIX_archives
                         INNER JOIN $PREFIX_categories ON cid=$PREFIX_categories.id
                          WHERE $PREFIX_archives.id IN (SELECT id FROM (
						 SELECT $PREFIX_archives.id FROM $PREFIX_archives
                         INNER JOIN $PREFIX_categories ON cid=$PREFIX_categories.id
                         WHERE $PREFIX_categories.siteId=@siteId AND (lft>=@lft AND rgt<=@rgt) 
                         AND " + SqlConst.Archive_NotSystemAndHidden + @" 
                         ORDER BY createdate DESC LImIT $[skipsize],$[pagesize]) as _t) ORDER BY createdate DESC";

                //INNER JOIN $PREFIX_modules ON $PREFIX_categories.`moduleid`=$PREFIX_modules.`id`
            }
        }


        public override string Archive_GetpagedArchivesCountSql
        {
            get
            {
//                return @"SELECT COUNT(a.id) FROM $PREFIX_archives a
//                        Left JOIN ($PREFIX_categories c INNER JOIN $PREFIX_modules m) ON
//                        a.cid=c.id AND c.moduleid=m.id
//                        Where {0}";

                return @"SELECT COUNT(a.id) FROM $PREFIX_archives a
                        INNER JOIN $PREFIX_categories c
                        ON a.cid=c.id Where {0}";
            }
        }

        public override string Archive_GetPagedArchivesByCategoryID
        {
            get
            {
                return @" SELECT a.`id` AS `id`,`strid`,`alias`,`title`,`thumbnail`,
                        c.`name` as categoryName,`cid`,`flags`,`author`,`content`,`source`,
                        `createDate`,`viewCount` FROM $PREFIX_archives a
                        INNER JOIN $PREFIX_categories c ON c.id=a.cid
                        WHERE a.id IN (SELECT id FROM (SELECT a.`ID` AS `id` FROM $PREFIX_archives a
                        INNER JOIN $PREFIX_categories c ON a.cid=c.ID
                        WHERE $[condition] ORDER BY $[orderByField] $[orderASC] LImIT $[skipsize],$[pagesize]) _t)
                         ORDER BY $[orderByField] $[orderASC]";
            }
        }


        public override string Archive_GetPagedOperations
        {
            get
            {
                return @"SELECT * FROM $PREFIX_operations,(SELECT id FROM $PREFIX_operations LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_operations.id=_t.id";
            }
        }

        public override string Message_GetPagedMessages
        {
            get
            {
                return @"SELECT * FROM $PREFIX_Message,
						(SELECT id FROM $PREFIX_Message WHERE Recycle=0 AND $[condition] ORDER BY [SendDate] DESC LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_Message.id=_t.id";
            }

        }

        public override string Member_GetPagedMembers
        {
            get
            {
                return @"SELECT $PREFIX_members.`id`,`username`,`avatar`,`nickname`,`RegIp`,`regtime`,`lastlogintime`
                        FROM $PREFIX_members INNER JOIN $PREFIX_memberdetails ON `id`=$PREFIX_memberdetails.uid,
						(SELECT $PREFIX_members.`id` FROM $PREFIX_members INNER JOIN $PREFIX_memberdetails ON $PREFIX_members.`id`=$PREFIX_memberdetails.uid
                         ORDER BY $PREFIX_members.`id` DESC LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_members.id=_t.id";
            }
        }

        public override string Archive_GetPagedSearchArchives
        {
            get
            {
                return @"SELECT *,$PREFIX_archives.`ID` AS `id` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=
                        $PREFIX_categories.`ID` WHERE $PREFIX_archives.id IN (SELECT id FROM 
						(SELECT $PREFIX_archives.`id` AS `id` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=
                        $PREFIX_categories.`ID` WHERE $[condition] AND $PREFIX_categories.siteid=$[siteid]
                        AND (`title` LIKE '%$[keyword]%'
                        OR `outline` LIKE '%$[keyword]%' OR `content` LIKE '%$[keyword]%'
                        OR `tags` LIKE '$[keyword]%') $[orderby] LImIT $[skipsize],$[pagesize]) _t)
                         $[orderby]";
            }
        }

        public override string Archive_GetPagedSearchArchivesByModuleID
        {
            get
            {
                return @"SELECT *,$PREFIX_archives.`ID` AS `id` FROM  $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`,
						(SELECT $PREFIX_archives.`ID` AS `id` FROM  $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`
                        WHERE $[condition] AND $PREFIX_categories.moduleid=$[moduleid] AND
                        (`title` LIKE '%$[keyword]%' OR `outline` LIKE '%$[keyword]%'
                        OR `content` LIKE '%$[keyword]%' OR `tags` LIKE '%$[keyword]%')
                        $[orderby] LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_archives.id=_t.id";
            }
        }

        public override string Archive_GetPagedSearchArchivesByCategoryID
        {
            get
            {
                return @"SELECT *,$PREFIX_archives.`ID` AS `id` FROM  $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`
                        WHERE $PREFIX_archives.id IN (SELECT id FROM 
						(SELECT $PREFIX_archives.id FROM  $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`
                        WHERE $[condition]  AND $PREFIX_categories.siteid=@siteId AND ($PREFIX_categories.lft>=@lft 
                        AND $PREFIX_categories.rgt<=@rgt)
                        AND (`title` LIKE '%$[keyword]%' OR `outline` LIKE '%$[keyword]%'
                        OR `content` LIKE '%$[keyword]%' OR `tags` LIKE '%$[keyword]%')
                        $[orderby] LImIT $[skipsize],$[pagesize]) _t)";
            }
        }

        public override string Archive_GetPagedOperationsByAvialble
        {
            get
            {
                return @"SELECT * FROM $PREFIX_operation,
						 (SELECT id FROM $PREFIX_operations WHERE $[condition] LImIT  $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_operation.id=_t.id";
            }
        }

        public override string Archive_GetArchivesByCondition
        {
            get
            {
                return @"SELECT $PREFIX_archives.`id`,`strid`,`alias`,`cid`,`title`,`tags`,`outline`,`thumbnail`,
                        `content`,`issystem`,`isspecial`,`visible`,`createdate` FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_categories.`id`=$PREFIX_archives.`cid`
                        WHERE {0} ORDER BY `createdate` DESC";
            }
        }

        public override string Comment_GetCommentsForArchive
        {
            get { return "SELECT * FROM $PREFIX_comments LEFT JOIN $PREFIX_members ON memberid=$PREFIX_members.id WHERE archiveid=@archiveId"; }
        }



        public override string Link_AddSiteLink
        {
            get { return @"INSERT INTO $PREFIX_links (`siteid`,`pid`,`type`,`text`,`uri`,
                        `imgurl`,`target`,`bind`,`visible`,`orderIndex`)VALUES(@siteid,@pid,
                        @TypeID,@Text,@Uri,@imgurl,@Target,@bind,@visible,@orderIndex)"; }
        }

        public override string Link_UpdateSiteLink
        {
            get { return @"UPDATE $PREFIX_links SET `pid`=@pid,`type`=@TypeID,`text`=@Text,
                          `uri`=@Uri,`imgurl`=@imgurl,`target`=@Target,`bind`=@bind,
                           visible=@visible,`orderIndex`=@orderIndex WHERE `ID`=@LinkId AND siteid=@siteId";
            }
        }

        public override string Archive_Add
        {
            get
            {
                return @"INSERT INTO $PREFIX_archives(strid,`alias`,`cid`,`author`,`title`,`flags`,
                                    `source`,`thumbnail`,`outline`,`content`,`tags`,`agree`,`disagree`,`viewcount`,
                                     `createdate`,`lastmodifydate`)
                                    VALUES(@strid,@alias,@CategoryId,@Author,@Title,@Flags,@Source,@thumbnail,@Outline,
                                    @Content,@Tags,0,0,1,@CreateDate,@LastModifyDate)";
            }
        }

        public override string Comment_GetCommentDetailsListForArchive
        {
            get
            {
                return @"SELECT $PREFIX_comments.`id` as cid,`IP`,`content`,`createDate`,
                       $PREFIX_members.`id` as uid,`avatar`,`nickname` FROM $PREFIX_comments 
                        INNER JOIN $PREFIX_members ON $PREFIX_comments.`memberID`=$PREFIX_members.`id`
                        WHERE `archiveid`=@archiveID ORDER BY `createdate` DESC";
            }
        }

        public override string Archive_Update
        {
            get
            {
                return @"UPDATE $PREFIX_archives SET `cid`=@CategoryId,`title`=@Title,`flags`=@flags,
                        `alias`=@Alias,`source`=@Source,`thumbnail`=@thumbnail,lastmodifydate=@lastmodifyDate,
                        `outline`=@Outline,`content`=@Content,`tags`=@Tags WHERE $PREFIX_archives.id=@id";
            }
        }


        public override string Archive_GetSearchRecordCountByModuleID
        {
            get
            {
                return @"SELECT COUNT(0) FROM $PREFIX_archives
                        INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`
                        WHERE {2} AND $PREFIX_categories.`moduleid`={0} AND 
                        (`title` LIKE '%{1}%' OR `outline` LIKE '%{1}%' OR `content` LIKE '%{1}%' OR `tags` LIKE '%{1}%')";
            }
        }

        public override string Archive_GetSearchRecordCountByCategoryID
        {
            get
            {
                return @"SELECT COUNT($PREFIX_archives.`id`) FROM $PREFIX_archives
                         INNER JOIN $PREFIX_categories ON $PREFIX_archives.`cid`=$PREFIX_categories.`id`
                         WHERE {1} AND $PREFIX_categories.siteid=@siteId AND ($PREFIX_categories.lft>=@lft AND $PREFIX_categories.rgt<=@rgt)
                         AND (`title` LIKE '%{0}%' OR `outline` LIKE '%{0}%' 
                         OR `content` LIKE '%{0}%' OR `tags` LIKE '%{0}%')";
            }
        }

        public override string Comment_AddComment
        {
            get { return "INSERT INTO $PREFIX_comments (`archiveid`,`memberid`,`ip`,`content`,`recycle`,`createdate`)VALUES(@ArchiveId,@MemberID,@IP,@Content,@Recycle,@CreateDate)"; }
        }

        public override string Member_RegisterMember
        {
            get { return "INSERT INTO $PREFIX_members(`username`,`password`,`avatar`,`sex`,`nickname`,`note`,`email`,`telephone`)values(@username,@password,@Avatar,@sex,@nickname,@note,@email,@Telephone)"; }
        }

        public override string Member_Update
        {
            get { return "UPDATE $PREFIX_members SET `password`=@Password,`avatar`=@Avatar,`sex`=@Sex,`nickname`=@Nickname,`email`=@Email,`telephone`=@Telephone,`note`=@Note WHERE `id`=@Id"; }
        }


        public override string Table_GetLastedRowID
        {
            get { return "SELECT `id` FROM $PREFIX_table_rows ORDER BY `id` DESC LImIT 0,1"; }
        }
        public override string Table_InsertRowData
        {
            get { return "INSERT INTO $PREFIX_table_rowsdata (`rid`,`cid`,`value`) VALUES(@rowid,@columnid,@value)"; }
        }
        public override string Table_GetPagedRows
        {
            get
            {
                return @"SELECT * FROM $PREFIX_table_rows,
						(SELECT id FROM $PREFIX_table_rows WHERE `tableid`=$[tableid] ORDER BY `submittime` DESC LImIT $[skipsize],$[pagesize]) _t
						 WHERE $PREFIX_table_rows.id=_t.id";
            }
        }
    }
}
