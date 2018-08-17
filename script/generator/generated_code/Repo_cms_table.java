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

/** 仓储实现 */
public class CmsTableRepoImpl implements ICmsTableRepo {
    /** 注入上下文 */
    @Inject XContext ctx;
    /** 获取 */
    public CmsTableEntity get(Serializable id){
        TinySession s = this.ctx.hibernate();
        CmsTableEntity e = s.get(CmsTableEntity.class,id);
        return e;
    }
    /** 根据条件获取单条 */
    public CmsTableEntity getCmsTableBy(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        CmsTableEntity e = s.get(CmsTableEntity.class,where, params);
        s.close();
        return e;
    }

    /** 根据条件获取多条 */
    public List<CmsTableEntity> selectCmsTable(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        List<CmsTableEntity> list = s.select(CmsTableEntity.class,
        "SELECT * FROM cms_table WHERE "+where, params);
        s.close();
        return list;
    }

    /** 保存 */
    public int saveCmsTable(CmsTableEntity v){
        TinySession s = this.ctx.hibernate();
        s.save(v);
        s.close();
        return v.getId();
    }

    /** 删除 */
    public Error deleteCmsTable(Serializable id){
        TinySession s = this.ctx.hibernate();
        Map<String, Object> data = new HashMap<>();
        data.put("id", id);
        s.execute("DELETE FROM cms_table WHERE id=:id", data);
        s.close();
        return null;
    }

    /** 批量删除 */
    public int BatchDeleteCmsTable(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        int i = s.execute("DELETE FROM cms_table WHERE "+where, params);
        s.close();
        return i;
    }
}
