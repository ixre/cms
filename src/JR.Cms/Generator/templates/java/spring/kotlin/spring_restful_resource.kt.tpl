#!target:spring/src/main/kotlin/{{.global.pkg}}/restful/{{.table.Prefix}}/{{.table.Title}}Resource.kt
package {{pkg "kotlin" .global.pkg}}.restful

import {{pkg "kotlin" .global.pkg}}.entity.{{.table.Title}}{{.global.entity_suffix}}
import {{pkg "kotlin" .global.pkg}}.service.{{.table.Prefix}}.{{.table.Title}}Service
import {{pkg "kotlin" .global.pkg}}.component.ReportComponent
import net.fze.annotation.Resource
import net.fze.common.Result
import net.fze.extras.report.DataResult
import net.fze.extras.report.ReportUtils
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.web.bind.annotation.*
import javax.inject.Inject


{{$tableTitle := .table.Title}}
{{$shortTitle := .table.ShortTitle}}
{{$pkType := type "kotlin" .table.PkType}}
{{$resPrefix := replace (name_path .table.Name) "/" ":"}}
{{$basePath := join .global.base_path (name_path .table.Name) "/"}}\

/* {{.table.Comment}}资源 */
@RestController
@RequestMapping("{{$basePath}}")
class {{.table.Title}}Resource {
    @Inject private lateinit var service:{{.table.Title}}Service
    @Inject private lateinit var reportComponent: ReportComponent

    /** 获取{{.table.Comment}} */
    @GetMapping("/{id}")
    @Resource("{{$resPrefix}}:get",name="获取{{.table.Comment}}")
    fun get(@PathVariable("id") id:{{$pkType}}): {{.table.Title}}{{.global.entity_suffix}}? {
        return this.service.find{{$shortTitle}}ById(id)
    }

    /** 创建{{.table.Comment}} */
    @PostMapping
    @Resource("{{$resPrefix}}:create",name="创建{{.table.Comment}}")
    fun create(@RequestBody entity: {{.table.Title}}{{.global.entity_suffix}}):Result {
        val err = this.service.save{{$shortTitle}}(entity)
        if(err != null)return Result.create(1,err.message)
        return Result.OK
    }

    /** 更新{{.table.Comment}} */
    @PutMapping("/{id}")
    @Resource("{{$resPrefix}}:update",name="更新{{.table.Comment}}")
    fun update(@PathVariable("id") id:{{$pkType}},@RequestBody entity: {{.table.Title}}{{.global.entity_suffix}}):Result {
        entity.{{lower_title .table.PkProp}} = id
        val err = this.service.save{{$shortTitle}}(entity)
        if(err != null)return Result.create(1,err.message)
        return Result.OK
    }


    /** 删除{{.table.Comment}} */
    @DeleteMapping("/{id}")
    @Resource("{{$resPrefix}}:delete",name="删除{{.table.Comment}}")
    fun delete(@PathVariable("id") id:{{$pkType}}):Result {
        val err = this.service.delete{{$shortTitle}}ById(id)
        if(err != null)return Result.create(1,err.message)
        return Result.OK
    }

    /** {{.table.Comment}}列表 */
    @GetMapping
    @Resource("{{$resPrefix}}:list",name="查询{{.table.Comment}}")
    fun list(@RequestParam("params") params:String="{}"): List<{{.table.Title}}{{.global.entity_suffix}}> {
        //val p = ReportUtils.parseParams(params).getValue()
        return this.service.findAll()
    }

    /** {{.table.Comment}}分页数据 */
    @GetMapping("/paging")
    @Resource("{{$resPrefix}}:paging",name="查询{{.table.Comment}}分页数据")
    fun paging(@RequestParam("params",required = false) params:String="{}"
               @RequestParam("page") page:String,
               @RequestParam("rows") rows:String
    ): DataResult {
        val p = ReportUtils.parseParams(params)
        return this.reportComponent.fetchData("default",
                "{{.table.Prefix}}/{{substr_n .table.Name "_" 1}}_list", p, page, rows)
    }
}
