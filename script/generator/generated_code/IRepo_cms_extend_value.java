package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsExtendValueRepo {
    /** 获取 */
    CmsExtendValueEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsExtendValueEntity getCmsExtendValueBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsExtendValueEntity> selectCmsExtendValue(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsExtendValue(CmsExtendValueEntity v);
    /** 删除 */
    Error deleteCmsExtendValue(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsExtendValue(String where,Map<String,Object> params);
}
