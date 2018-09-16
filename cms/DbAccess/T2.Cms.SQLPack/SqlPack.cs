using System;
using JR.DevFw.Data;

namespace T2.Cms.Sql
{
    public abstract class SqlPack
    {
        public static SqlPack Factory(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.OLEDB:
                    throw  new NotSupportedException("版本1.2及以上，不再支持Access数据库！");
                    //return new OleDbSqlPack();
				case DataBaseType.MonoSQLite:
				case DataBaseType.SQLite:
					return new SQLiteSqlPack();
                case DataBaseType.SQLServer: return new SqlServerSqlPack();
                case DataBaseType.MySQL: return new MySQLSqlPack();
			}
            throw new Exception("暂不支持此数据库！无法生成SQL包实例!");
        }

        #region 文档相关



        /// <summary>
        /// 增加文档
        /// </summary>
        public abstract string ArchiveAdd { get; }


        /// <summary>
        /// 更新文档
        /// </summary>
        public abstract string ArchiveUpdate { get; }

        /// <summary>
        /// 重新发布文档
        /// </summary>
        public readonly string ArchiveRepublish = @"UPDATE $PREFIX_archive 
                                SET createdate=@CreateDate WHERE id IN (SELECT id FROM
                                (SELECT $PREFIX_archive.id FROM $PREFIX_archive INNER JOIN 
                                $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cat_id
                                WHERE site_id=@siteId AND $PREFIX_archive.id=@id) t)";

        /// <summary>
        /// 删除文档
        /// </summary>
        public readonly string Archive_Delete = @"DELETE FROM $PREFIX_archive
                                WHERE id in (SELECT id FROM (SELECT $PREFIX_archive.id FROM $PREFIX_archive INNER JOIN 
                                $PREFIX_category ON $PREFIX_category.id=$PREFIX_archive.cat_id
                                WHERE site_id=@siteId AND $PREFIX_archive.id=@id) t)";

        /// <summary>
        /// 检查别名是否存在
        /// </summary>
        public readonly string Archive_CheckAliasIsExist =@"SELECT alias FROM $PREFIX_archive
                                WHERE site_id=@siteId AND (alias=@alias or $PREFIX_archive.str_id=@alias)";


        /// <summary>
        /// 增加浏览数量
        /// </summary>
        public readonly string Archive_AddViewCount = @"UPDATE $PREFIX_archive SET view_count=view_count+@count WHERE site_id=@siteId AND $PREFIX_archive.id=@id";

        /// <summary>
        /// 根据站点编号获取文档
        /// </summary>
        public readonly string Archive_GetArchiveByStrIDOrAlias = @"
                    SELECT * FROM $PREFIX_archive WHERE site_id=@siteId 
                    AND (alias=@strid OR $PREFIX_archive.str_id=@strid)";

        /// <summary>
        /// 根据文档编号获取文档
        /// </summary>
        public readonly string Archive_GetArchiveById = @"SELECT * FROM $PREFIX_archive WHERE 
                site_id=@siteId AND id=@id";


        /// <summary>
        /// 获取所有文档
        /// </summary>
        public abstract string Archive_GetAllArchive { get; }

        /// <summary>
        /// 根据条件获取文档
        /// </summary>
        public abstract string Archive_GetArchivesByCondition { get; }

        /// <summary>
        /// 根据栏目左右值获取获取指定数量的文档,包括子栏目的文档
        /// </summary>
        public abstract string Archive_GetSelfAndChildArchives { get; }

        /// <summary>
        /// 获取自己包含子栏目的文档扩展信息
        /// </summary>
        public abstract string Archive_GetSelfAndChildArchiveExtendValues { get; }

        /// <summary>
        /// 获取栏目的扩展信息
        /// </summary>
        public abstract string Archive_GetArchivesExtendValues { get; }

        /// <summary>
        /// 根据栏目别名获取文档
        /// </summary>
        public abstract string Archive_GetArchivesByCategoryAlias { get; }

        /// <summary>
        /// 根据模块编号获取文档
        /// </summary>
        public abstract string Archive_GetArchivesByModuleId { get; }

        /// <summary>
        /// 获取指定栏目浏览最多的文档
        /// </summary>
        public abstract string Archive_GetArchivesByViewCountDesc { get; }

        /// <summary>
        /// 获取指定栏目浏览最多的文档(使用栏目别名)
        /// </summary>
        public abstract string Archive_GetArchivesByViewCountDesc_Tag { get; }

        /// <summary>
        /// 获取指定模块浏览最多的文档
        /// </summary>
        public abstract string Archive_GetArchivesByModuleIDAndViewCountDesc { get; }


        /// <summary>
        /// 根据栏目获取特殊文档(包括子类)
        /// </summary>
        public abstract string Archive_GetSpecialArchivesByCategoryId { get; }

        /// <summary>
        /// 根据栏目获取特殊文档（不包括子类)
        /// </summary>
        public abstract string Archive_GetSpecialArchivesByCategoryTag { get; }

