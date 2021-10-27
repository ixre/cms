ALTER TABLE `cms_site` 
CHANGE COLUMN `site_id` `site_id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '站点编号' ,
CHANGE COLUMN `language` `language` INT(1) NOT NULL COMMENT '语言' ,
CHANGE COLUMN `tpl` `tpl` VARCHAR(100) NULL DEFAULT NULL COMMENT '模板' ,
CHANGE COLUMN `note` `note` VARCHAR(200) NULL DEFAULT NULL COMMENT '备注' ,
CHANGE COLUMN `seo_title` `seo_title` VARCHAR(200) NULL DEFAULT NULL COMMENT ' SEO标题' ,
CHANGE COLUMN `seo_keywords` `seo_keywords` VARCHAR(250) NULL DEFAULT NULL COMMENT ' SEO关键词' ,
CHANGE COLUMN `seo_description` `seo_description` VARCHAR(250) NULL DEFAULT NULL COMMENT 'SEO描述' ,
CHANGE COLUMN `state` `state` INT(1) NOT NULL COMMENT '状态' ,
CHANGE COLUMN `pro_tel` `pro_tel` VARCHAR(50) NULL DEFAULT NULL COMMENT '电话' ,
CHANGE COLUMN `pro_phone` `pro_phone` VARCHAR(11) NULL DEFAULT NULL COMMENT '手机' ,
CHANGE COLUMN `pro_fax` `pro_fax` VARCHAR(50) NULL DEFAULT NULL COMMENT '传真' ,
CHANGE COLUMN `pro_address` `pro_address` VARCHAR(100) NULL DEFAULT NULL COMMENT '地址' ,
CHANGE COLUMN `pro_email` `pro_email` VARCHAR(100) NULL DEFAULT NULL COMMENT '邮箱' ,
CHANGE COLUMN `pro_im` `pro_im` VARCHAR(100)  NULL DEFAULT NULL COMMENT 'IM' ,
CHANGE COLUMN `pro_post` `pro_post` VARCHAR(100) NULL DEFAULT NULL COMMENT '邮政编码' ,
CHANGE COLUMN `pro_notice` `pro_notice` VARCHAR(250) NULL DEFAULT NULL COMMENT '通知' ,
CHANGE COLUMN `pro_slogan` `pro_slogan` VARCHAR(250) NULL DEFAULT NULL COMMENT '标语' ;


ALTER TABLE cms_site ADD seo_force_https int(4) DEFAULT 0 NOT NULL COMMENT '强制HTTPS';
ALTER TABLE cms_site ADD seo_force_redirect int(4) DEFAULT 0 NULL COMMENT '强制重定向';
ALTER TABLE cms_site ADD alone_board int(4) DEFAULT 0 NULL COMMENT '独立管理面板';


-- cms.cms_site_variable definition

CREATE TABLE `cms_site_variable` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号',
  `site_id` int(11) NOT NULL DEFAULT 0 COMMENT '站点编号',
  `name` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '名称',
  `value` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '值',
  `remark` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`id`)
) DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='站点变量';


