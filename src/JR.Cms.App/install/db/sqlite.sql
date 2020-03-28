DROP TABLE IF EXISTS "cms_archive";
CREATE TABLE "cms_archive" ("id" integer PRIMARY KEY  NOT NULL ,"str_id" varchar(16) NOT NULL  DEFAULT (null) ,"alias" varchar(50),"cid" integer DEFAULT (0) ,"author_id" integer DEFAULT (null) ,"title" varchar(100),"flags" varchar(100),"tags" varchar(100),"outline" varchar(255),"content" memo,"thumbnail" varchar(150),"view_count" integer DEFAULT (0) ,"agree" integer,"disagree" integer,"create_time" datetime,"lastModifyDate" datetime,"source" nvarchar(50),"location" nvarchar(150),"small_title" varchar(100),"sort_number" INTEGER NOT NULL  DEFAULT (0) );

DROP TABLE IF EXISTS "cms_category";
CREATE TABLE "cms_category" ("id" integer PRIMARY KEY  NOT NULL  DEFAULT (null) ,"site_id" integer NOT NULL  DEFAULT (1) ,"page_title" varchar(150) DEFAULT ('0') ,"module_id" integer DEFAULT (0) ,"tag" varchar(50) DEFAULT ('null') ,"name" varchar(80) DEFAULT ('null') ,"page_keywords" varchar(100) DEFAULT ('null') ,"page_description" varchar(200) DEFAULT ('null') ,"lft" integer DEFAULT (0) ,"rgt" integer DEFAULT (0) ,"sort_number" integer DEFAULT (0) ,"location" nvarchar(150),"icon" nvarchar(150));
INSERT INTO "cms_category" VALUES (1,1,"-","1","root","根栏目","-","-",1,4,"1",null,null);
INSERT INTO "cms_category" VALUES (2,1,"-","1","cms","文章文类1","-","-",2,3,"1",null,null);

DROP TABLE IF EXISTS "cms_category_extend";
CREATE TABLE "cms_category_extend" ("id" integer PRIMARY KEY  NOT NULL ,"category_id" integer NOT NULL  DEFAULT (null) ,"extend_id" int NOT NULL  DEFAULT (null) ,"enabled" bit NOT NULL );

DROP TABLE IF EXISTS "cms_extend_field";
CREATE TABLE "cms_extend_field" ("id" integer PRIMARY KEY  NOT NULL ,"site_id" integer NOT NULL  DEFAULT (null) ,"name" nvarchar(50) NOT NULL ,"type" nvarchar(100) NOT NULL ,"default_value" nvarchar(50) DEFAULT (null) ,"regex" nvarchar(50),"message" nvarchar(50));

DROP TABLE IF EXISTS "cms_extend_value";
CREATE TABLE "cms_extend_value" ("id" integer PRIMARY KEY  NOT NULL ,"relation_id" integer DEFAULT (null) ,"field_id" integer DEFAULT (null) ,"field_value" varchar(500) DEFAULT (null) ,"relation_type" int NOT NULL  DEFAULT (1) );

DROP TABLE IF EXISTS "cms_comment";
CREATE TABLE "cms_comment" ("ID" integer  Primary Key NOT NULL ,"archiveID" varchar (16),"memberID" integer,"ip" varchar (20),"content" memo,"recycle" boolean DEFAULT (0) ,"createdate" datetime);

DROP TABLE IF EXISTS "cms_credential";
CREATE TABLE "cms_credential" ("id" INTEGER PRIMARY KEY  NOT NULL ,"user_id" INTEGER DEFAULT (0) ,"user_name" varchar(50) DEFAULT (null) ,"password" VARCHAR(50) DEFAULT (null) ,"enabled" INTEGER);

DROP TABLE IF EXISTS "cms_link";
CREATE TABLE "cms_link" ("id" integer PRIMARY KEY  DEFAULT (null) ,"site_id" INTEGER NOT NULL  DEFAULT (1) ,"type" integer,"text" varchar(50),"uri" varchar(255),"target" varchar(20) DEFAULT (null) ,"sort_number" integer DEFAULT (null) ,"visible" boolean DEFAULT (0) ,"pid" INTEGER,"img_url" varchar(100) DEFAULT (null) ,"bind" varchar(20));

