-- MySQL dump 10.13  Distrib 5.6.22, for osx10.8 (x86_64)
--
-- Host: 192.168.31.129    Database: cms
-- ------------------------------------------------------
-- Server version	5.5.41-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cms_archive`
--

DROP TABLE IF EXISTS `cms_archive`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_archive` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `strid` varchar(16) NOT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `cid` int(11) NOT NULL,
  `publisher_id` varchar(50) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `small_title` varchar(100) DEFAULT NULL,
  `location` varchar(150) DEFAULT NULL,
  `sort_number` int(50) DEFAULT '0',
  `source` varchar(50) DEFAULT NULL,
  `tags` varchar(100) DEFAULT NULL,
  `outline` varchar(255) DEFAULT NULL,
  `content` text,
  view_count int(50) DEFAULT '0',
  `agree` int(11) DEFAULT NULL,
  `disagree` int(11) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  `lastmodifydate` datetime DEFAULT NULL,
  `flags` varchar(100) DEFAULT '{st:''''0'''',sc:''''0'''',v:''''1'''',p:''''0''''}',
  `thumbnail` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `id_index` (`id`,`alias`),
  KEY `cid_index` (`cid`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_archive`
--

LOCK TABLES `cms_archive` WRITE;
/*!40000 ALTER TABLE `cms_archive` DISABLE KEYS */;
INSERT INTO `cms_archive` VALUES (2,'cms','welcome',2,'1','欢迎使用J6 Cms .NET',NULL,NULL,0,NULL,NULL,NULL,'<div style=\\\"text-align:center;font-size:30px\\\"><h2>欢迎使用J6 Cms .NET!</h2></div>',0,NULL,NULL,'2015-08-01 00:00:00','2015-08-01 00:00:00','{st:\'\'0\'\',sc:\'\'0\'\',v:\'\'1\'\',p:\'\'0\'\'}',NULL);
/*!40000 ALTER TABLE `cms_archive` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_category`
--

DROP TABLE IF EXISTS `cms_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_category` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `siteid` int(10) DEFAULT '1',
  `moduleid` int(11) NOT NULL,
  `tag` varchar(100) DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  `icon` varchar(150) DEFAULT NULL,
  `lft` int(11) DEFAULT NULL,
  `rgt` int(11) DEFAULT NULL,
  `pagetitle` varchar(200) DEFAULT NULL,
  `keywords` varchar(200) DEFAULT NULL,
  `description` varchar(250) DEFAULT NULL,
  `location` varchar(150) DEFAULT NULL COMMENT '跳转到的地址',
  `orderindex` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_category`
--

LOCK TABLES `cms_category` WRITE;
/*!40000 ALTER TABLE `cms_category` DISABLE KEYS */;
INSERT INTO `cms_category` VALUES (1,1,1,'root','根栏目','',1,4,'','','','',0),(2,1,1,'cms','欢迎使用','',2,3,'','','','',0);
/*!40000 ALTER TABLE `cms_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_categoryExtend`
--

DROP TABLE IF EXISTS `cms_categoryExtend`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_categoryExtend` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `categoryId` int(11) NOT NULL,
  `extendId` int(11) NOT NULL,
  `enabled` bit(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_categoryExtend`
--

LOCK TABLES `cms_categoryExtend` WRITE;
/*!40000 ALTER TABLE `cms_categoryExtend` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_categoryExtend` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_comment`
--

DROP TABLE IF EXISTS `cms_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_comment` (
  `id` int(16) NOT NULL,
  `archiveid` varchar(16) DEFAULT NULL,
  `memberid` int(11) DEFAULT NULL,
  `ip` varchar(20) DEFAULT NULL,
  `content` text,
  `recycle` tinyint(1) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_comment`
--

LOCK TABLES `cms_comment` WRITE;
/*!40000 ALTER TABLE `cms_comment` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_credential`
--

DROP TABLE IF EXISTS `cms_credential`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_credential` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) DEFAULT NULL,
  `user_name` varchar(20) DEFAULT NULL,
  `password` varchar(32) DEFAULT NULL,
  `enabled` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_credential`
--

LOCK TABLES `cms_credential` WRITE;
/*!40000 ALTER TABLE `cms_credential` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_credential` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_extendField`
--

DROP TABLE IF EXISTS `cms_extendField`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_extendField` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `siteId` int(11) DEFAULT NULL,
  `name` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `defaultValue` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `regex` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `message` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_extendField`
--

LOCK TABLES `cms_extendField` WRITE;
/*!40000 ALTER TABLE `cms_extendField` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_extendField` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_extendValue`
--

DROP TABLE IF EXISTS `cms_extendValue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_extendValue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relationId` int(11) NOT NULL,
  `relationType` int(11) NOT NULL,
  `fieldId` int(11) NOT NULL,
  `fieldValue` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_extendValue`
--

LOCK TABLES `cms_extendValue` WRITE;
/*!40000 ALTER TABLE `cms_extendValue` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_extendValue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_link`
--

DROP TABLE IF EXISTS `cms_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_link` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pid` int(11) DEFAULT NULL,
  `siteid` int(10) DEFAULT '1',
  `type` int(11) NOT NULL,
  `text` varchar(100) NOT NULL,
  `uri` varchar(255) NOT NULL,
  `target` varchar(50) DEFAULT NULL,
  `imgurl` varchar(100) DEFAULT NULL,
  `bind` varchar(20) DEFAULT NULL,
  `orderIndex` int(11) DEFAULT NULL,
  `visible` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_link`
--

LOCK TABLES `cms_link` WRITE;
/*!40000 ALTER TABLE `cms_link` DISABLE KEYS */;
INSERT INTO `cms_link` VALUES (1,0,1,2,'SPC.NET','http://z3q.net/cms/cms/','_blank',NULL,NULL,2,1),(2,0,1,1,'首页','/',NULL,NULL,NULL,1,1),(3,0,1,1,'欢迎使用','/cms/welcome.html',NULL,NULL,NULL,2,1);
/*!40000 ALTER TABLE `cms_link` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_log`
--

DROP TABLE IF EXISTS `cms_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_log` (
  `id` varchar(30) NOT NULL,
  `typeid` int(11) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `content` text,
  `helplink` varchar(255) DEFAULT NULL,
  `recorddate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_log`
--

LOCK TABLES `cms_log` WRITE;
/*!40000 ALTER TABLE `cms_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_member`
--

DROP TABLE IF EXISTS `cms_member`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_member` (
  `id` int(11) NOT NULL,
  `username` varchar(20) DEFAULT NULL,
  `password` varchar(40) DEFAULT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `sex` varchar(7) DEFAULT NULL,
  `nickname` varchar(15) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `telphone` varchar(20) DEFAULT NULL,
  `note` varchar(255) DEFAULT NULL,
  `usergroupid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_member`
--

LOCK TABLES `cms_member` WRITE;
/*!40000 ALTER TABLE `cms_member` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_member` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_memberdetails`
--

DROP TABLE IF EXISTS `cms_memberdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_memberdetails` (
  `uid` int(11) NOT NULL,
  `status` varchar(10) DEFAULT NULL,
  `regip` varchar(15) DEFAULT NULL,
  `regtime` datetime DEFAULT NULL,
  `lastlogintime` datetime DEFAULT NULL,
  `token` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_memberdetails`
--

LOCK TABLES `cms_memberdetails` WRITE;
/*!40000 ALTER TABLE `cms_memberdetails` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_memberdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_message`
--

DROP TABLE IF EXISTS `cms_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_message` (
  `id` int(16) NOT NULL,
  `senduid` int(11) DEFAULT NULL,
  `receiveuid` int(11) DEFAULT NULL,
  `subject` varchar(50) DEFAULT NULL,
  `content` varchar(255) DEFAULT NULL,
  `hasread` tinyint(1) DEFAULT NULL,
  `recycle` tinyint(1) DEFAULT NULL,
  `senddate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_message`
--

LOCK TABLES `cms_message` WRITE;
/*!40000 ALTER TABLE `cms_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_modules`
--

DROP TABLE IF EXISTS `cms_modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_modules` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `siteid` int(1) DEFAULT '1',
  `name` varchar(50) NOT NULL,
  `issystem` tinyint(1) DEFAULT NULL,
  `isdelete` tinyint(1) DEFAULT NULL,
  `extid1` int(11) DEFAULT '0',
  `extid2` int(11) DEFAULT '0',
  `extid3` int(11) DEFAULT '0',
  `extid4` int(11) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_modules`
--

LOCK TABLES `cms_modules` WRITE;
/*!40000 ALTER TABLE `cms_modules` DISABLE KEYS */;
INSERT INTO `cms_modules` VALUES (1,0,'自定义页面',1,0,0,0,0,0),(2,0,'文档',1,0,0,0,0,0);
/*!40000 ALTER TABLE `cms_modules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_operation`
--

DROP TABLE IF EXISTS `cms_operation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_operation` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `path` varchar(255) DEFAULT NULL,
  `available` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=86 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_operation`
--

LOCK TABLES `cms_operation` WRITE;
/*!40000 ALTER TABLE `cms_operation` DISABLE KEYS */;
INSERT INTO `cms_operation` VALUES (44,'友情链接','link?view=list&type=friendlink',1),(45,'分类->分类列表','catalog?view=list',1),(46,'系统->网站设置','config?id=1',1),(47,'系统->网站基本资料修改','config?id=2',1),(48,'系统->网站优化设置','config?id=3',1),(49,'错误日志','system?view=errorlog',1),(50,'内容->页面管理','archive?view=list&type=1',1),(51,'内容->添加页面','archive?view=create&type=1',1),(52,'内容->信息列表','archive?view=list&type=2',1),(53,'内容->发布信息','archive?view=create&type=2',1),(54,'分类->删除分类','app.axd?do=catalog:delete',1),(55,'分类->修改分类','app.axd?do=catalog:update',1),(56,'会员>会员列表','user?view=member',1),(57,'会员>删除会员','app.axd?do=member:delete',1),(58,'系统用户管理','user?view=user',1),(59,'删除评论','app.axd?do=archive:deletecomment',1),(60,'头部导航链接','link?view=list&type=headerlink',1),(61,'网站导航链接','link?view=list&type=navigation',1),(62,'添加新链接','link?view=create',1),(63,'修改链接','link?view=edit',1),(64,'删除链接','app.axd?do=link:delete',1),(65,'更新页面','archive?view=update&typeid=1',1),(66,'更新信息','archive?view=update&typeid=2',1),(67,'更新图文信息','archive?view=update&typeid=3',1),(68,'更新画廊信息','archive?view=update&typeid=4',1),(69,'更新视频信息','archive?view=update&typeid=5',1),(70,'更新专题信息','archive?view=update&typeid=6',1),(71,'图文信息列表','archive?view=list&typeid=3',1),(72,'画廊息列表','archive?view=list&typeid=4',1),(73,'视频信息列表','archive?view=list&typeid=5',1),(74,'专题列表','archive?view=list&typeid=6',1),(75,'发布图文信息','archive?view=create&typeid=3',1),(76,'发布画廊信息','archive?view=create&typeid=4',1),(77,'发布视频信息','archive?view=create&typeid=5',1),(78,'创建新专题','archive?view=create&typeid=6',1),(79,'清除缓存','system?view=clearcache',1),(80,'操作列表','operation?view=list',1),(81,'用户组操作权限设置','operation?view=set',1),(82,'清除错误日志','/app.axd?log:clearErrorLog',1),(83,'删除文档','archive:delete',1),(84,'刷新文档创建时间','archive:refresh',1),(85,'内容采集','/plugin/collection.ashx',1);
/*!40000 ALTER TABLE `cms_operation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_related_link`
--

DROP TABLE IF EXISTS `cms_related_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_related_link` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `typeIndent` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `relatedId` int(11) DEFAULT NULL,
  `name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `title` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uri` varchar(150) COLLATE utf8_unicode_ci DEFAULT NULL,
  `enabled` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_related_link`
--

LOCK TABLES `cms_related_link` WRITE;
/*!40000 ALTER TABLE `cms_related_link` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_related_link` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_review`
--

DROP TABLE IF EXISTS `cms_review`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_review` (
  `id` varchar(255) NOT NULL,
  `members` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_review`
--

LOCK TABLES `cms_review` WRITE;
/*!40000 ALTER TABLE `cms_review` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_review` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_site`
--

DROP TABLE IF EXISTS `cms_site`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_site` (
  `siteid` int(1) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `dirname` varchar(50) DEFAULT NULL,
  `domain` varchar(50) DEFAULT NULL,
  `location` varchar(100) DEFAULT NULL,
  `language` int(1) NOT NULL,
  `tpl` varchar(100) DEFAULT NULL,
  `note` varchar(200) DEFAULT NULL,
  `seotitle` varchar(200) DEFAULT NULL,
  `seokeywords` varchar(250) DEFAULT NULL,
  `seodescription` varchar(250) DEFAULT NULL,
  `state` int(1) NOT NULL,
  `protel` varchar(50) DEFAULT NULL,
  `prophone` varchar(11) DEFAULT NULL,
  `profax` varchar(50) DEFAULT NULL,
  `proaddress` varchar(100) DEFAULT NULL,
  `proemail` varchar(100) DEFAULT NULL,
  `im` varchar(100) DEFAULT NULL,
  `postcode` varchar(100) DEFAULT NULL,
  `pronotice` varchar(250) DEFAULT NULL,
  `proslogan` varchar(250) DEFAULT NULL,
  PRIMARY KEY (`siteid`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_site`
--

LOCK TABLES `cms_site` WRITE;
/*!40000 ALTER TABLE `cms_site` DISABLE KEYS */;
INSERT INTO `cms_site` VALUES (1,'默认站点',NULL,NULL,NULL,1,'default',NULL,'默认站点-Speicial Cms .NET!',NULL,NULL,1,'','','','',NULL,NULL,NULL,'SPC.NET是一款跨平台支持多站点基于ASP.NET MVC技术架构的内容管理系统!','');
/*!40000 ALTER TABLE `cms_site` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_table`
--

DROP TABLE IF EXISTS `cms_table`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `note` varchar(500) DEFAULT NULL,
  `apiserver` varchar(200) DEFAULT NULL,
  `issystem` bit(1) NOT NULL,
  `available` bit(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_table`
--

LOCK TABLES `cms_table` WRITE;
/*!40000 ALTER TABLE `cms_table` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_table` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_table_column`
--

DROP TABLE IF EXISTS `cms_table_column`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table_column` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tableid` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `note` varchar(50) DEFAULT NULL,
  `validformat` varchar(200) DEFAULT NULL,
  `orderindex` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_table_column`
--

LOCK TABLES `cms_table_column` WRITE;
/*!40000 ALTER TABLE `cms_table_column` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_table_column` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_table_row`
--

DROP TABLE IF EXISTS `cms_table_row`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table_row` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tableid` int(11) NOT NULL,
  `submittime` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_table_row`
--

LOCK TABLES `cms_table_row` WRITE;
/*!40000 ALTER TABLE `cms_table_row` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_table_row` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_table_rowdata`
--

DROP TABLE IF EXISTS `cms_table_rowdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table_rowdata` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL,
  `cid` int(11) NOT NULL,
  `value` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_table_rowdata`
--

LOCK TABLES `cms_table_rowdata` WRITE;
/*!40000 ALTER TABLE `cms_table_rowdata` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_table_rowdata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_tplbind`
--

DROP TABLE IF EXISTS `cms_tplbind`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_tplbind` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bindid` int(20) NOT NULL,
  `bindtype` int(2) NOT NULL,
  `tplpath` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_tplbind`
--

LOCK TABLES `cms_tplbind` WRITE;
/*!40000 ALTER TABLE `cms_tplbind` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_tplbind` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_user`
--

DROP TABLE IF EXISTS `cms_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `avatar` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '1',
  `phone` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `email` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `flag` int(11) DEFAULT NULL,
  `check_code` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `last_login_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_user`
--

LOCK TABLES `cms_user` WRITE;
/*!40000 ALTER TABLE `cms_user` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_user_role`
--

DROP TABLE IF EXISTS `cms_user_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_user_role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `app_id` int(11) NOT NULL,
  `flag` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_user_role`
--

LOCK TABLES `cms_user_role` WRITE;
/*!40000 ALTER TABLE `cms_user_role` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_user_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_usergroup`
--

DROP TABLE IF EXISTS `cms_usergroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_usergroup` (
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `permissions` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_usergroup`
--

LOCK TABLES `cms_usergroup` WRITE;
/*!40000 ALTER TABLE `cms_usergroup` DISABLE KEYS */;
INSERT INTO `cms_usergroup` VALUES (1,'超级管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42'),(2,'管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,40,41,42'),(3,'编辑','1,2,3,4,5,6,10,11,12,13,14,15'),(4,'会员','1,2,3,4,5,6'),(5,'游客','3,4'),(0,NULL,NULL);
/*!40000 ALTER TABLE `cms_usergroup` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-07-18 12:07:12
