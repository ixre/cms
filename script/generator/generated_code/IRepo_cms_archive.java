package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsArchiveRepo {
    /** 获取 */
    CmsArchiveEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsArchiveEntity getCmsArchiveBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsArchiveEntity> selectCmsArchive(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsArchive(CmsArchiveEntity v);
    /** 删除 */
    Error deleteCmsArchive(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsArchive(String where,Map<String,Object> params);
}
