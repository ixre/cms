package com.gcy.sz.repo;
// auto generate by gof (http://github.com/jsix/goex)
import java.io.Serializable;
import java.util.List;
import java.util.Map;
import com.gcy.sz.repo.model.*;

/** 仓储 */
public interface ICmsCredentialRepo {
    /** 获取 */
    CmsCredentialEntity get(Serializable id);
    /** 根据条件获取单条 */
    CmsCredentialEntity getCmsCredentialBy(String where,Map<String,Object> params);
    /** 根据条件获取多条 */
    List<CmsCredentialEntity> selectCmsCredential(String where,Map<String,Object> params);
    /** 保存 */
    int saveCmsCredential(CmsCredentialEntity v);
    /** 删除 */
    Error deleteCmsCredential(Serializable id);
    /** 批量删除 */
    int BatchDeleteCmsCredential(String where,Map<String,Object> params);
}
