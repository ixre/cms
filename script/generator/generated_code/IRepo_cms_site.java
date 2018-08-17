package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsSiteRepo {
    /** 获取 */
    CmsSiteEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsSiteEntity getCmsSiteBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsSiteEntity> selectCmsSite(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsSite(CmsSiteEntity v);
    /** 删除 */
    Error deleteCmsSite(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsSite(String where,Map<String,Object> params);
}
