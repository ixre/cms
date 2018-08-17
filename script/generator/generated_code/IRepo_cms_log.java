package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsLogRepo {
    /** 获取 */
    CmsLogEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsLogEntity getCmsLogBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsLogEntity> selectCmsLog(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsLog(CmsLogEntity v);
    /** 删除 */
    Error deleteCmsLog(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsLog(String where,Map<String,Object> params);
}
