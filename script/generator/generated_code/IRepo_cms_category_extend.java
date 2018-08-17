package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsCategoryExtendRepo {
    /** 获取 */
    CmsCategoryExtendEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsCategoryExtendEntity getCmsCategoryExtendBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsCategoryExtendEntity> selectCmsCategoryExtend(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsCategoryExtend(CmsCategoryExtendEntity v);
    /** 删除 */
    Error deleteCmsCategoryExtend(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsCategoryExtend(String where,Map<String,Object> params);
}