DROP TABLE IF EXISTS "cms_log";
CREATE TABLE "cms_log" 
(
	[ID] varchar (255) NOT NULL , 
	[TypeID] integer, 
	[Description] varchar (255), 
	[Content] memo, 
	[HelpLink] varchar (255), 
	[RecordDate] datetime DEFAULT 'Date()+Time()'
);

DROP TABLE IF EXISTS "cms_member";
CREATE TABLE "cms_member" ("id" integer PRIMARY KEY  NOT NULL ,"Username" varchar (20),"Password" varchar (32) DEFAULT (null) ,"Avatar" varchar (255),"sex" varchar (7),"nickname" varchar (15),"Email" varchar (50),"TelPhone" varchar (20),"Note" varchar (255),"UserGroupID" integer);

DROP TABLE IF EXISTS "cms_memberdetails";
CREATE TABLE "cms_memberdetails" ("uid" integer NOT NULL ,"Status" varchar (10),"RegIp" varchar (15),"RegTime" datetime,"LastLoginTime" datetime,"token" varchar (30) DEFAULT (null) );

DROP TABLE IF EXISTS "cms_message";
CREATE TABLE "cms_message" ("ID" integer  Primary Key NOT NULL ,"SendUID" integer,"ReceiveUID" integer,"Subject" varchar (50),"Content" varchar (255),"HasRead" boolean DEFAULT (0) ,"Recycle" boolean DEFAULT (0) ,"SendDate" datetime);

DROP TABLE IF EXISTS "cms_modules";
CREATE TABLE "cms_modules" ("id" integer PRIMARY KEY  NOT NULL ,"name" nvarchar (50),"issystem" bit NOT NULL  DEFAULT (0) ,"isdelete" bit DEFAULT (0) , "siteid" INTEGER DEFAULT 0, "extid1" INTEGER, "extid2" INTEGER, "extid3" INTEGER, "extid4" INTEGER);

DROP TABLE IF EXISTS "cms_related_link";
CREATE TABLE "cms_related_link" ("id" integer PRIMARY KEY  NOT NULL ,"content_type" varchar(10) NOT NULL  DEFAULT (null) ,"content_id" int NOT NULL  DEFAULT (null) , "related_site_id" int ,"related_indent" int DEFAULT (null) ,"related_content_id" int NOT NULL  DEFAULT (null) ,"enabled" int NOT NULL  DEFAULT (null));

DROP TABLE IF EXISTS "cms_review";
CREATE TABLE "cms_review" ("ID" varchar (16) NOT NULL  DEFAULT (null) ,"Members" memo);

DROP TABLE IF EXISTS "cms_site";
CREATE TABLE "cms_site" ("site_id" integer PRIMARY KEY  NOT NULL  DEFAULT (null) ,"name" varchar(50) NOT NULL ,"app_name" varchar(50) DEFAULT (null) ,"domain" varchar(50) NOT NULL ,"language" integer NOT NULL ,"tpl" varchar(100),"note" varchar(200),"seo_title" varchar(200) DEFAULT (null) ,"seo_keywords" varchar(200) DEFAULT (null) ,"seo_description" varchar(200) DEFAULT (null) ,"state" integer NOT NULL  DEFAULT (1) ,"pro_tel" varchar(50) DEFAULT (null) ,"pro_phone" varchar(11) DEFAULT (null) ,"pro_fax" varchar(50) DEFAULT (null) ,"pro_address" varchar(100) DEFAULT (null) ,"pro_email" varchar(100) DEFAULT (null) ,"pro_im" varchar(100) DEFAULT (null) ,"pro_post" varchar(100) DEFAULT (null) ,"pro_notice" varchar(250) DEFAULT (null) ,"pro_slogan" varchar(250) DEFAULT (null) ,"location" VARCHAR);
INSERT INTO "cms_site" VALUES(1,'默认站点','','',2,'default','','b','sf','p',1,'123','12','2','22222','2222','2','','n','222222','');

