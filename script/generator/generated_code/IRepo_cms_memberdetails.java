package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsMemberdetailsRepo {
    /** 获取 */
    CmsMemberdetailsEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsMemberdetailsEntity getCmsMemberdetailsBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsMemberdetailsEntity> selectCmsMemberdetails(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsMemberdetails(CmsMemberdetailsEntity v);
    /** 删除 */
    Error deleteCmsMemberdetails(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsMemberdetails(String where,Map<String,Object> params);
}
