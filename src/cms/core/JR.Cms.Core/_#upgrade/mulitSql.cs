/*


2013-06-28 => cms_sites 表

2013-07-24 => cms_category 添加siteid字段
2013-07-25 => cms_link 添加siteid字段
2013-07-26 => cms_user 添加siteid,userid字段
2013-09-25 => cms_modules 增加siteid,默认为0
 * 
 * 2014-04-06  links index = > orderindex
 * 2014-04-27  sites   proqq  -> im   promsn => postcode
 * 2014-10-14  archives  添加location
 * 2014-12-12  site表增加location
 *                    _sites => _site
 *                    _users => _user
 *  2014-04-16 archive表增加small_title,sort_number
 *  2015-06-16  UserGroup表修改
 *    CREATE TABLE "cms_usergroup" ("id" integer PRIMARY KEY  NOT NULL  DEFAULT (null) ,"name" varchar (50) DEFAULT (null) ,"permission" varchar (255) DEFAULT (null) ,"is_master" INTEGER DEFAULT (0) ,"is_super" INTEGER DEFAULT (0) ,"enabled" INTEGER DEFAULT (1) )
*/