DROP TABLE IF EXISTS "cms_table";
CREATE TABLE "cms_table" ("id" INTEGER PRIMARY KEY  NOT NULL ,"name" VARCHAR(50),"note" VARCHAR(500),"api_server" VARCHAR(200) DEFAULT (null) ,"is_system" boolean DEFAULT (null) ,"enabled" boolean DEFAULT (1) );

DROP TABLE IF EXISTS "cms_table_column";
CREATE TABLE "cms_table_column" ("id" INTEGER PRIMARY KEY  NOT NULL ,"table_id" INTEGER DEFAULT (null) ,"name" VARCHAR(20),"note" VARCHAR(50),"valid_format" VARCHAR(200) DEFAULT (0) ,"sort_number" INTEGER DEFAULT (null) );

DROP TABLE IF EXISTS "cms_table_record";
CREATE TABLE "cms_table_record" ("id" INTEGER PRIMARY KEY  NOT NULL ,"row_id" INTEGER NOT NULL  DEFAULT (null) ,"col_id" INTEGER NOT NULL  DEFAULT (null) ,"value" VARCHAR(1000) NOT NULL );

DROP TABLE IF EXISTS "cms_table_row";
CREATE TABLE "cms_table_row" ("id" INTEGER PRIMARY KEY  NOT NULL ,"table_id" INTEGER NOT NULL  DEFAULT (null) ,"submit_time" DATETIME DEFAULT (null) );

DROP TABLE IF EXISTS "cms_tpl_bind";
CREATE TABLE "cms_tpl_bind" ("id" INTEGER PRIMARY KEY  NOT NULL ,"bind_id" int NOT NULL  DEFAULT (null) ,"bind_type" INTEGER NOT NULL  DEFAULT (null) ,"tpl_path" varchar(200) DEFAULT (null) );

DROP TABLE IF EXISTS "cms_user";
CREATE TABLE "cms_user" ("id" INTEGER PRIMARY KEY  NOT NULL ,"name" varchar(50),"avatar" varchar(100) DEFAULT (null) ,"phone" VARCHAR(20),"email" VARCHAR(50),"check_code" varchar(10) DEFAULT (null) ,"create_time" DATETIME DEFAULT (null) ,"last_login_time" DATETIME,"flag" INTEGER DEFAULT (null) );

DROP TABLE IF EXISTS "cms_user_role";
CREATE TABLE "cms_user_role" ("id" INTEGER PRIMARY KEY  NOT NULL ,"user_id" INTEGER DEFAULT (null) ,"app_id" INTEGER,"flag" INTEGER DEFAULT (null) );

DROP TABLE IF EXISTS "cms_usergroup";
CREATE TABLE "cms_usergroup" ("id" integer PRIMARY KEY  NOT NULL  DEFAULT (null) ,"name" varchar (50) DEFAULT (null) ,"permission" varchar (255) DEFAULT (null) ,"is_master" INTEGER DEFAULT (0) ,"is_super" INTEGER DEFAULT (0) ,"enabled" INTEGER DEFAULT (1) );
INSERT INTO "cms_usergroup" VALUES(1,'超级管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42',0,0,1);
INSERT INTO "cms_usergroup" VALUES(2,'管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,40,41,42',0,0,1);
INSERT INTO "cms_usergroup" VALUES(3,'编辑','1,2,3,4,5,6,10,11,12,13,14,15',0,0,1);
INSERT INTO "cms_usergroup" VALUES(4,'会员','1,2,3,4,5,6',0,0,1);
INSERT INTO "cms_usergroup" VALUES(5,'游客','3,4',0,0,1);

