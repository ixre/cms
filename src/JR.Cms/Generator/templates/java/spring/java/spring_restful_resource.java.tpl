#!target:spring/src/main/java/{{.global.pkg}}/restful/{{.table.Prefix}}/{{.table.Title}}Resource.java
package {{pkg "kotlin" .global.pkg}}.restful;

import {{pkg "kotlin" .global.pkg}}.entity.{{.table.Title}}{{.global.entity_suffix}};
import {{pkg "kotlin" .global.pkg}}.service.{{.table.Prefix}}.{{.table.Title}}Service;
import {{pkg "kotlin" .global.pkg}}.component.ReportComponent;
import net.fze.annotation.Resource;
import net.fze.common.Result;
import net.fze.extras.report.DataResult;
import net.fze.extras.report.ReportUtils;
import net.fze.extras.report.Params;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;
import javax.inject.Inject;
import java.util.List;
import java.util.ArrayList;

{{$tableTitle := .table.Title}}
{{$shortTitle := .table.ShortTitle}}
{{$pkType := type "kotlin" .table.PkType}}
{{$resPrefix := replace (name_path .table.Name) "/" ":"}}
{{$basePath := join .global.base_path (name_path .table.Name) "/"}}\

/* {{.table.Comment}}资源 */
@RestController
@RequestMapping("{{$basePath}}")
public class {{.table.Title}}Resource {
    @Inject private {{.table.Title}}Service service;
    @Inject private ReportComponent reportComponent;

    /** 获取{{.table.Comment}} */
    @GetMapping("/{id}")
    @Resource(key = "{{$resPrefix}}:get",name="获取{{.table.Comment}}")
    public {{.table.Title}}{{.global.entity_suffix}} get(@PathVariable("id") {{$pkType}} id){
        return this.service.find{{$shortTitle}}ById(id);
    }

    /** 创建{{.table.Comment}} */
    @PostMapping
    @Resource(key = "{{$resPrefix}}:create",name="创建{{.table.Comment}}")
    public Result create(@RequestBody {{.table.Title}}{{.global.entity_suffix}} entity){
        Error err = this.service.save{{$shortTitle}}(entity);
        if(err != null)return Result.create(1,err.getMessage());
        return Result.OK;
    }

    /** 更新{{.table.Comment}} */
    @PutMapping("/{id}")
    @Resource(key = "{{$resPrefix}}:update",name="更新{{.table.Comment}}")
    public Result update(@PathVariable("id") {{$pkType}} id,@RequestBody {{.table.Title}}{{.global.entity_suffix}} entity) {
        entity.set{{.table.PkProp}}(id);
        Error err = this.service.save{{$shortTitle}}(entity);
        if(err != null) return Result.create(1,err.getMessage());
        return Result.OK;
    }


    /** 删除{{.table.Comment}} */
    @DeleteMapping("/{id}")
    @Resource(key = "{{$resPrefix}}:delete",name="删除{{.table.Comment}}")
    public Result delete(@PathVariable("id") {{$pkType}} id){
        Error err = this.service.delete{{$shortTitle}}ById(id);
        if(err != null) return Result.create(1,err.getMessage());
        return Result.OK;
    }

    /** {{.table.Comment}}列表 */
    @GetMapping
    @Resource(key = "{{$resPrefix}}:list",name="查询{{.table.Comment}}")
    public List<{{.table.Title}}{{.global.entity_suffix}}> list(@RequestParam(name="params",defaultValue="{}") String params) {
        //val p = ReportUtils.parseParams(params).getValue();
        return this.service.findAll();
    }

    /** {{.table.Comment}}分页数据 */
    @GetMapping("/paging")
    @Resource(key = "{{$resPrefix}}:paging",name="查询{{.table.Comment}}分页数据")
    public DataResult paging(@RequestParam(name ="params",defaultValue = "{}", required = false) String params,
               @RequestParam("page") String page,
               @RequestParam("rows") String rows){
        Params p = ReportUtils.parseParams(params);
        return this.reportComponent.fetchData("default",
                "{{.table.Prefix}}/{{substr_n .table.Name "_" 1}}_list", p, page, rows);
    }
}
