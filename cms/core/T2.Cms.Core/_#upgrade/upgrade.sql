
          
ALTER TABLE `cms_archive` 
CHANGE COLUMN `alias` `alias` VARCHAR(50) NULL DEFAULT NULL COMMENT ' 别名' ,
ADD COLUMN `site_id` INT NOT NULL COMMENT '站点编号' AFTER `str_id`;


ALTER TABLE `cms_archive` 
CHANGE COLUMN `cid` `cat_id` INT(11) NOT NULL COMMENT '栏目编号' ;

update cms_archive set site_id=(select site_id FROM cms_category where id=cat_id);


ALTER TABLE `cms_category` 
CHANGE COLUMN `site_id` `site_id` INT(11) NOT NULL DEFAULT 1 COMMENT '站点编号' ,
ADD COLUMN `parent_id` INT NOT NULL COMMENT '上级编号' AFTER `site_id`;

ALTER TABLE `cms_category` 
ADD COLUMN `code` VARCHAR(40) NOT NULL COMMENT '代码' AFTER `parent_id`,
ADD COLUMN `path` VARCHAR(180) NOT NULL COMMENT '路径' AFTER `code`,
ADD COLUMN `flag` INT(8) NOT NULL COMMENT '标记' AFTER `path`;

ALTER TABLE `cms_category` 
CHANGE COLUMN `moduleid` `module_id` INT(11) NOT NULL COMMENT '模块编号';

ALTER TABLE `cms_category` 
ADD COLUMN `module_id` INT NULL COMMENT '模块编号' AFTER `icon`;


ALTER TABLE `cms_category` 
CHANGE COLUMN `id` `id` INT(11) NOT NULL AUTO_INCREMENT ;


ALTER TABLE `cms_site` 
CHANGE COLUMN `name` `name` VARCHAR(50) NOT NULL COMMENT '站点名称' ,
CHANGE COLUMN `dir_name` `app_name` VARCHAR(50) NOT NULL COMMENT '应用名称' ,
CHANGE COLUMN `domain` `domain` VARCHAR(50) NOT NULL COMMENT '域名' ,
CHANGE COLUMN `location` `location` VARCHAR(100) NOT NULL COMMENT '重定向URL' ;


