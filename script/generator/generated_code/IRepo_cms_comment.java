package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsCommentRepo {
    /** 获取 */
    CmsCommentEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsCommentEntity getCmsCommentBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsCommentEntity> selectCmsComment(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsComment(CmsCommentEntity v);
    /** 删除 */
    Error deleteCmsComment(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsComment(String where,Map<String,Object> params);
}
