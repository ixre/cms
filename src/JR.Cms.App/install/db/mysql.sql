-- MySQL dump 10.13  Distrib 5.7.17, for macos10.12 (x86_64)
--
-- Host: 127.0.0.1    Database: cms
-- ------------------------------------------------------
-- Server version	5.5.5-10.2.15-MariaDB

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
  `str_id` varchar(16) NOT NULL,
  `site_id` int(11) NOT NULL COMMENT '站点编号',
  `alias` varchar(50) DEFAULT NULL COMMENT ' 别名',
  `cat_id` int(11) NOT NULL COMMENT '栏目编号',
  `path` varchar(180) NOT NULL COMMENT '文档路径',
  `flag` int(8) NOT NULL COMMENT '文档标志',
  `author_id` int(11) NOT NULL COMMENT '作者编号',
  `title` varchar(100) DEFAULT NULL,
  `small_title` varchar(100) DEFAULT NULL,
  `location` varchar(150) DEFAULT NULL,
  `sort_number` int(50) DEFAULT 0,
  `source` varchar(50) DEFAULT NULL,
  `tags` varchar(100) DEFAULT NULL,
  `outline` varchar(255) DEFAULT NULL,
  `content` text DEFAULT NULL,
  `view_count` int(11) DEFAULT 0,
  `agree` int(11) DEFAULT 0,
  `disagree` int(11) DEFAULT 0,
  `thumbnail` varchar(150) DEFAULT NULL,
  `create_time` int(11) NOT NULL COMMENT '创建时间',
  `update_time` int(11) NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `id_index` (`id`,`alias`),
  KEY `cid_index` (`cat_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_archive`
--

LOCK TABLES `cms_archive` WRITE;
/*!40000 ALTER TABLE `cms_archive` DISABLE KEYS */;
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
  `site_id` int(11) NOT NULL DEFAULT 1 COMMENT '站点编号',
  `parent_id` int(11) NOT NULL COMMENT '上级编号',
  `code` varchar(40) NOT NULL COMMENT '代码',
  `path` varchar(180) NOT NULL COMMENT '路径',
  `flag` int(8) NOT NULL COMMENT '标记',
  `module_id` int(11) NOT NULL COMMENT '模块编号',
  `tag` varchar(100) DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  `icon` varchar(150) DEFAULT NULL,
  `page_title` varchar(200) DEFAULT NULL,
  `page_keywords` varchar(200) DEFAULT NULL,
  `page_description` varchar(250) DEFAULT NULL,
  `location` varchar(150) DEFAULT NULL COMMENT '跳转到的地址',
  `sort_number` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_category`
--

LOCK TABLES `cms_category` WRITE;
/*!40000 ALTER TABLE `cms_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_site_variables`
--
DROP TABLE IF EXISTS `cms_site_variables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_site_variables` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号',
  `site_id` int(11) NOT NULL DEFAULT 0 COMMENT '站点编号',
  `name` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '名称',
  `value` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '值',
  `remark` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`id`)
) DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='站点变量';


--
-- Table structure for table `cms_category_extend`
--

DROP TABLE IF EXISTS `cms_category_extend`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_category_extend` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category_id` int(11) NOT NULL,
  `extend_id` int(11) NOT NULL,
  `enabled` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_category_extend`
--

LOCK TABLES `cms_category_extend` WRITE;
/*!40000 ALTER TABLE `cms_category_extend` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_category_extend` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_comment`
--

-- cms.cms_extend_field definition
DROP TABLE IF EXISTS `cms_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_comment` (
  `id` int(16) NOT NULL,
  `archiveid` varchar(16) DEFAULT NULL,
  `memberid` int(11) DEFAULT NULL,
  `ip` varchar(20) DEFAULT NULL,
  `content` text DEFAULT NULL,
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
  `password` varchar(50) DEFAULT NULL,
  `enabled` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_credential`
--

LOCK TABLES `cms_credential` WRITE;
/*!40000 ALTER TABLE `cms_credential` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_credential` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_extend_field`
--

DROP TABLE IF EXISTS `cms_extend_field`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_extend_field` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `site_id` int(11) DEFAULT NULL,
  `name` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `default_value` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `regex` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `message` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_extend_field`
--

LOCK TABLES `cms_extend_field` WRITE;
/*!40000 ALTER TABLE `cms_extend_field` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_extend_field` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_extend_value`
--

DROP TABLE IF EXISTS `cms_extend_value`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_extend_value` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relation_id` int(11) NOT NULL,
  `relation_type` int(11) NOT NULL,
  `field_id` int(11) NOT NULL,
  `field_value` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_extend_value`
--

LOCK TABLES `cms_extend_value` WRITE;
/*!40000 ALTER TABLE `cms_extend_value` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_extend_value` ENABLE KEYS */;
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
  `site_id` int(11) DEFAULT 1,
  `type` int(11) NOT NULL,
  `text` varchar(100) NOT NULL,
  `uri` varchar(255) NOT NULL,
  `target` varchar(50) DEFAULT NULL,
  `img_url` varchar(100) DEFAULT NULL,
  `bind` varchar(20) DEFAULT NULL,
  `sort_number` int(11) DEFAULT NULL,
  `visible` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_link`
--

LOCK TABLES `cms_link` WRITE;
/*!40000 ALTER TABLE `cms_link` DISABLE KEYS */;
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
  `content` text DEFAULT NULL,
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
  `siteid` int(1) DEFAULT 1,
  `name` varchar(50) NOT NULL,
  `issystem` tinyint(1) DEFAULT NULL,
  `isdelete` tinyint(1) DEFAULT NULL,
  `extid1` int(11) DEFAULT 0,
  `extid2` int(11) DEFAULT 0,
  `extid3` int(11) DEFAULT 0,
  `extid4` int(11) DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_modules`
--

LOCK TABLES `cms_modules` WRITE;
/*!40000 ALTER TABLE `cms_modules` DISABLE KEYS */;
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
  `available` tinyint(1) DEFAULT 0,
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
  `content_type` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `content_id` int(11) DEFAULT NULL,
  `related_site_id` int(11) DEFAULT NULL,
  `related_indent` int(5) DEFAULT NULL,
  `related_content_id` int(11) DEFAULT NULL,
  `enabled` tinyint(1) DEFAULT NULL,
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
  `site_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL COMMENT '站点名称',
  `app_name` varchar(50) NOT NULL COMMENT '应用名称',
  `domain` varchar(120) NOT NULL COMMENT '域名',
  `location` varchar(120) NOT NULL COMMENT '重定向URL',
  `language` int(1) NOT NULL,
  `tpl` varchar(100) DEFAULT NULL,
  `note` varchar(200) DEFAULT NULL,
  `seo_title` varchar(200) DEFAULT NULL,
  `seo_keywords` varchar(250) DEFAULT NULL,
  `seo_description` varchar(250) DEFAULT NULL,
  `seo_force_https` int(4) NOT NULL DEFAULT 0 COMMENT '强制HTTPS',
  `seo_force_redirect` int(4) NOT NULL DEFAULT 0 COMMENT '强制重定向',
  `state` int(1) NOT NULL,
  `alone_board` int(4) NOT NULL DEFAULT 0 COMMENT '独立管理面板',
  `pro_tel` varchar(50) DEFAULT NULL,
  `pro_phone` varchar(11) DEFAULT NULL,
  `pro_fax` varchar(50) DEFAULT NULL,
  `pro_address` varchar(100) DEFAULT NULL,
  `pro_email` varchar(100) DEFAULT NULL,
  `pro_im` varchar(100) DEFAULT NULL,
  `pro_post` varchar(100) DEFAULT NULL,
  `pro_notice` varchar(250) DEFAULT NULL,
  `pro_slogan` varchar(250) DEFAULT NULL,
  `beian_no` varchar(20) NOT NULL DEFAULT '' COMMENT '网站备案号',
  PRIMARY KEY (`site_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_site`
--

LOCK TABLES `cms_site` WRITE;
/*!40000 ALTER TABLE `cms_site` DISABLE KEYS */;
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
  `api_server` varchar(200) DEFAULT NULL,
  `is_system` tinyint(1) NOT NULL,
  `enabled` tinyint(1) NOT NULL,
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
  `table_id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `note` varchar(50) DEFAULT NULL,
  `valid_format` varchar(200) DEFAULT NULL,
  `sort_number` int(11) NOT NULL,
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
-- Table structure for table `cms_table_record`
--

DROP TABLE IF EXISTS `cms_table_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table_record` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `row_id` int(11) NOT NULL,
  `col_id` int(11) NOT NULL,
  `value` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_table_record`
--

LOCK TABLES `cms_table_record` WRITE;
/*!40000 ALTER TABLE `cms_table_record` DISABLE KEYS */;
/*!40000 ALTER TABLE `cms_table_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_table_row`
--

DROP TABLE IF EXISTS `cms_table_row`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_table_row` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `table_id` int(11) NOT NULL,
  `submit_time` datetime NOT NULL,
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
-- Table structure for table `cms_tpl_bind`
--

DROP TABLE IF EXISTS `cms_tpl_bind`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cms_tpl_bind` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bind_id` int(20) NOT NULL,
  `bind_type` int(2) NOT NULL,
  `tpl_path` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=22 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_tpl_bind`
--

LOCK TABLES `cms_tpl_bind` WRITE;
/*!40000 ALTER TABLE `cms_tpl_bind` DISABLE KEYS */;
INSERT INTO `cms_tpl_bind` VALUES (1,4,3,'category.case.html'),(2,4,4,'archive.case.html'),(3,2,4,'archive.about.html'),(4,7,3,'category.news.html'),(5,5,3,'category.partner.html'),(6,23,5,'archive.agent-dm.html'),(19,3,3,'category.product.html'),(8,24,5,'product-wechat-card.html'),(9,25,5,'product-wechat-employee.html'),(10,27,5,'product-cashier-demo.html'),(11,31,5,'product-finance-demo.html'),(12,30,5,'product-marketing-demo.html'),(13,29,5,'product-customer-demo.html'),(14,28,5,'product-order-demo.html'),(15,26,5,'product-set-up-demo.html'),(17,42,5,'product-cashier-demo_new.html'),(18,43,5,'product-warehouse-demo.html'),(20,50,5,'archive'),(21,3,5,'archive');
/*!40000 ALTER TABLE `cms_tpl_bind` ENABLE KEYS */;
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
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
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
/*!40000 ALTER TABLE `cms_usergroup` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `cms_site_word`
--
DROP TABLE IF EXISTS `cms_site_word`;
CREATE TABLE cms_site_word (
   id BIGINT AUTO_INCREMENT NOT NULL COMMENT '编号',
   word varchar(20) NOT NULL COMMENT '关键词',
   url varchar(180) NOT NULL COMMENT '链接地址',
   title varchar(120) NOT NULL COMMENT '描述',
   PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='站内关键词';

--
-- Dumping data for table `cms_job`
--
DROP TABLE IF EXISTS `cms_job`;
CREATE TABLE cms_job(
   id BIGINT AUTO_INCREMENT NOT NULL COMMENT '编号',
   job_name varchar(20) NOT NULL COMMENT '任务名称',
   job_class varchar(180) NOT NULL COMMENT '任务类',
   cron_exp varchar(120) NOT NULL COMMENT 'CRON表达式',
   job_describe varchar(180) NOT NULL COMMENT '任务描述',
   enabled int(2) NOT NULL COMMENT '是否启用',
   PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='定时任务';

--
-- Dumping data for table `cms_job`
--
DROP TABLE IF EXISTS `cms_search_engine`;
CREATE TABLE cms_search_engine(
    id BIGINT AUTO_INCREMENT NOT NULL COMMENT '编号',
    site_id BIGINT NOT NULL COMMENT '站点编号',
    site_url varchar(40) not null comment 'URL前缀',
    baidu_site_token varchar(180) NOT NULL COMMENT '百度推送Token',
    PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='搜索引擎设置';


/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-09-27 14:19:37
