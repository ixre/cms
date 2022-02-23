#!target:spring/src/main/java/{{.global.pkg}}/service/{{.table.Prefix}}/{{.table.Title}}Service.java
package {{pkg "java" .global.pkg}}.service;

import {{pkg "java" .global.pkg}}.entity.{{.table.Title}}{{.global.entity_suffix}};
import {{pkg "java" .global.pkg}}.repo.{{.table.Prefix}}.{{.table.Title}}JpaRepository;
import org.springframework.stereotype.Service;
import net.fze.common.Standard;
import net.fze.util.Times;
import net.fze.util.TypeConv;
import javax.inject.Inject;
import java.util.List;
{{$tableTitle := .table.Title}}\
{{$shortTitle := .table.ShortTitle}}\
{{$pkName := .table.Pk}}\
{{$pkProp :=  .table.PkProp}}\
{{$pkType := type "kotlin" .table.PkType}}
/** {{.table.Comment}}服务  */
@Service("{{.table.Name}}_service")
public class {{.table.Title}}Service {
    @Inject
    private {{$tableTitle}}JpaRepository repo;

    /** 查找{{.table.Comment}} */
    public {{$tableTitle}}{{.global.entity_suffix}} find{{$shortTitle}}ById({{$pkType}} id){
        return this.repo.findById(id).orElse(null);
    }

    /** 查找全部{{.table.Comment}} */
    public List<{{$tableTitle}}{{.global.entity_suffix}}> findAll() {
        return this.repo.findAll();
    }

    /** 保存{{.table.Comment}} */
    public Error save{{$shortTitle}}({{$tableTitle}}{{.global.entity_suffix}} e){
         return Standard.std.tryCatch(()-> {
            {{$tableTitle}}{{.global.entity_suffix}} dst;
            {{if equal_any .table.PkType 3 4 5}}\
            if (e.get{{$pkProp}}() > 0) {
            {{else}}
            if (e.get{{$pkProp}}() != "") {
            {{end}}
                dst = this.repo.findById(e.get{{$pkProp}}()).orElse(null);
                if(dst == null)throw new IllegalArgumentException("");
            } else {
                dst = {{$tableTitle}}{{.global.entity_suffix}}.createDefault();
                {{$c := try_get .columns "create_time"}}\
                {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
                dst.setCreateTime(TypeConv.toLong(Times.unix()));
                {{else}}\
                dst.setCreateTime(new java.util.Date());{{end}}{{end}}
            }\
            {{range $i,$c := exclude .columns $pkName "create_time" "update_time"}}
            dst.set{{$c.Prop}}(e.get{{$c.Prop}}());{{end}}\
            {{$c := try_get .columns "update_time"}}
            {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
            dst.setUpdateTime(TypeConv.toLong(Times.unix()));
            {{else}}\
            dst.setUpdateTime(new java.util.Date());{{end}}{{end}}
            this.repo.save(dst);
            return null;
        }).error();
    }


    /** 批量保存{{.table.Comment}} */
    public Iterable<{{$tableTitle}}{{.global.entity_suffix}}> saveAll{{$shortTitle}}(Iterable<{{$tableTitle}}{{.global.entity_suffix}}> entities){
        return this.repo.saveAll(entities);
    }

    /** 删除{{.table.Comment}} */
    public Error delete{{$shortTitle}}ById({{$pkType}} id) {
         return Standard.std.tryCatch(()-> {
             this.repo.deleteById(id);
             return null;
         }).error();
    }

}
