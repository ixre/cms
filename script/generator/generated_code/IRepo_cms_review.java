package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsReviewRepo {
    /** 获取 */
    CmsReviewEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsReviewEntity getCmsReviewBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsReviewEntity> selectCmsReview(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsReview(CmsReviewEntity v);
    /** 删除 */
    Error deleteCmsReview(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsReview(String where,Map<String,Object> params);
}