        /// <summary>
        /// 获取指定模块的特殊文档
        /// </summary>
        public abstract string Archive_GetSpecialArchivesByModuleID { get; }


        /// <summary>
        /// 获取指定栏目的第一篇特殊文档
        /// </summary>
        public abstract string Archive_GetFirstSpecialArchiveByCategoryID { get; }

        /// <summary>
        /// 获取上一篇文档(@sameCategory=1表示同分类)
        /// </summary>
        public abstract string Archive_GetPreviousArchive { get; }

        /// <summary>
        /// 获取下一篇文档(@sameCategory=1表示同分类)
        /// </summary>
        public abstract string Archive_GetNextArchive { get; }

        /// <summary>
        /// 删除指定会员的文档
        /// </summary>
        public readonly string ArchiveDeleteMemberArchives = "DELETE FROM $PREFIX_archive WHERE publisher_id=@Id";

        /// <summary>
        /// 切换作者
        /// </summary>
        public readonly string ArchiveTransferPublisherId = "UPDATE $PREFIX_archive SET publisher_id=@toPublisherId WHERE publisher_id=@publisherId";

        /// <summary>
        /// 获取分页文档条数(前台调用)
        /// </summary>
        public readonly string ArchiveGetPagedArchivesCountSqlPagerqurey = @"
            SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive 
            INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=$PREFIX_category.id 
            WHERE $PREFIX_category.site_id=@siteId AND " 
            + SqlConst.Archive_NotSystemAndHidden + " AND $PREFIX_archive.cat_id IN ($[catIdArray])";

        /// <summary>
        /// 根据栏目获取分页文档
        /// </summary>
        public abstract string ArchiveGetPagedArchivesByCategoryIdPagerquery { get; }


        /// <summary>
        /// 获取分页文档条数
        /// </summary>
        public abstract string ArchiveGetpagedArchivesCountSql { get; }

        /*
            INNER JOIN $PREFIX_category c INNER JOIN $PREFIX_modules m ON
            a.cat_id=c.id AND c.module_id=m.id
        */
        /// <summary>
        /// 获取分页文档条数
        /// </summary>
        public readonly string Archive_GetpagedArchivesCountSql2 = @"
            SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive
            INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=$PREFIX_category.id
            INNER JOIN $PREFIX_modules ON $PREFIX_category.module_id=$PREFIX_modules.id
            Where {0}";
        
        /// <summary>
        /// 获取栏目下的文档数量
        /// </summary>
        public readonly string Archive_GetCategoryArchivesCount =@"
            SELECT COUNT($PREFIX_archive.id) FROM $PREFIX_archive 
            INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=$PREFIX_category.id
            WHERE site_id=@siteId AND $PREFIX_category.lft BETWEEN @lft AND @rgt";
        
        /// <summary>
        /// 获取分页文档
        /// </summary>
        public abstract string Archive_GetPagedArchivesByCategoryId { get; }


        /// <summary>
        /// 获取搜索文档数量
        /// </summary>
        public readonly string Archive_GetSearchRecordCount = @"
                        SELECT COUNT(0) FROM $PREFIX_archive 
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=$PREFIX_category.id
                        WHERE {0}";

        /// <summary>
        /// 获取搜索分页符合条件的文档列表
        /// </summary>
        public abstract string Archive_GetPagedSearchArchives { get; }

        /// <summary>
        /// 获取根据模块搜索符合条件的文档数量文档数量
        /// </summary>
        public abstract string ArchiveGetSearchRecordCountByModuleId { get; }

        /// <summary>
        /// 获取根据模块搜索符合条件的文档列表
        /// </summary>
        public abstract string ArchiveGetPagedSearchArchivesByModuleId { get; }

        /// <summary>
        /// 根据栏目搜索符合条件的文档数量
        /// </summary>
        public abstract string ArchiveGetSearchRecordCountByCategoryId{get;}

        /// <summary>
        ///获取根据栏目搜索符合条件的文档列表
        /// </summary>
        public abstract string ArchiveGetPagedSearchArchivesByCategoryId { get; }

        /// <summary>
        /// 获取最大的排序号码
        /// </summary>
        public string ArchiveGetMaxSortNumber = @"SELECT MAX($PREFIX_archive.sort_number) FROM  $PREFIX_archive 
                        INNER JOIN $PREFIX_category ON $PREFIX_archive.cat_id=$PREFIX_category.id
                        WHERE $PREFIX_category.site_id=@siteId";

        /// <summary>
        /// 更新排序号
        /// </summary>
        public string ArchiveUpdateSortNumber = "UPDATE $PREFIX_archive SET sort_number=@sort_number WHERE id=@archiveId";

        #endregion

        #region 栏目相关

        /// <summary>
        /// 获取所有栏目
        /// </summary>
        public readonly string CategoryGetAllCategories = "select * from $PREFIX_category ORDER BY id";

        /// <summary>
        /// 更新栏目
        /// </summary>
        public readonly string CategoryUpdate = @"
                    UPDATE $PREFIX_category SET tag=@tag, site_id=@site_id, parent_id=@parent_id,
                    code=@code, path=@path, flag=@flag,module_id=@module_id, name=@name, icon=@icon,
                    page_title=@page_title, page_keywords=@page_keywords, page_description=@page_description, 
                    location=@location, sort_number=@sort_number WHERE id=@id";

