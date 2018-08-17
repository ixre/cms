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
public class CmsReviewRepoImpl implements ICmsReviewRepo {
    /** 注入上下文 */
    @Inject XContext ctx;
    /** 获取 */
    public CmsReviewEntity get(Serializable id){
        TinySession s = this.ctx.hibernate();
        CmsReviewEntity e = s.get(CmsReviewEntity.class,id);
        return e;
    }
    /** 根据条件获取单条 */
    public CmsReviewEntity getCmsReviewBy(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        CmsReviewEntity e = s.get(CmsReviewEntity.class,where, params);
        s.close();
        return e;
    }

    /** 根据条件获取多条 */
    public List<CmsReviewEntity> selectCmsReview(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        List<CmsReviewEntity> list = s.select(CmsReviewEntity.class,
        "SELECT * FROM cms_review WHERE "+where, params);
        s.close();
        return list;
    }

    /** 保存 */
    public int saveCmsReview(CmsReviewEntity v){
        TinySession s = this.ctx.hibernate();
        s.save(v);
        s.close();
        return v.getId();
    }

    /** 删除 */
    public Error deleteCmsReview(Serializable id){
        TinySession s = this.ctx.hibernate();
        Map<String, Object> data = new HashMap<>();
        data.put("id", id);
        s.execute("DELETE FROM cms_review WHERE id=:id", data);
        s.close();
        return null;
    }

    /** 批量删除 */
    public int BatchDeleteCmsReview(String where,Map<String,Object> params){
        TinySession s = this.ctx.hibernate();
        int i = s.execute("DELETE FROM cms_review WHERE "+where, params);
        s.close();
        return i;
    }
}
