package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsTplBindRepo {
    /** 获取 */
    CmsTplBindEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsTplBindEntity getCmsTplBindBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsTplBindEntity> selectCmsTplBind(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsTplBind(CmsTplBindEntity v);
    /** 删除 */
    Error deleteCmsTplBind(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsTplBind(String where,Map<String,Object> params);
}
