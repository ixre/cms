package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsUserRoleRepo {
    /** 获取 */
    CmsUserRoleEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsUserRoleEntity getCmsUserRoleBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsUserRoleEntity> selectCmsUserRole(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsUserRole(CmsUserRoleEntity v);
    /** 删除 */
    Error deleteCmsUserRole(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsUserRole(String where,Map<String,Object> params);
}
