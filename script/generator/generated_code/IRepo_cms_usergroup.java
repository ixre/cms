package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsUsergroupRepo {
    /** 获取 */
    CmsUsergroupEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsUsergroupEntity getCmsUsergroupBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsUsergroupEntity> selectCmsUsergroup(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsUsergroup(CmsUsergroupEntity v);
    /** 删除 */
    Error deleteCmsUsergroup(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsUsergroup(String where,Map<String,Object> params);
}
