package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsTableRowRepo {
    /** 获取 */
    CmsTableRowEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsTableRowEntity getCmsTableRowBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsTableRowEntity> selectCmsTableRow(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsTableRow(CmsTableRowEntity v);
    /** 删除 */
    Error deleteCmsTableRow(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsTableRow(String where,Map<String,Object> params);
}