        /// <summary>
        /// 添加栏目
        /// </summary>
        public readonly string CategoryInsert = @"
                    INSERT INTO $PREFIX_category (tag, site_id, parent_id, code, path, flag, 
                    module_id, name, icon, page_title, page_keywords, page_description, 
                    location, sort_number) VALUES(@tag,@site_id,@parent_id,@code,@path,@flag,@module_id,@name,
                    @icon,@page_title,@page_keywords,@page_description,@location,@sort_number)";

        /// <summary>
        /// 获取子栏目的数量
        /// </summary>
        //public string Category_GetChildCategoryCount = "SELECT COUNT(id) FROM $PREFIX_category WHERE pid=@pid";

        /// <summary>
        /// 删除栏目
        /// </summary>
        public readonly string Category_DeleteByLft = "DELETE FROM $PREFIX_category WHERE site_id=@siteId AND lft>=@lft AND rgt<=@rgt";

        public readonly string Category_UpdateInsertLeft = "UPDATE $PREFIX_category SET lft=lft+2 WHERE lft>@lft AND site_id=@siteId";
        public readonly string Category_UpdateInsertRight = "UPDATE $PREFIX_category SET rgt=rgt+2 WHERE rgt>@lft AND site_id=@siteId";

        public readonly string Category_UpdateDeleteLft = "UPDATE $PREFIX_category SET lft=lft-@val WHERE lft>@lft AND site_id=@siteId";
        public readonly string Category_UpdateDeleteRgt = "UPDATE $PREFIX_category SET rgt=rgt-@val WHERE rgt>@rgt AND site_id=@siteId";

        public string Category_GetMaxRight = "SELECT max(rgt) FROM $PREFIX_category WHERE site_id=@siteId";


        /*
        /// <summary>
        /// 更新左值比当前节点右值大,且小于新的父节点的左值的节点左值
        /// </summary>
        public string Category_ChangeUpdateTreeLeft = "UPDATE $PREFIX_category SET lft=lft-@rgt-@lft-1 WHERE lft>@rgt AND lft<=@tolft AND site_id=@siteId";

        /// <summary>
        /// 更新右值比当前节点右值大,且小于新的父节点的右值的节点右值
        /// </summary>
        public string Category_ChangeUpdateTreeRight="UPDATE $PREFIX_category SET rgt=rgt-@rgt-@lft-1 WHERE rgt>@rgt AND rgt<@tolft AND site_id=@siteId";
        
        /// <summary>
        /// 移动子类
        /// </summary>
        public string Category_ChangeUpdateTreeChildNodes = "UPDATE $PREFIX_category SET lft=lft+@tolft-@rgt,rgt=rgt+@tolft-@rgt WHERE lft>=@lft AND rgt<=@rgt AND site_id=@siteId";
        */


        public string Category_ChangeUpdateTreeLeft = "UPDATE $PREFIX_category SET lft=lft-(@rgt-@lft)-1 WHERE lft>@rgt AND rgt<=@torgt AND site_id=@siteId";
        public string Category_ChangeUpdateTreeRight = "UPDATE $PREFIX_category SET rgt=rgt-(@rgt-@lft)-1 WHERE rgt>@rgt AND rgt<@torgt AND site_id=@siteId";
        public string Category_ChangeUpdateTreeChildNodes = @"UPDATE $PREFIX_category SET lft=lft+(@torgt-@rgt-1),rgt=rgt+(@torgt-@rgt-1)
                                                           WHERE lft>=@lft AND rgt<=@rgt AND site_id=@siteId";

        public string Category_ChangeUpdateTreeLeft2 = "UPDATE $PREFIX_category SET lft=lft-(@rgt-@lft)+1 WHERE lft>@torgt AND lft<@lft AND site_id=@siteId";
        public string Category_ChangeUpdateTreeRight2 = "UPDATE $PREFIX_category SET rgt=rgt-(@rgt-@lft)+1 WHERE rgt>=@torgt AND rgt<@lft AND site_id=@siteId";
        public string Category_ChangeUpdateTreeBettown2 = @"UPDATE $PREFIX_category SET lft=lft-(@lft-@torgt),rgt=rgt-(@lft-@torgt)
                                                          WHERE lft>=@lft AND rgt<=@rgt AND site_id=@siteId";
        

        #endregion

        #region 用户相关

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        public readonly string UserGetUserById = "SELECT * FROM $PREFIX_user WHERE id=@id";

        public readonly string UserGetUserRole = "SELECT * FROM $PREFIX_user_role WHERE user_id=@userId";

        /// <summary>
        /// 根据用户名和密码获取用户信息
        /// </summary>
        public readonly string UserGetUserCredentialByUserName = "SELECT id,user_id,user_name,password,enabled FROM $PREFIX_credential WHERE user_name=@userName";

