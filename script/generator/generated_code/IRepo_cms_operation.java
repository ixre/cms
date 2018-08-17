package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsOperationRepo {
    /** 获取 */
    CmsOperationEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsOperationEntity getCmsOperationBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsOperationEntity> selectCmsOperation(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsOperation(CmsOperationEntity v);
    /** 删除 */
    Error deleteCmsOperation(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsOperation(String where,Map<String,Object> params);
}
