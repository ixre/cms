package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsLinkRepo {
    /** 获取 */
    CmsLinkEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsLinkEntity getCmsLinkBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsLinkEntity> selectCmsLink(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsLink(CmsLinkEntity v);
    /** 删除 */
    Error deleteCmsLink(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsLink(String where,Map<String,Object> params);
}
