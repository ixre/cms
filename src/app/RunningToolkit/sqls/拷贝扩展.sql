INSERT INTO lms_extendValue(relationId,relationType,fieldId,fieldValue)
SELECT id as relationId, 1 as relationType,1+4 as fieldId,JR.fieldValue FROM
(select a.title,(@rownum:=@rownum+1) as rowNum,a.id from lms_archives a 
INNER JOIN lms_categories c ON a.cid = c.id 
WHERE lft>=24 and rgt<=39 and c.siteid=4 order by id asc) t
INNER JOIN (select a.title,fieldValue from lms_extendValue v
INNER JOIN lms_archives a ON v.relationId = a.id
INNER JOIN lms_categories c ON a.cid = c.id 
where relationType=1 and fieldId=1 and lft>=24 and rgt<=39 and c.siteid=1  order by relationId asc) JR
 ON t.title=JR.title
