package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** {{.T.Comment}}仓储 */
public interface I{{.T.Title}}Repo {
    /** 获取{{.T.Comment}} */
    {{.T.Title}}Entity get(Serializable id);
    /** 根据条件获取单条{{.T.Comment}} */
    {{.T.Title}}Entity get{{.T.Title}}By(String where,Map<String,Object> params);
    /** 根据条件获取多条{{.T.Comment}} */
    List<{{.T.Title}}Entity> select{{.T.Title}}(String where,Map<String,Object> params);
    /** 保存{{.T.Comment}} */
    int save{{.T.Title}}({{.T.Title}}Entity v);
    /** 删除{{.T.Comment}} */
    Error delete{{.T.Title}}(Serializable id);
    /** 批量删除{{.T.Comment}} */
    int BatchDelete{{.T.Title}}(String where,Map<String,Object> params);
}
