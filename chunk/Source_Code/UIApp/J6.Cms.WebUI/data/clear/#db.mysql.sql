CREATE DATABASE  IF NOT EXISTS `db1` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;
USE `db1`;
-- MySQL dump 10.13  Distrib 5.1.61, for redhat-linux-gnu (i386)
--
-- Host: localhost    Database: db1
-- ------------------------------------------------------
-- Server version	5.1.61

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TImE_ZONE=@@TImE_ZONE */;
/*!40103 SET TImE_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `O_operation`
--

DROP TABLE IF EXISTS `O_operation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_operation` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `Path` varchar(255) DEFAULT NULL,
  `available` tinyint(1) DEFAULT '0',
  PRImARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=44 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_operation`
--

LOCK TABLES `O_operation` WRITE;
/*!40000 ALTER TABLE `O_operation` DISABLE KEYS */;
INSERT INTO `O_operation` VALUES (1,'友情链接','link?view=list&type=friendlink',1),(2,'分类->分类列表','catalog?view=list',1),(3,'系统->网站设置','config?id=1',1),(4,'系统->网站基本资料修改','config?id=2',1),(5,'系统->网站优化设置','config?id=3',1),(6,'错误日志','system?view=errorlog',1),(7,'内容->页面管理','archive?view=list&type=1',1),(8,'内容->添加页面','archive?view=create&type=1',1),(9,'内容->信息列表','archive?view=list&type=2',1),(10,'内容->发布信息','archive?view=create&type=2',1),(11,'分类->删除分类','app.axd?do=catalog:delete',1),(12,'分类->修改分类','app.axd?do=catalog:update',1),(13,'会员>会员列表','user?view=member',1),(14,'会员>删除会员','app.axd?do=member:delete',1),(15,'系统用户管理','user?view=user',1),(16,'删除评论','app.axd?do=archive:deletecomment',1),(17,'头部导航链接','link?view=list&type=headerlink',1),(18,'网站导航链接','link?view=list&type=navigation',1),(19,'添加新链接','link?view=create',1),(20,'修改链接','link?view=edit',1),(21,'删除链接','app.axd?do=link:delete',1),(22,'更新页面','archive?view=update&typeid=1',1),(23,'更新信息','archive?view=update&typeid=2',1),(24,'更新图文信息','archive?view=update&typeid=3',1),(25,'更新画廊信息','archive?view=update&typeid=4',1),(26,'更新视频信息','archive?view=update&typeid=5',1),(27,'更新专题信息','archive?view=update&typeid=6',1),(28,'图文信息列表','archive?view=list&typeid=3',1),(29,'画廊息列表','archive?view=list&typeid=4',1),(30,'视频信息列表','archive?view=list&typeid=5',1),(31,'专题列表','archive?view=list&typeid=6',1),(32,'发布图文信息','archive?view=create&typeid=3',1),(33,'发布画廊信息','archive?view=create&typeid=4',1),(34,'发布视频信息','archive?view=create&typeid=5',1),(35,'创建新专题','archive?view=create&typeid=6',1),(36,'清除缓存','system?view=clearcache',1),(37,'操作列表','operation?view=list',1),(38,'用户组操作权限设置','operation?view=set',1),(39,'清除错误日志','/app.axd?log:clearErrorLog',1),(40,'删除文档','archive:delete',1),(41,'刷新文档创建时间','archive:refresh',1),(42,'内容采集','/plugin/collection.ashx',1);
/*!40000 ALTER TABLE `O_operation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_modules`
--

DROP TABLE IF EXISTS `O_modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_modules` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `viewtemplate` varchar(50) DEFAULT NULL,
  `islock` tinyint(1) DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_modules`
--

LOCK TABLES `O_modules` WRITE;
/*!40000 ALTER TABLE `O_modules` DISABLE KEYS */;
INSERT INTO `O_modules` VALUES (1,'µ¥Ò³','${content}',1),(2,'ÐÂÎÅ×ÊÑ¶','${content}',1),(3,'Í¼ÎÄÐÅÏ¢','${content}',1),(4,'»­ÀÈÐÅÏ¢','${content}',NULL),(5,'ÊÓÆµ','${content}',NULL),(6,'×¨Ìâ','${content}',NULL);
/*!40000 ALTER TABLE `O_modules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_members`
--

DROP TABLE IF EXISTS `O_members`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_members` (
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
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_members`
--

LOCK TABLES `O_members` WRITE;
/*!40000 ALTER TABLE `O_members` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_members` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_memberdetails`
--

DROP TABLE IF EXISTS `O_memberdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_memberdetails` (
  `uid` int(11) NOT NULL,
  `status` varchar(10) DEFAULT NULL,
  `regip` varchar(15) DEFAULT NULL,
  `regtime` datetime DEFAULT NULL,
  `lastlogintime` datetime DEFAULT NULL,
  `token` varchar(100) DEFAULT NULL,
  PRImARY KEY (`uid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_memberdetails`
--

LOCK TABLES `O_memberdetails` WRITE;
/*!40000 ALTER TABLE `O_memberdetails` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_memberdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_categories`
--

DROP TABLE IF EXISTS `O_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_categories` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL,
  `moduleid` int(11) NOT NULL,
  `tag` varchar(50) DEFAULT NULL,
  `name` varchar(50) DEFAULT NULL,
  `keywords` varchar(100) DEFAULT NULL,
  `description` varchar(200) DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_categories`
--

LOCK TABLES `O_categories` WRITE;
/*!40000 ALTER TABLE `O_categories` DISABLE KEYS */;
INSERT INTO `O_categories` VALUES (1,0,1,'page','页面',' ',' '),(2,0,2,'news','新闻','','');
/*!40000 ALTER TABLE `O_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_users`
--

DROP TABLE IF EXISTS `O_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_users` (
  `username` varchar(50) CHARACTER SET utf8 NOT NULL,
  `password` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `groupid` int(11) DEFAULT NULL,
  `available` tinyint(4) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  `lastlogindate` datetime DEFAULT NULL,
  PRImARY KEY (`username`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_users`
--

LOCK TABLES `O_users` WRITE;
/*!40000 ALTER TABLE `O_users` DISABLE KEYS */;
INSERT INTO `O_users` VALUES ('admin','285c96f7702357c07f9b5daee2660e79','系统管理员',1,1,'2011-05-20 11:44:38','2012-05-25 15:14:21');
/*!40000 ALTER TABLE `O_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_comment`
--

DROP TABLE IF EXISTS `O_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_comment` (
  `id` int(16) NOT NULL,
  `archiveid` varchar(16) DEFAULT NULL,
  `memberid` int(11) DEFAULT NULL,
  `ip` varchar(20) DEFAULT NULL,
  `content` text,
  `recycle` tinyint(1) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_comment`
--

LOCK TABLES `O_comment` WRITE;
/*!40000 ALTER TABLE `O_comment` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_review`
--

DROP TABLE IF EXISTS `O_review`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_review` (
  `id` varchar(255) NOT NULL,
  `members` text NOT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_review`
--

LOCK TABLES `O_review` WRITE;
/*!40000 ALTER TABLE `O_review` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_review` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_message`
--

DROP TABLE IF EXISTS `O_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_message` (
  `id` int(16) NOT NULL,
  `senduid` int(11) DEFAULT NULL,
  `receiveuid` int(11) DEFAULT NULL,
  `subject` varchar(50) DEFAULT NULL,
  `content` varchar(255) DEFAULT NULL,
  `hasread` tinyint(1) DEFAULT NULL,
  `recycle` tinyint(1) DEFAULT NULL,
  `senddate` datetime DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_message`
--

LOCK TABLES `O_message` WRITE;
/*!40000 ALTER TABLE `O_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_logs`
--

DROP TABLE IF EXISTS `O_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_logs` (
  `id` varchar(30) NOT NULL,
  `typeid` int(11) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `content` text,
  `helplink` varchar(255) DEFAULT NULL,
  `recorddate` datetime DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_logs`
--

LOCK TABLES `O_logs` WRITE;
/*!40000 ALTER TABLE `O_logs` DISABLE KEYS */;
/*!40000 ALTER TABLE `O_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_links`
--

DROP TABLE IF EXISTS `O_links`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_links` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` int(11) NOT NULL,
  `text` varchar(50) NOT NULL,
  `uri` varchar(255) NOT NULL,
  `target` varchar(50) DEFAULT NULL,
  `index` int(5) DEFAULT NULL,
  `visible` tinyint(1) NOT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_links`
--

LOCK TABLES `O_links` WRITE;
/*!40000 ALTER TABLE `O_links` DISABLE KEYS */;
INSERT INTO `O_links` VALUES (1,3,'建站软件','http://cms.s1n1.com/soft/opsite.html','_blank',2,1),(2,3,'奥博网络','http://cms.s1n1.com','_blank',1,1);
/*!40000 ALTER TABLE `O_links` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_archives`
--

DROP TABLE IF EXISTS `O_archives`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_archives` (
  `id` varchar(16) NOT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `cid` int(11) NOT NULL,
  `publisher_id` varchar(50) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `source` varchar(50) DEFAULT NULL,
  `tags` varchar(100) DEFAULT NULL,
  `outline` varchar(255) DEFAULT NULL,
  `content` text,
  `viewcount` varchar(50) DEFAULT NULL,
  `agree` int(11) DEFAULT NULL,
  `disagree` int(11) DEFAULT NULL,
  `isspecial` tinyint(1) DEFAULT NULL,
  `issystem` tinyint(1) DEFAULT NULL,
  `visible` tinyint(1) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  `lastmodifydate` datetime DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_archives`
--

LOCK TABLES `O_archives` WRITE;
/*!40000 ALTER TABLE `O_archives` DISABLE KEYS */;
INSERT INTO `O_archives` VALUES ('mjmpe','siteinfo',1,'admin','网站介绍','','','','网站介绍<br />','0',0,0,0,0,1,'2012-05-25 14:32:07',NULL),('6oi94','aboutus',1,'admin','关于我们','','','','<p>关天我们</p><p><br /></p><br />','7',0,0,1,1,1,'2012-05-25 14:11:25',NULL);
/*!40000 ALTER TABLE `O_archives` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `O_usergroup`
--

DROP TABLE IF EXISTS `O_usergroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `O_usergroup` (
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `permissions` varchar(255) DEFAULT NULL,
  PRImARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `O_usergroup`
--

LOCK TABLES `O_usergroup` WRITE;
/*!40000 ALTER TABLE `O_usergroup` DISABLE KEYS */;
INSERT INTO `O_usergroup` VALUES (1,'超级管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42'),(2,'管理员','1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,40,41,42'),(3,'编辑','1,2,3,4,5,6,10,11,12,13,14,15'),(4,'会员','1,2,3,4,5,6'),(5,'游客','3,4'),(0,'',NULL);
/*!40000 ALTER TABLE `O_usergroup` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TImE_ZONE=@OLD_TImE_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2012-05-25 19:06:46
