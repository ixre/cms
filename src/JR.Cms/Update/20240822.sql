-- 文章添加定时发布字段
ALTER TABLE `cms_archive` ADD COLUMN `schedule_time` INT(10) NOT NULL DEFAULT 0 COMMENT '定时发布时间' AFTER `thumbnail`;