package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsExtendFieldRepo {
    /** 获取 */
    CmsExtendFieldEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsExtendFieldEntity getCmsExtendFieldBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsExtendFieldEntity> selectCmsExtendField(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsExtendField(CmsExtendFieldEntity v);
    /** 删除 */
    Error deleteCmsExtendField(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsExtendField(String where,Map<String,Object> params);
}
