if exists (select * from sysobjects where id = OBJECT_ID('[cms_archives]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_archives]

CREATE TABLE [cms_archives] (
[aid] [int]  IDENTITY (1, 1)  NOT NULL,
[id] [nvarchar]  (48) NOT NULL,
[alias] [nvarchar]  (150) NULL,
[cid] [int]  NOT NULL,
[author] [nvarchar]  (150) NULL,
[title] [nvarchar]  (300) NULL,
[source] [nvarchar]  (150) NULL,
[tags] [nvarchar]  (300) NULL,
[outline] [nvarchar]  (765) NULL,
[content] [ntext]  NULL,
[properties] [nvarchar]  (1500) NULL,
[viewcount] [int]  NULL DEFAULT (0),
[agree] [int]  NULL,
[disagree] [int]  NULL,
[createdate] [datetime]  NULL,
[lastmodifydate] [datetime]  NULL,
[flags] [nvarchar]  (300) NULL DEFAULT ('{st:''''0'''',sc:''''0'''',v:''''1'''',p:''''0''''}'),
[thumbnail] [nvarchar]  (450) NULL)

ALTER TABLE [cms_archives] WITH NOCHECK ADD  CONSTRAINT [PK_cms_archives] PRIMARY KEY  NONCLUSTERED ( [aid],[id],[alias],[cid] )
SET IDENTITY_INSERT [cms_archives] ON

INSERT [cms_archives] ([aid],[id],[alias],[cid],[author],[title],[content],[properties],[viewcount],[agree],[disagree],[createdate],[lastmodifydate],[flags]) VALUES ( 1,N'spcnet',N'welcome',2,N'admin',N'欢迎使用Special Cms .NET',N'<div style="text-align:center;font-size:30px"><h2>??￠è??????”¨Special Cms .NET!</h2></div>',N'{}',1,0,0,N'2013/1/1 1:01:01',N'2013/1/1 1:01:01',N'{st:''0'',sc:''0'',v:''1'',p:''0''}')

SET IDENTITY_INSERT [cms_archives] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_categories]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_categories]

CREATE TABLE [cms_categories] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[siteid] [int]  NULL DEFAULT (1),
[moduleid] [int]  NOT NULL,
[tag] [nvarchar]  (300) NULL,
[name] [nvarchar]  (300) NULL,
[lft] [int]  NULL,
[rgt] [int]  NULL,
[pagetitle] [nvarchar]  (600) NULL,
[keywords] [nvarchar]  (600) NULL,
[description] [nvarchar]  (750) NULL,
[orderindex] [int]  NULL DEFAULT (0))

ALTER TABLE [cms_categories] WITH NOCHECK ADD  CONSTRAINT [PK_cms_categories] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_categories] ON

INSERT [cms_categories] ([id],[siteid],[moduleid],[tag],[name],[lft],[rgt],[orderindex]) VALUES ( 1,0,1,N'root',N'根栏目',1,4,0)
INSERT [cms_categories] ([id],[siteid],[moduleid],[tag],[name],[lft],[rgt],[orderindex]) VALUES ( 2,1,1,N'cms',N'欢迎使用',2,3,0)

SET IDENTITY_INSERT [cms_categories] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_comments]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_comments]

CREATE TABLE [cms_comments] (
[id] [int]  NOT NULL,
[archiveid] [nvarchar]  (48) NULL,
[memberid] [int]  NULL,
[ip] [nvarchar]  (60) NULL,
[content] [ntext]  NULL,
[recycle] [smallint]  NULL,
[createdate] [datetime]  NULL)

ALTER TABLE [cms_comments] WITH NOCHECK ADD  CONSTRAINT [PK_cms_comments] PRIMARY KEY  NONCLUSTERED ( [id] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_dataExtend]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_dataExtend]

CREATE TABLE [cms_dataExtend] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[name] [nvarchar]  (45) NULL,
[state] [int]  NULL DEFAULT (1))

ALTER TABLE [cms_dataExtend] WITH NOCHECK ADD  CONSTRAINT [PK_cms_dataExtend] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_dataExtend] ON


SET IDENTITY_INSERT [cms_dataExtend] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_dataExtendAttr]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_dataExtendAttr]

