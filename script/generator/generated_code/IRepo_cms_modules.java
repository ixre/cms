package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsModulesRepo {
    /** 获取 */
    CmsModulesEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsModulesEntity getCmsModulesBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsModulesEntity> selectCmsModules(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsModules(CmsModulesEntity v);
    /** 删除 */
    Error deleteCmsModules(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsModules(String where,Map<String,Object> params);
}
