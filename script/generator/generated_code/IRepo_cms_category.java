package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsCategoryRepo {
    /** 获取 */
    CmsCategoryEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsCategoryEntity getCmsCategoryBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsCategoryEntity> selectCmsCategory(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsCategory(CmsCategoryEntity v);
    /** 删除 */
    Error deleteCmsCategory(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsCategory(String where,Map<String,Object> params);
}