CREATE TABLE [cms_dataExtendAttr] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[extId] [int]  NOT NULL,
[attrName] [nvarchar]  (150) NOT NULL,
[attrType] [nvarchar]  (300) NOT NULL,
[attrVal] [nvarchar]  (60) NULL,
[regex] [nvarchar]  (300) NULL,
[attrMsg] [nvarchar]  (300) NULL,
[enabled] [int]  NULL DEFAULT (1))

ALTER TABLE [cms_dataExtendAttr] WITH NOCHECK ADD  CONSTRAINT [PK_cms_dataExtendAttr] PRIMARY KEY  NONCLUSTERED ( [id],[extId] )
SET IDENTITY_INSERT [cms_dataExtendAttr] ON


SET IDENTITY_INSERT [cms_dataExtendAttr] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_dataExtendField]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_dataExtendField]

CREATE TABLE [cms_dataExtendField] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[rid] [int]  NULL,
[extId] [int]  NULL,
[attrId] [int]  NULL,
[attrVal] [nvarchar]  (1500) NULL)

ALTER TABLE [cms_dataExtendField] WITH NOCHECK ADD  CONSTRAINT [PK_cms_dataExtendField] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_dataExtendField] ON


SET IDENTITY_INSERT [cms_dataExtendField] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_links]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_links]

CREATE TABLE [cms_links] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[pid] [int]  NULL,
[siteid] [int]  NULL DEFAULT (1),
[type] [int]  NOT NULL,
[text] [nvarchar]  (300) NOT NULL,
[uri] [nvarchar]  (765) NOT NULL,
[target] [nvarchar]  (150) NULL,
[imgurl] [nvarchar]  (300) NULL,
[bind] [nvarchar]  (60) NULL,
[index] [int]  NULL,
[visible] [smallint]  NOT NULL)

ALTER TABLE [cms_links] WITH NOCHECK ADD  CONSTRAINT [PK_cms_links] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_links] ON

INSERT [cms_links] ([id],[pid],[siteid],[type],[text],[uri],[target],[index],[visible]) VALUES ( 1,0,1,2,N'SPC.NET',N'http://www.ops.cc/cms/',N'_blank',2,1)
INSERT [cms_links] ([id],[pid],[siteid],[type],[text],[uri],[index],[visible]) VALUES ( 2,1,1,1,N'首页',N'/',1,1)
INSERT [cms_links] ([id],[pid],[siteid],[type],[text],[uri],[index],[visible]) VALUES ( 3,0,1,1,N'欢迎使用',N'/cms/welcome.html',2,1)

SET IDENTITY_INSERT [cms_links] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_logs]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_logs]

CREATE TABLE [cms_logs] (
[id] [nvarchar]  (90) NOT NULL,
[typeid] [int]  NULL,
[description] [nvarchar]  (765) NULL,
[content] [ntext]  NULL,
[helplink] [nvarchar]  (765) NULL,
[recorddate] [datetime]  NULL)

ALTER TABLE [cms_logs] WITH NOCHECK ADD  CONSTRAINT [PK_cms_logs] PRIMARY KEY  NONCLUSTERED ( [id] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_memberdetails]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_memberdetails]

CREATE TABLE [cms_memberdetails] (
[uid] [int]  NOT NULL,
[status] [nvarchar]  (30) NULL,
[regip] [nvarchar]  (45) NULL,
[regtime] [datetime]  NULL,
[lastlogintime] [datetime]  NULL,
[token] [nvarchar]  (300) NULL)

ALTER TABLE [cms_memberdetails] WITH NOCHECK ADD  CONSTRAINT [PK_cms_memberdetails] PRIMARY KEY  NONCLUSTERED ( [uid] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_members]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_members]

CREATE TABLE [cms_members] (
[id] [int]  NOT NULL,
[username] [nvarchar]  (60) NULL,
[password] [nvarchar]  (120) NULL,
[avatar] [nvarchar]  (765) NULL,
[sex] [nvarchar]  (21) NULL,
[nickname] [nvarchar]  (45) NULL,
[email] [nvarchar]  (150) NULL,
[telphone] [nvarchar]  (60) NULL,
[note] [nvarchar]  (765) NULL,
[usergroupid] [int]  NULL)

