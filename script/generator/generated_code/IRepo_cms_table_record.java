package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsTableRecordRepo {
    /** 获取 */
    CmsTableRecordEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsTableRecordEntity getCmsTableRecordBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsTableRecordEntity> selectCmsTableRecord(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsTableRecord(CmsTableRecordEntity v);
    /** 删除 */
    Error deleteCmsTableRecord(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsTableRecord(String where,Map<String,Object> params);
}
