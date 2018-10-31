
          
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

update `cms_category` set path = tag WHERE path='';


ALTER TABLE `cms_site` 
CHANGE COLUMN `name` `name` VARCHAR(50) NOT NULL COMMENT '站点名称' ,
CHANGE COLUMN `dir_name` `app_name` VARCHAR(50) NOT NULL COMMENT '应用名称' ,
CHANGE COLUMN `domain` `domain` VARCHAR(50) NOT NULL COMMENT '域名' ,
CHANGE COLUMN `location` `location` VARCHAR(100) NOT NULL COMMENT '重定向URL' ;



------------------------------------------

ALTER TABLE `cms_archive` 
ADD COLUMN `path` VARCHAR(180) NOT NULL COMMENT '文档路径' AFTER `cat_id`,
ADD COLUMN `flag` INT(8) NOT NULL COMMENT '文档标志' AFTER `path`;


ALTER TABLE `cms_archive` 
CHANGE COLUMN `publisher_id` `author_id` INT NOT NULL COMMENT '作者编号' ,
ADD COLUMN `create_time` INT NOT NULL COMMENT '创建时间' AFTER `thumbnail`,
ADD COLUMN `update_time` INT NOT NULL COMMENT '修改时间' AFTER `create_time`;


UPDATE cms_archive set alias=str_id WHERE alias='';
update cms_archive set path=CONCAT((SELECT distinct(path) FROM cms_category where id=cat_id),"/",alias) WHERE path = '';

-------------------------------------------

update cms_archive set create_time=UNIX_TIMESTAMP(createdate),update_time=UNIX_TIMESTAMP(lastmodifydate)
where create_time <= 0;

ALTER TABLE `cms_archive` 
DROP COLUMN `lastmodifydate`,
DROP COLUMN `createdate`;

ALTER TABLE `cms_category` 
DROP COLUMN `rgt`,
DROP COLUMN `lft`;


update cms_archive set flag = 1 where flag= 0;

ALTER TABLE `cms_archive` DROP COLUMN `flags`;