ALTER TABLE [cms_members] WITH NOCHECK ADD  CONSTRAINT [PK_cms_members] PRIMARY KEY  NONCLUSTERED ( [id] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_message]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_message]

CREATE TABLE [cms_message] (
[id] [int]  NOT NULL,
[senduid] [int]  NULL,
[receiveuid] [int]  NULL,
[subject] [nvarchar]  (150) NULL,
[content] [nvarchar]  (765) NULL,
[hasread] [smallint]  NULL,
[recycle] [smallint]  NULL,
[senddate] [datetime]  NULL)

ALTER TABLE [cms_message] WITH NOCHECK ADD  CONSTRAINT [PK_cms_message] PRIMARY KEY  NONCLUSTERED ( [id] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_modules]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_modules]

CREATE TABLE [cms_modules] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[siteid] [int]  NULL DEFAULT (1),
[name] [nvarchar]  (150) NOT NULL,
[issystem] [smallint]  NULL,
[isdelete] [smallint]  NULL,
[extid1] [int]  NULL DEFAULT (0),
[extid2] [int]  NULL DEFAULT (0),
[extid3] [int]  NULL DEFAULT (0),
[extid4] [int]  NULL DEFAULT (0))

ALTER TABLE [cms_modules] WITH NOCHECK ADD  CONSTRAINT [PK_cms_modules] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_modules] ON

INSERT [cms_modules] ([id],[siteid],[name],[issystem],[isdelete],[extid1],[extid2],[extid3],[extid4]) VALUES ( 1,0,N'自定义页面',1,0,0,0,0,0)
INSERT [cms_modules] ([id],[siteid],[name],[issystem],[isdelete],[extid1],[extid2],[extid3],[extid4]) VALUES ( 2,0,N'文档',1,0,0,0,0,0)

SET IDENTITY_INSERT [cms_modules] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_operations]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_operations]

CREATE TABLE [cms_operations] (
[id] [bigint]  IDENTITY (1, 1)  NOT NULL,
[name] [nvarchar]  (765) NULL,
[path] [nvarchar]  (765) NULL,
[available] [smallint]  NULL DEFAULT (0))

ALTER TABLE [cms_operations] WITH NOCHECK ADD  CONSTRAINT [PK_cms_operations] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_operations] ON

INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 44,N'友情链接',N'link?view=list&type=friendlink',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 45,N'分类->分类列表',N'catalog?view=list',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 46,N'系统->网站设置',N'config?id=1',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 47,N'系统->网站基本资料修改',N'config?id=2',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 48,N'系统->网站优化设置',N'config?id=3',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 49,N'错误日志',N'system?view=errorlog',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 50,N'内容->页面管理',N'archive?view=list&type=1',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 51,N'内容->添加页面',N'archive?view=create&type=1',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 52,N'内容->信息列表',N'archive?view=list&type=2',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 53,N'内容->发布信息',N'archive?view=create&type=2',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 54,N'分类->删除分类',N'app.axd?do=catalog:delete',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 55,N'分类->修改分类',N'app.axd?do=catalog:update',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 56,N'会员>会员列表',N'user?view=member',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 57,N'会员>删除会员',N'app.axd?do=member:delete',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 58,N'系统用户管理',N'user?view=user',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 59,N'删除评论',N'app.axd?do=archive:deletecomment',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 60,N'头部导航链接',N'link?view=list&type=headerlink',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 61,N'网站导航链接',N'link?view=list&type=navigation',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 62,N'添加新链接',N'link?view=create',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 63,N'修改链接',N'link?view=edit',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 64,N'删除链接',N'app.axd?do=link:delete',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 65,N'更新页面',N'archive?view=update&typeid=1',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 66,N'更新信息',N'archive?view=update&typeid=2',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 67,N'更新图文信息',N'archive?view=update&typeid=3',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 68,N'更新画廊信息',N'archive?view=update&typeid=4',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 69,N'更新视频信息',N'archive?view=update&typeid=5',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 70,N'更新专题信息',N'archive?view=update&typeid=6',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 71,N'图文信息列表',N'archive?view=list&typeid=3',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 72,N'画廊息列表',N'archive?view=list&typeid=4',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 73,N'视频信息列表',N'archive?view=list&typeid=5',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 74,N'专题列表',N'archive?view=list&typeid=6',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 75,N'发布图文信息',N'archive?view=create&typeid=3',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 76,N'发布画廊信息',N'archive?view=create&typeid=4',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 77,N'发布视频信息',N'archive?view=create&typeid=5',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 78,N'创建新专题',N'archive?view=create&typeid=6',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 79,N'清除缓存',N'system?view=clearcache',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 80,N'操作列表',N'operation?view=list',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 81,N'用户组操作权限设置',N'operation?view=set',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 82,N'清除错误日志',N'/app.axd?log:clearErrorLog',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 83,N'删除文档',N'archive:delete',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 84,N'刷新文档创建时间',N'archive:refresh',1)
INSERT [cms_operations] ([id],[name],[path],[available]) VALUES ( 85,N'内容采集',N'/plugin/collection.ashx',1)

