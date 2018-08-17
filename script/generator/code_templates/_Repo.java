package com.gcy.sz.repo.impl;
// auto generate by gof (http://github.com/jsix/goex)
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.io.Serializable;
import com.google.inject.Inject;
import com.line.arch.component.XContext;
import com.line.arch.component.hibernate.TinySession;
import com.gcy.sz.repo.*;
import com.gcy.sz.repo.model.*;

/** {{.T.Comment}}仓储实现 */
public class {{.T.Title}}RepoImpl implements I{{.T.Title}}Repo {
    /** 注入上下文 */
    @Inject XContext ctx;
    /** 获取{{.T.Comment}} */
    public {{.T.Title}}Entity get(Serializable id){
        TinySession s = this.ctx.hibernate();
        {{.T.Title}}Entity e = s.get({{.T.Title}}Entity.class,id);
        return e;
    }
    /** 根据条件获取单条{{.T.Comment}} */
    public {{.T.Title}}Entity get{{.T.Title}}By(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        {{.T.Title}}Entity e = s.get({{.T.Title}}Entity.class,where, params);
        s.close();
        return e;
    }

    /** 根据条件获取多条{{.T.Comment}} */
    public List<{{.T.Title}}Entity> select{{.T.Title}}(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        List<{{.T.Title}}Entity> list = s.select({{.T.Title}}Entity.class,
        "SELECT * FROM {{.T.Name}} WHERE "+where, params);
        s.close();
        return list;
    }

    /** 保存{{.T.Comment}} */
    public int save{{.T.Title}}({{.T.Title}}Entity v){
        TinySession s = this.ctx.hibernate();
        s.save(v);
        s.close();
        return v.getId();
    }

    /** 删除{{.T.Comment}} */
    public Error delete{{.T.Title}}(Serializable id){
        TinySession s = this.ctx.hibernate();
        Map<String, Object> data = new HashMap<>();
        data.put("id", id);
        s.execute("DELETE FROM {{.T.Name}} WHERE id=:id", data);
        s.close();
        return null;
    }

    /** 批量删除{{.T.Comment}} */
    public int BatchDelete{{.T.Title}}(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        int i = s.execute("DELETE FROM {{.T.Name}} WHERE "+where, params);
        s.close();
        return i;
    }
}
