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
public class CmsCategoryRepoImpl implements ICmsCategoryRepo {
    /** 注入上下文 */
    @Inject XContext ctx;
    /** 获取 */
    public CmsCategoryEntity get(Serializable id){
        TinySession s = this.ctx.hibernate();
        CmsCategoryEntity e = s.get(CmsCategoryEntity.class,id);
        return e;
    }
    /** 根据条件获取单条 */
    public CmsCategoryEntity getCmsCategoryBy(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        CmsCategoryEntity e = s.get(CmsCategoryEntity.class,where, params);
        s.close();
        return e;
    }

    /** 根据条件获取多条 */
    public List<CmsCategoryEntity> selectCmsCategory(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        List<CmsCategoryEntity> list = s.select(CmsCategoryEntity.class,
        "SELECT * FROM cms_category WHERE "+where, params);
        s.close();
        return list;
    }

    /** 保存 */
    public int saveCmsCategory(CmsCategoryEntity v){
        TinySession s = this.ctx.hibernate();
        s.save(v);
        s.close();
        return v.getId();
    }

    /** 删除 */
    public Error deleteCmsCategory(Serializable id){
        TinySession s = this.ctx.hibernate();
        Map<String, Object> data = new HashMap<>();
        data.put("id", id);
        s.execute("DELETE FROM cms_category WHERE id=:id", data);
        s.close();
        return null;
    }

    /** 批量删除 */
    public int BatchDeleteCmsCategory(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        int i = s.execute("DELETE FROM cms_category WHERE "+where, params);
        s.close();
        return i;
    }
}
