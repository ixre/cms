package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsRelatedLinkRepo {
    /** 获取 */
    CmsRelatedLinkEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsRelatedLinkEntity getCmsRelatedLinkBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsRelatedLinkEntity> selectCmsRelatedLink(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsRelatedLink(CmsRelatedLinkEntity v);
    /** 删除 */
    Error deleteCmsRelatedLink(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsRelatedLink(String where,Map<String,Object> params);
}
