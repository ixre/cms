package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsMemberRepo {
    /** 获取 */
    CmsMemberEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsMemberEntity getCmsMemberBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsMemberEntity> selectCmsMember(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsMember(CmsMemberEntity v);
    /** 删除 */
    Error deleteCmsMember(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsMember(String where,Map<String,Object> params);
}