SET IDENTITY_INSERT [cms_operations] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_reviews]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_reviews]

CREATE TABLE [cms_reviews] (
[id] [nvarchar]  (765) NOT NULL,
[members] [ntext]  NOT NULL)

ALTER TABLE [cms_reviews] WITH NOCHECK ADD  CONSTRAINT [PK_cms_reviews] PRIMARY KEY  NONCLUSTERED ( [id] )
if exists (select * from sysobjects where id = OBJECT_ID('[cms_sites]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_sites]

CREATE TABLE [cms_sites] (
[siteid] [int]  IDENTITY (1, 1)  NOT NULL,
[name] [nvarchar]  (150) NOT NULL,
[dirname] [nvarchar]  (150) NULL,
[domain] [nvarchar]  (150) NULL,
[language] [int]  NOT NULL,
[tpl] [nvarchar]  (300) NULL,
[note] [nvarchar]  (600) NULL,
[seotitle] [nvarchar]  (600) NULL,
[seokeywords] [nvarchar]  (750) NULL,
[seodescription] [nvarchar]  (750) NULL,
[state] [int]  NOT NULL,
[protel] [nvarchar]  (150) NULL,
[prophone] [nvarchar]  (33) NULL,
[profax] [nvarchar]  (150) NULL,
[proaddress] [nvarchar]  (300) NULL,
[proemail] [nvarchar]  (300) NULL,
[proqq] [nvarchar]  (300) NULL,
[promsn] [nvarchar]  (300) NULL,
[pronotice] [nvarchar]  (750) NULL,
[proslogan] [nvarchar]  (750) NULL)

ALTER TABLE [cms_sites] WITH NOCHECK ADD  CONSTRAINT [PK_cms_sites] PRIMARY KEY  NONCLUSTERED ( [siteid] )
SET IDENTITY_INSERT [cms_sites] ON

INSERT [cms_sites] ([siteid],[name],[language],[tpl],[seotitle],[state],[pronotice]) VALUES ( 1,N'默认站点',1,N'default',N'默认站点-Speicial Cms .NET!',1,N'SPC.NET是一款跨平台支持多站点基于ASP.NET MVC技术架构的内容管理系统!')

SET IDENTITY_INSERT [cms_sites] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_table]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_table]

CREATE TABLE [cms_table] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[name] [nvarchar]  (150) NOT NULL,
[note] [nvarchar]  (1500) NULL,
[apiserver] [nvarchar]  (600) NULL,
[issystem] [bit]  NOT NULL DEFAULT (0),
[available] [bit]  NOT NULL DEFAULT (0))

ALTER TABLE [cms_table] WITH NOCHECK ADD  CONSTRAINT [PK_cms_table] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_table] ON


SET IDENTITY_INSERT [cms_table] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_table_columns]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_table_columns]

CREATE TABLE [cms_table_columns] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[tableid] [int]  NOT NULL,
[name] [nvarchar]  (60) NOT NULL,
[note] [nvarchar]  (150) NULL,
[validformat] [nvarchar]  (600) NULL,
[orderindex] [int]  NOT NULL)

