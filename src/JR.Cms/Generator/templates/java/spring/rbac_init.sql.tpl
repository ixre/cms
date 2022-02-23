#!kind:1

/**
该模板生成权限系统的SQL脚本

# 变量
VUE_PREFIX: 如:bz/
*/

/** #! INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_) */
/** #! VALUES ('注册表', 1, 1, 'registry', '/registry/index', 'hamburger', '', 0, 0, 0, 1607214871, '_features/registry/index', ''); */

{{$prefix := env "VUE_PREFIX"}}
{{$parent := "autogen"}}

/** 三级菜单才会用到这个层级 */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('{{$parent}}', 0, 0, '', '{{$parent}}', 'menu', '', 0, 0, 0, 1607214871, '', '');


{{range $k,$tables :=  .groups}}
{{if ne $k "perm"}}

/* ====== insert data : {{$k}} ===== */

INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('{{$k}}', 0, (SELECT DISTINCT id FROM perm_res WHERE name='{{$parent}}' LIMIT 1), '{{$k}}', '/{{$k}}/index', 'hamburger', '', 0, 0, 0, 1607214871, '', '');

{{range $i,$table := $tables}}
   /* {{$table.Comment}}前端页面 */
   INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
   VALUES ('{{$table.Comment}}', 2, (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}', '{{$prefix}}{{name_path $table.Name}}/index', 'hamburger', '',
    0, 0, 0, 1607214871, '{{$table.Title}}Index', '');

   /* 新增{{$table.Comment}} */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('新增{{$table.Comment}}(接口)', 1,  (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}:create', '', 'create', '',0, 0, 0, 0, '', '');

/* 更新{{$table.Comment}} */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('更新{{$table.Comment}}(接口)', 1,  (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}:update', '', 'update', '',0, 0, 0, 0, '', '');

/* 查询{{$table.Comment}} */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('查询{{$table.Comment}}(接口)', 1, (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}:get', '', 'update', '',0, 0, 0, 0, '', '');

/* 查询{{$table.Comment}} */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('{{$table.Comment}}列表(接口)', 1,  (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}:list', '', 'update', '',0, 0, 0, 0, '', '');

/* 删除{{$table.Comment}} */
INSERT INTO public.perm_res (name, res_type, pid, key, path, icon, permission, sort_num, is_external, is_hidden, create_time, component_name, cache_)
VALUES ('删除{{$table.Comment}}(接口)', 1,  (SELECT DISTINCT id FROM perm_res WHERE name='{{$k}}' LIMIT 1),
    '{{replace (name_path $table.Name) "/" ":"}}:delete', '', 'update', '',0, 0, 0, 0, '', '');

{{end}}
{{end}}
{{end}}