        public readonly string UserGetUserCredential = "SELECT id,user_id,user_name,password,enabled FROM $PREFIX_credential WHERE user_id=@userId";

        /// <summary>
        /// 获取所有用户
        /// </summary>
        public readonly string UserGetAllUsers = "SELECT * FROM $PREFIX_user ORDER BY createdate DESC";

      
        public readonly string UserGetUserIdByUserName = "SELECT user_id FROM $PREFIX_credential WHERE user_name=@userName";


        /// <summary>
        /// 创建用户
        /// </summary>
        public readonly string UserCreateUser = @"INSERT INTO $PREFIX_user(name,avatar,phone,email, check_code,
                flag,create_time,last_login_time)VALUES(@name,@avatar,@phone,@email,@checkCode,@roleFlag,@createTime,@loginTime)";


        /// <summary>
        /// 修改用户密码
        /// </summary>
        //public readonly string UserModifyPassword = "UPDATE $PREFIX_user SET password=@Password WHERE username=@UserName";

        /// <summary>
        /// 更新用户
        /// </summary>
        public readonly string UserUpdateUser = @"UPDATE $PREFIX_user SET name=@name,avatar=@avatar,phone=@phone,email=@email,
                check_code=@checkCode,flag=@roleFlag,create_time=@createTime,last_login_time=@loginTime WHERE id=@id";


        /****************** 操作相关 ******************/
        /// <summary>
        /// 检查操作的路径是否存在
        /// </summary>
        public readonly string Operation_CheckPathExist = "SELECT path FROM $PREFIX_operation WHERE path=@path";


        /// <summary>
        /// 创建新操作
        /// </summary>
        public readonly string Operation_CreateOperation = "INSERT INTO $PREFIX_operation (name,path,available) VALUES (@Name,@Path,@available)";

        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        public string Member_UpdateUserLastLoginDate = "UPDATE $PREFIX_user SET LastLoginDate=@LastLoginDate WHERE username=@username";



        #endregion

        #region 用户操作相关

        /// <summary>
        /// 删除操作
        /// </summary>
        public readonly string Operation_DeleteOperation = "DELETE FROM $PREFIX_operation WHERE id=@Id";

        /// <summary>
        /// 获取操作
        /// </summary>
        public readonly string Operation_GetOperation = "SELECT * FROM $PREFIX_operation WHERE id=@Id";

        /// <summary>
        /// 获取所有操作
        /// </summary>
        public readonly string Operation_GetOperations = "SELECT * FROM $PREFIX_operation";

        /// <summary>
        /// 更新操作
        /// </summary>
        public readonly string Operation_UpdateOperation = "UPDATE $PREFIX_operation SET name=@Name,path=@Path,available=@available WHERE id=@Id";


        /// <summary>
        /// 获取操作数
        /// </summary>
        public readonly string Operation_GetOperationCount = "SELECT COUNT(0) FROM $PREFIX_operation";

        /// <summary>
        /// 获取分页操作列表
        /// </summary>
        public abstract string Archive_GetPagedOperations { get; }


        /// <summary>
        /// 获取可用或不可用的操作数量
        /// </summary>
        public string Operation_GetOperationsCountByAvailable = "SELECT COUNT(0) FROM $PREFIX_operation WHERE {0}";


        /// <summary>
        /// 获取可用或不可用的操作分页列表
        /// </summary>
        public abstract string Archive_GetPagedOperationsByAvialble { get; }

        #endregion

        #region 用户组相关


        /// <summary>
        /// 更新用户权限
        /// </summary>
        public readonly string UserGroup_UpdatePermissions = "UPDATE $PREFIX_usergroup SET permissions=@Permissions WHERE id=@GroupId";

        /// <summary>
        /// 重命名用户名
        /// </summary>
        public readonly string UserGroup_RenameGroup = "UPDATE $PREFIX_usergroup SET name=@Name WHERE ID=@GroupId";

        #endregion

        #region 评论相关

        /// <summary>
        /// 插入评论
        /// </summary>
        public abstract string Comment_AddComment { get; }

        /// <summary>
        /// 查询文档评论数目
        /// </summary>
        public readonly string Comment_QueryCommentsCountForArchive = "SELECT count(id) FROM $PREFIX_comment WHERE archiveid=@ArchiveId";

        /// <summary>
        /// 获取文档的评论
        /// </summary>
        public abstract string Comment_GetCommentsForArchive { get; }

        /// <summary>
        /// 获取文档的评论及用户信息
        /// </summary>
        public abstract string Comment_GetCommentDetailsListForArchive { get; }


        /// <summary>
        /// 删除指定会员的评论
        /// </summary>
        public readonly string Comment_DeleteMemberComments = "DELETE FROM $PREFIX_comment WHERE memberid=@id";


        /// <summary>
        /// 删除评论
        /// </summary>
        public readonly string Comment_Delete = "DELETE FROM $PREFIX_comment WHERE id=@Id";

        /// <summary>
        /// 删除指定文档的评论
        /// </summary>
        public readonly string Comment_DeleteArchiveComments = "DELETE FROM $PREFIX_comment WHERE archiveid=@ArchiveId";


        #endregion

        #region 链接

        /// <summary>
        /// 获取链接
        /// </summary>
        public readonly string Link_GetSiteLinksByLinkType =@"SELECT * FROM $PREFIX_link WHERE $PREFIX_link.type=@linkType AND site_id=@siteId ORDER BY $PREFIX_link.sort_number";

        public readonly string Link_GetSiteLinkById = "SELECT * FROM $PREFIX_link WHERE id=@linkId AND site_id=@siteId";

        /// <summary>
        /// 添加链接
        /// </summary>
        public abstract string Link_AddSiteLink { get; }

        /// <summary>
        /// 更新链接
        /// </summary>
        public abstract string Link_UpdateSiteLink { get; }

        /// <summary>
        /// 删除链接
        /// </summary>
        public readonly string Link_DeleteSiteLink = "DELETE FROM $PREFIX_link WHERE id=@LinkId AND site_id=@siteId";

        #endregion

        #region 消息相关

        /// <summary>
        /// 获取信息
        /// </summary>
        public readonly string Message_GetMessage = "SELECT * FROM $PREFIX_Message WHERE [ID]=@Id";

        /// <summary>
        /// 写入新消息
        /// </summary>
        public readonly string Mesage_InsertMessage = @"INSERT INTO $PREFIX_Message ([SendUID],[ReceiveUID],[Subject],[Content],[HasRead],[Recycle],[SendDate])
                                                        VALUES(@SendUID,@ReceiveUID,@Subject,@Content,@HasRead,@Recycle,@SendDate)";

        /// <summary>
        /// 设置消息为已读
        /// </summary>
        public readonly string Message_SetRead = "UPDATE $PREFIX_Message SET [HasRead]=1 WHERE [receiveUID]=@ReceiveUID AND [ID]=@id";

        /// <summary>
        /// 将消息设为回收
        /// </summary>
        public readonly string Message_SetRecycle = "UPDATE $PREFIX_Message SET [Recycle]=1 WHERE [receiveUID]=@ReceiveUID AND [ID]=@id";

        /// <summary>
        /// 删除消息
        /// </summary>
        public readonly string Message_Delete = "DELETE FROM $PREFIX_Message WHERE [receiveUID]=@ReceiveUID AND [ID]=@id";

        /// <summary>
        /// 获取分页消息条数
        /// {0}->$[condition]
        /// </summary>
        public readonly string Message_GetPagedMessagesCount = "SELECT COUNT([ID]) FROM $PREFIX_Message WHERE Recycle=0 AND {0}";

        /// <summary>
        /// 获取分页消息
        /// </summary>
        public abstract string Message_GetPagedMessages { get; }

        #endregion

        #region 会员相关

        /// <summary>
        /// 检测用户名是否存在
        /// </summary>
        public readonly string Member_DetectUsernameAndNickNameHasExits = "SELECT id FROM $PREFIX_member WHERE username=@Username OR nickname=@Nickname";
        /// <summary>
        /// 注册会员
        /// </summary>
        public abstract string Member_RegisterMember { get; }
        /// <summary>
        /// 写入会员的详细信息
        /// </summary>
        public readonly string Member_InsertMemberDetails = "INSERT INTO $PREFIX_memberdetails values(@UID,@Status,@RegIP,@RegTime,@LastLoginTime,@Token)";
        /// <summary>
        /// 根据会员ID获取会员信息及详细信息
        /// </summary>
        public readonly string Member_GetMemberInfoAndDetails = "SELECT * FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails ON $PREFIX_member.id=$PREFIX_memberdetails.uid WHERE id=@id OR username=@Username";


        /// <summary>
        /// 检测会员名是否存在
        /// </summary>
        public readonly string Member_DetectUsername = "SELECT id FROM $PREFIX_member WHERE username=@Username";

        /// <summary>
        /// 检测昵称是否存在
        /// </summary>
        public readonly string Member_DetectNickname = "SELECT id FROM $PREFIX_member WHERE nickname=@Nickname";

        /// <summary>
        /// 获取会员号
        /// </summary>
        public readonly string Member_GetMemberUID = "SELECT id FROM $PREFIX_member WHERE username=@Username";


        /// <summary>
        /// 验证会员名和密码是否匹配，匹配则返回会员信息
        /// </summary>
        public readonly string Member_VerifyMember = "SELECT * FROM $PREFIX_member WHERE username=@UserName AND password=@password";


        /// <summary>
        /// 根据会员ID获取会员信息
        /// </summary>
        public readonly string Member_GetMemberByID = "SELECT * FROM $PREFIX_member WHERE id=@id";

        /// <summary>
        /// 根据会员名获取会员信息
        /// </summary>
        public readonly string Member_GetMemberByUsername = "SELECT * FROM $PREFIX_member WHERE username=@Username";

        /// <summary>
        /// 会员资料更新
        /// </summary>
        public abstract string Member_Update { get; }

        /// <summary>
        /// 删除会员
        /// </summary>
        public string Member_DeleteMember = "DELETE FROM $PREFIX_member where ID=@id";

        /// <summary>
        /// 删除会员详细信息
        /// </summary>
        public string Member_DeleteMemberDetails = "DELETE FROM $PREFIX_memberdetails WHERE uid=@id";

        /// <summary>
        /// 获取会员数
        /// </summary>
        public string Member_GetMemberCount = "SELECT COUNT(0) FROM $PREFIX_member INNER JOIN $PREFIX_memberdetails ON id=$PREFIX_memberdetails.uid";

        /// <summary>
        /// 获取分页会员列表
        /// </summary>
        public abstract string Member_GetPagedMembers { get; }

        #endregion

        #region 点评相关

        /// <summary>
        /// 创建点评数据
        /// </summary>
        public readonly string Reviews_Create = "INSERT INTO $PREFIX_review VALUES(@id,'')";

        /// <summary>
        /// 获取参与点评的会员数
        /// </summary>
        public readonly string Reviews_GetMember = "SELECT members FROM $PREFIX_review WHERE id=@id";

        /// <summary>
        /// 更新同意点评
        /// </summary>
        public string Reviews_UpdateEvaluate_Agree = "Update $PREFIX_archive set Agree=agree+1 where id=@id";

        /// <summary>
        /// 更新不同意点评
        /// </summary>
        public string Reviews_UpdateEvaluate_Disagree = "UPDATE $PREFIX_archive SET disagree=disagree+1 WHERE id=@id";

        /// <summary>
        /// 更新点评
        /// </summary>
        public string Reviews_UpdateReviews = "UPDATE $PREFIX_review SET members=@Members WHERE id=@id";

        /// <summary>
        /// 删除点评
        /// </summary>
        public string Reviews_Delete = "DELETE FROM $PREFIX_review WHERE id=@id";

        #endregion

        #region 模块相关
        public readonly string Module_Add = "INSERT INTO $PREFIX_modules(site_id,name,isSystem,isDelete) VALUES(@siteId,@name,@isSystem,@isDelete)";

        // public readonly string Module_Delete = "UPDATE $PREFIX_Modules SET isDelete=1 WHERE id=@id";
        public readonly string Module_Delete = "DELETE FROM $PREFIX_modules WHERE id=@id";

        public readonly string Module_Update = "UPDATE $PREFIX_modules SET name=@name, isDelete=@isDelete WHERE id=@id";
        
        public readonly string Module_GetAll = "SELECT * FROM  $PREFIX_modules";
        public readonly string Module_GetByID = "SELECT * FROM  $PREFIX_modules WHERE id=@id";
        public readonly string Module_GetByName = "SELECT * FROM  $PREFIX_modules WHERE name=@name";     
        #endregion

        #region 扩展相关

        public readonly string DataExtend_CreateField = @"
                INSERT INTO $PREFIX_extend_field(site_id,name,type,default_value,regex,message)
                VALUES(@siteId,@name,@type,@defaultValue,@regex,@message)";


        public readonly string DataExtend_DeleteExtendField =
                @"DELETE FROM $PREFIX_extend_field WHERE site_id=@siteId AND id=@id";

        /// <summary>
        /// 获取分类扩展属性绑定次数
        /// </summary>
        public readonly string DataExtend_GetCategoryExtendRefrenceNum = @"
                SELECT Count(0) FROM $PREFIX_extend_value v
                INNER JOIN $PREFIX_archive a ON v.relation_id=a.id
                INNER JOIN $PREFIX_category c ON c.id=a.cat_id
                AND v.relation_type=1 AND c.site_id=@siteId AND a.cat_id=@categoryId AND v.field_id=@fieldId";


        public readonly string DataExtend_UpdateField = @"
                UPDATE $PREFIX_extend_field SET name=@name,type=@type,regex=@regex,
                default_value=@defaultValue,message=@message WHERE id=@id AND site_id=@siteId";




        /// <summary>
        /// 获取所有的扩展字段
        /// </summary>
        public readonly string DataExtend_GetAllExtends = @"SELECT * FROM $PREFIX_extend_field";


        public readonly string DataExtend_GetExtendFieldByName =
            "SELECT * FROM $PREFIX_extend_field WHERE site_id=@siteId AND name=@name AND type=@type";

        /// <summary>
        /// 获取相关联的数据
        /// </summary>
        public readonly string DataExtend_GetExtendValues = @"
            SELECT v.id as id,relation_id,field_id,f.name as fieldName,field_value
	        FROM $PREFIX_extend_value v INNER JOIN $PREFIX_extend_field f ON v.field_id=f.id
	        WHERE relation_id=@relationId AND f.site_id=@siteId AND relation_type=@relationType";

        /// <summary>
        /// 获取相关联的数据
        /// </summary>
        public readonly string DataExtend_GetExtendValuesList = @"
            SELECT v.id as id,relation_id,field_id,f.name as fieldName,field_value
	        FROM $PREFIX_extend_value v INNER JOIN $PREFIX_extend_field f ON v.field_id=f.id
	        WHERE  relation_type=@relationType AND f.site_id=@siteId AND relation_id IN ({0})";


        public readonly string DataExtend_ClearupExtendFielValue = @"
                DELETE FROM $PREFIX_extend_value WHERE /*fieldId=@fieldId AND*/
                relation_id=@relationId AND relation_type=@relationType;";

        /// <summary>
        /// 修改扩展字段值
        /// </summary>
        //public readonly string DataExtend_UpdateFieldValue = "UPDATE $PREFIX_dataExtendField SET attrVal=@attrVal WHERE attrId=@attrId AND rid=@rid";
        public readonly string DataExtend_InsertOrUpdateFieldValue = @"
                    INSERT INTO $PREFIX_extend_value
                    (relation_id,field_id,field_value,relation_type)
                    VALUES (@relationId,@fieldId,@fieldValue,@relationType)
                ";

        /// <summary>
        /// 获取栏目的扩展属性
        /// </summary>
        public readonly string DataExtend_GetCategoryExtendIdList = @"
                SELECT extend.extend_id FROM $PREFIX_category_extend extend
                INNER JOIN $PREFIX_category c ON c.id=extend.category_id
                WHERE c.site_id=@siteId AND c.id=@categoryId
                ";
        #endregion

        #region 模板绑定

        public readonly string TplBind_Add = "INSERT INTO $PREFIX_tpl_bind (bind_id,bind_type,tpl_path) VALUES(@bindID,@bindType,@tplPath)";
        public readonly string TplBind_Update = "UPDATE $PREFIX_tpl_bind SET tpl_path=@tplPath WHERE bind_id=@bindID AND bind_type=@bindType";
        public readonly string TplBind_GetBind = "SELECT * FROM $PREFIX_tpl_bind WHERE bind_id=@bindID AND bind_type=@bindType";
        public readonly string TplBind_CheckExists = "SELECT count(0) FROM $PREFIX_tpl_bind WHERE bind_id=@bindID AND bind_type=@bindType";
        public readonly string TplBind_RemoveBind = "DELETE FROM $PREFIX_tpl_bind WHERE bind_id=@bindID AND bind_type=@bindType";
        public readonly string TplBind_GetBindList = "SELECT * FROM $PREFIX_tpl_bind";

        /// <summary>
        /// 删除未关联的栏目模版
        /// </summary>
        public readonly string TplBind_RemoveErrorCategoryBind = "DELETE FROM $PREFIX_tpl_bind WHERE bind_id NOT IN (SELECT id FROM $PREFIX_category) AND bind_type IN(3,4)";

        #endregion

        #region 表格

        /// <summary>
        /// 检查表格的名称是否存在
        /// </summary>
        public readonly string Table_GetTableIDByName = "SELECT id FROM $PREFIX_table WHERE name=@name";

        /// <summary>
        /// 添加表格
        /// </summary>
        public  readonly string Table_Add = "INSERT INTO $PREFIX_table (name,note,api_server,is_system,enabled) VALUES(@name,@note,@apiServer,@isSystem,@enabled)";

        /// <summary>
        /// 删除表格
        /// </summary>
        public readonly string Table_DeleteTable = "DELETE FROM $PREFIX_table WHERE id=@tableId";

        /// <summary>
        /// 添加表格列
        /// </summary>
        public readonly string Table_CreateColumn = "INSERT INTO $PREFIX_table_column (table_id,name,note,valid_format,sort_number) VALUES(@tableId,@name,@note,@validFormat,@sortNumber)";

        /// <summary>
        /// 更新表格列
        /// </summary>
        public readonly string Table_UpdateColumn = "UPDATE $PREFIX_table_column SET name=@name,note=@note,valid_format=@validFormat,sort_number=@sortNumber WHERE id=@columnId";

        /// <summary>
        /// 获取列
        /// </summary>
        public readonly string Table_GetColumn = "SELECT * FROM $PREFIX_table_column WHERE id=@columnId";


        /// <summary>
        /// 删除列
        /// </summary>
        public readonly string Table_DeleteColumn = "DELETE FROM $PREFIX_table_column WHERE table_id=@tableId AND id=@columnId";

        /// <summary>
        /// 删除所有列
        /// </summary>
        public readonly string Table_DeleteColumns = "DELETE FROM $PREFIX_table_column WHERE table_id=@tableId";

        /// <summary>
        /// 获取表格记录
        /// </summary>
        public readonly string Table_GetRowsCount = "SELECT count(0) FROM $PREFIX_table_row WHERE table_id=@tableId";


        /// <summary>
        /// 修改表格
        /// </summary>
        public readonly string Table_Update = "UPDATE $PREFIX_table set name=@name,note=@note,api_server=@apiServer,is_system=@isSystem,enabled=@enabled WHERE id=@tableId";

        /// <summary>
        /// 获取表格
        /// </summary>
        public readonly string Table_GetTableById = "SELECT * FROM $PREFIX_table WHERE id=@tableId";

        /// <summary>
        /// 获取最小的表格编号
        /// </summary>
        public readonly string Table_GetMinTableId = "SELECT min(id) FROM $PREFIX_table";

        /// <summary>
        /// 是否存在系统表格
        /// </summary>
        public readonly string Table_HasExistsSystemTale = "SELECT count(0) FROM $PREFIX_table WHERE id=@tableId AND isSystem";

        /// <summary>
        /// 获取所有表格
        /// </summary>
        public readonly string Table_GetTables = "SELECT * FROM $PREFIX_table";

        /// <summary>
        /// 获取所有列根据表格编号
        /// </summary>
        public readonly string TableGetColumnsByTableId = "SELECT * FROM $PREFIX_table_column WHERE table_id=@tableId ORDER BY sort_number ASC";

        /// <summary>
        /// 创建行
        /// </summary>
        public readonly string Table_CreateRow = "INSERT INTO $PREFIX_table_row (table_id,submit_time) VALUES(@tableId,@submitTime)";

        /// <summary>
        /// 删除行
        /// </summary>
        public readonly string Table_DeleteRow = "DELETE FROM $PREFIX_table_row WHERE id=@rowId AND table_id=@tableId";

        /// <summary>
        /// 获取行
        /// </summary>
        public readonly string Table_GetRow = "SELECT * FROM $PREFIX_table_row WHERE id=@rowId";

        /// <summary>
        /// 获取行数据
        /// </summary>
        public readonly string table_GetRowData = "SELECT * FROM $PREFIX_table_record WHERE row_id IN ($[range])";


        /// <summary>
        /// 清理删除的列在数据行中的数据
        /// </summary>
        public readonly string Table_ClearDeletedColumnData = "DELETE FROM $PREFIX_table_record WHERE row_id IN (SELECT id FROM $PREFIX_table_row WHERE table_id=@tableId) AND col_id=@columnId";


        /// <summary>
        /// 清理删除的行在数据行中的数据
        /// </summary>
        public readonly string Table_ClearDeletedRowData = "DELETE FROM $PREFIX_table_record WHERE row_id = (SELECT id FROM $PREFIX_table_row WHERE table_id=@tableId AND id=@rowId)";


      
    




        /// <summary>
        /// 获取最后创建的行号
        /// </summary>
        public abstract string TableGetLastedRowId { get; }

        /// <summary>
        /// 插入数据
        /// </summary>
        public abstract string TableInsertRowData { get; }

        /// <summary>
        /// 获取分页行
        /// </summary>
        public abstract string TableGetPagedRows { get; }
       
        #endregion

        #region 站点

        /// <summary>
        /// 创建站点
        /// </summary>
        public readonly string SiteCreateSite = @"INSERT INTO $PREFIX_site(name,app_name,domain,location,tpl,language,
                                    note,seo_title,seo_keywords,seo_description,state,pro_tel,pro_phone,pro_fax,pro_address,
                                    pro_email,pro_im,pro_post,pro_notice,pro_slogan)VALUES
                                    (@name,@appName,@domain,@location,@tpl,@language,@note,@seoTitle,@seoKeywords,@seoDescription,@state,
                                    @proTel,@proPhone,@proFax,@proAddress,@proEmail,@proIm,@proPost,@proNotice,@proSlogan)";
       
        /// <summary>
        /// 获取所有站点
        /// </summary>
        public readonly string SiteGetSites = "SELECT * FROM $PREFIX_site ORDER BY site_id ASC";

        /// <summary>
        /// 更新站点
        /// </summary>
        public readonly string SiteEditSite = @"UPDATE $PREFIX_site SET name=@name,app_name=@appName,
                                        domain=@domain,location=@location,tpl=@tpl,
                                        language=@language,note=@note,seo_title=@seoTitle,seo_keywords=@seoKeywords,
                                        seo_description=@seoDescription,state=@state,pro_tel=@proTel,pro_phone=@proPhone,
                                        pro_fax=@proFax,pro_address=@proAddress,pro_email=@proEmail,pro_im=@proIm,
                                        pro_post=@proPost,pro_notice=@proNotice,pro_slogan=@proSlogan WHERE site_id=@siteId";


        #endregion
        public readonly String Link_RemoveRelatedLinks = @"DELETE FROM $PREFIX_related_link
                        WHERE content_type = @contentType AND content_id = @contentId
                        AND id in ({0})
                        ";

        public readonly string Link_GetRelatedLinks =@"SELECT * FROM $PREFIX_related_link
                        WHERE content_type = @contentType AND content_id = @contentId";

        public readonly String Link_InsertRelatedLink = @"
                        INSERT INTO $PREFIX_related_link(content_type,content_id,related_site_id,related_indent,related_content_id,enabled)
                        VALUES (@contentType,@contentId,@relatedSiteId,@relatedIndent,@relatedContentId,@enabled)";

        public readonly String Link_UpdateRelatedLink= @"UPDATE $PREFIX_related_link
	                    SET related_site_id=@relatedSiteId,related_indent=@relatedIndent,related_content_id=@relatedContentId,enabled = @enabled
	                    WHERE id=@id AND content_type = @contentType AND content_id = @contentId";
    }
}