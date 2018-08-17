package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsUserRepo {
    /** 获取 */
    CmsUserEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsUserEntity getCmsUserBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsUserEntity> selectCmsUser(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsUser(CmsUserEntity v);
    /** 删除 */
    Error deleteCmsUser(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsUser(String where,Map<String,Object> params);
}
