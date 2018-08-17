package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsTableRepo {
    /** 获取 */
    CmsTableEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsTableEntity getCmsTableBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsTableEntity> selectCmsTable(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsTable(CmsTableEntity v);
    /** 删除 */
    Error deleteCmsTable(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsTable(String where,Map<String,Object> params);
}