DROP TABLE IF EXISTS "cms_operation";
CREATE TABLE "cms_operation" ("ID" integer PRIMARY KEY ,"Name" varchar (255),"Path" varchar (255),"Available" boolean DEFAULT (0) );
INSERT INTO "cms_operation" VALUES(1,'友情链接','link?view=list&type=friendlink',1);
INSERT INTO "cms_operation" VALUES(2,'分类->分类列表','catalog?view=list',1);
INSERT INTO "cms_operation" VALUES(3,'系统->网站设置','config?id=1s',1);
INSERT INTO "cms_operation" VALUES(4,'系统->网站基本资料修改','config?id=2',1);
INSERT INTO "cms_operation" VALUES(5,'系统->网站优化设置','config?id=3',1);
INSERT INTO "cms_operation" VALUES(6,'错误日志','system?view=errorlog',1);
INSERT INTO "cms_operation" VALUES(7,'内容->页面管理','archive?view=list&type=1',1);
INSERT INTO "cms_operation" VALUES(8,'内容->添加页面','archive?view=create&type=1',1);
INSERT INTO "cms_operation" VALUES(9,'内容->信息列表','archive?view=list&type=2',1);
INSERT INTO "cms_operation" VALUES(10,'内容->发布信息','archive?view=create&type=2',1);
INSERT INTO "cms_operation" VALUES(11,'分类->删除分类','app.axd?do=catalog:delete',1);
INSERT INTO "cms_operation" VALUES(12,'分类->修改分类','app.axd?do=catalog:update',1);
INSERT INTO "cms_operation" VALUES(13,'会员>会员列表','user?view=member',1);
INSERT INTO "cms_operation" VALUES(14,'会员>删除会员','app.axd?do=member:delete',1);
INSERT INTO "cms_operation" VALUES(15,'系统用户管理','user?view=user',1);
INSERT INTO "cms_operation" VALUES(16,'删除评论','app.axd?do=archive:deletecomment',1);
INSERT INTO "cms_operation" VALUES(17,'头部导航链接','link?view=list&type=headerlink',1);
INSERT INTO "cms_operation" VALUES(18,'网站导航链接','link?view=list&type=navigation',1);
INSERT INTO "cms_operation" VALUES(19,'添加新链接','link?view=create',1);
INSERT INTO "cms_operation" VALUES(20,'修改链接','link?view=edit',1);
INSERT INTO "cms_operation" VALUES(21,'删除链接','app.axd?do=link:delete',1);
INSERT INTO "cms_operation" VALUES(22,'更新页面','archive?view=update&typeid=1',1);
INSERT INTO "cms_operation" VALUES(23,'更新信息','archive?view=update&typeid=2',1);
INSERT INTO "cms_operation" VALUES(24,'更新图文信息','archive?view=update&typeid=3',1);
INSERT INTO "cms_operation" VALUES(25,'更新画廊信息','archive?view=update&typeid=4',1);
INSERT INTO "cms_operation" VALUES(26,'更新视频信息','archive?view=update&typeid=5',1);
INSERT INTO "cms_operation" VALUES(27,'更新专题信息','archive?view=update&typeid=6',1);
INSERT INTO "cms_operation" VALUES(28,'图文信息列表','archive?view=list&typeid=3',1);
INSERT INTO "cms_operation" VALUES(29,'画廊息列表','archive?view=list&typeid=4',1);
INSERT INTO "cms_operation" VALUES(30,'视频信息列表','archive?view=list&typeid=5',1);
INSERT INTO "cms_operation" VALUES(31,'专题列表','archive?view=list&typeid=6',1);
INSERT INTO "cms_operation" VALUES(32,'发布图文信息','archive?view=create&typeid=3',1);
INSERT INTO "cms_operation" VALUES(33,'发布画廊信息','archive?view=create&typeid=4',1);
INSERT INTO "cms_operation" VALUES(34,'发布视频信息','archive?view=create&typeid=5',1);
INSERT INTO "cms_operation" VALUES(35,'创建新专题','archive?view=create&typeid=6',1);
INSERT INTO "cms_operation" VALUES(36,'清除缓存','system?view=clearcache',1);
INSERT INTO "cms_operation" VALUES(37,'操作列表','operation?view=list',1);
INSERT INTO "cms_operation" VALUES(38,'用户组操作权限设置','operation?view=set',1);
INSERT INTO "cms_operation" VALUES(39,'清除错误日志','/app.axd?log:clearErrorLog',1);
INSERT INTO "cms_operation" VALUES(40,'删除文档','archive:delete',1);
INSERT INTO "cms_operation" VALUES(41,'刷新文档创建时间','archive:refresh',1);
INSERT INTO "cms_operation" VALUES(42,'采集插件','/plugin/collection.ashx',1);