ALTER TABLE [cms_table_columns] WITH NOCHECK ADD  CONSTRAINT [PK_cms_table_columns] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_table_columns] ON


SET IDENTITY_INSERT [cms_table_columns] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_table_rows]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_table_rows]

CREATE TABLE [cms_table_rows] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[tableid] [int]  NOT NULL,
[submittime] [datetime]  NULL)

ALTER TABLE [cms_table_rows] WITH NOCHECK ADD  CONSTRAINT [PK_cms_table_rows] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_table_rows] ON


SET IDENTITY_INSERT [cms_table_rows] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_table_rowsdata]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_table_rowsdata]

CREATE TABLE [cms_table_rowsdata] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[rid] [int]  NOT NULL,
[cid] [int]  NOT NULL,
[value] [nvarchar]  (3000) NULL)

ALTER TABLE [cms_table_rowsdata] WITH NOCHECK ADD  CONSTRAINT [PK_cms_table_rowsdata] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_table_rowsdata] ON


SET IDENTITY_INSERT [cms_table_rowsdata] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_tplbinds]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_tplbinds]

CREATE TABLE [cms_tplbinds] (
[id] [int]  IDENTITY (1, 1)  NOT NULL,
[bindid] [nvarchar]  (60) NOT NULL,
[bindtype] [int]  NOT NULL,
[tplpath] [nvarchar]  (600) NULL)

ALTER TABLE [cms_tplbinds] WITH NOCHECK ADD  CONSTRAINT [PK_cms_tplbinds] PRIMARY KEY  NONCLUSTERED ( [id] )
SET IDENTITY_INSERT [cms_tplbinds] ON


SET IDENTITY_INSERT [cms_tplbinds] OFF
if exists (select * from sysobjects where id = OBJECT_ID('[cms_usergroups]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_usergroups]

CREATE TABLE [cms_usergroups] (
[id] [int]  NOT NULL,
[name] [nvarchar]  (150) NULL,
[permissions] [nvarchar]  (765) NULL)

ALTER TABLE [cms_usergroups] WITH NOCHECK ADD  CONSTRAINT [PK_cms_usergroups] PRIMARY KEY  NONCLUSTERED ( [id] )
INSERT [cms_usergroups] ([id]) VALUES ( 0)
INSERT [cms_usergroups] ([id],[name],[permissions]) VALUES ( 1,N'超级管理员',N'1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42')
INSERT [cms_usergroups] ([id],[name],[permissions]) VALUES ( 2,N'管理员',N'1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,40,41,42')
INSERT [cms_usergroups] ([id],[name],[permissions]) VALUES ( 3,N'编辑',N'1,2,3,4,5,6,10,11,12,13,14,15')
INSERT [cms_usergroups] ([id],[name],[permissions]) VALUES ( 4,N'会员',N'1,2,3,4,5,6')
INSERT [cms_usergroups] ([id],[name],[permissions]) VALUES ( 5,N'游客',N'3,4')
if exists (select * from sysobjects where id = OBJECT_ID('[cms_users]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) 
DROP TABLE [cms_users]

CREATE TABLE [cms_users] (
[userid] [int]  IDENTITY (1, 1)  NOT NULL,
[siteid] [int]  NOT NULL DEFAULT (1),
[username] [nvarchar]  (150) NOT NULL,
[password] [nvarchar]  (150) NULL,
[name] [nvarchar]  (150) NULL,
[groupid] [int]  NULL,
[available] [smallint]  NULL,
[createdate] [datetime]  NULL,
[lastlogindate] [datetime]  NULL)

ALTER TABLE [cms_users] WITH NOCHECK ADD  CONSTRAINT [PK_cms_users] PRIMARY KEY  NONCLUSTERED ( [userid] )
SET IDENTITY_INSERT [cms_users] ON

INSERT [cms_users] ([userid],[siteid],[username],[password],[name],[groupid],[available],[createdate],[lastlogindate]) VALUES ( 1,0,N'admin',N'285c96f7702357c07f9b5daee2660e79',N'master',1,1,N'2013/12/18 0:00:00',N'2013/12/18 0:00:00')

SET IDENTITY_INSERT [cms_users] OFF
