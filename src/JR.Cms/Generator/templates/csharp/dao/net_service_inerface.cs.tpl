#!target:csharp/Service/{{.table.Prefix}}/I{{.table.Title}}Service.cs
namespace {{pkg "csharp" .global.pkg}}.Service
{

using {{pkg "csharp" .global.pkg}}.Entity;
using {{pkg "csharp" .global.pkg}}.Repository;

{{$tableTitle := .table.Title}}\
{{$shortTitle := .table.ShortTitle}}\
{{$entityName := join .table.Title .global.entity_suffix}}\
{{$pkName := .table.Pk}}\
{{$pkProp :=  .table.PkProp}}\
{{$pkType := type "kotlin" .table.PkType}}

/// <summary>
/// {{.table.Comment}}服务
/// </summary>
public class {{.table.Title}}ServiceImpl {
    
    private I{{.table.Title}}Repository _repo;

    /// <summary>
    /// 创建服务实例
    /// </summary>
    /// <param name="repo">仓储</param>
    public {{.table.Title}}ServiceImpl(I{{.table.Title}}Repository repo)
    {
        this._repo = repo;
    }
    
    /// <summary>
    /// 查找{{.table.Comment}}
    /// </summary>
    /// <returns></returns>
    public {{$entityName}} Find{{$shortTitle}}ById({{$pkType}} {{$pkName}}){
        return this._repo.FindById({{$pkName}});
    }

    /// <summary>
    /// 查找全部{{.table.Comment}}
    /// </summary>
    /// <returns></returns>
    public IList<{{$entityName}}> FindAll{{$shortTitle}}() {
        return this._repo.FindAll();
    }        

    /** 保存{{.table.Comment}} */
    public Error Save{{$shortTitle}}({{$entityName}} e){
         try
         {
            {{$entityName}} dst;
            {{if equal_any .table.PkType 3 4 5}}\
            if (e.{{$pkProp}} > 0) {
            {{else}}
            if (e.{{$pkProp}} != "") {
            {{end}}
                dst = this._repo.FindById(e.{{$pkProp}})
                if(dst == null)throw new IllegalArgumentException("no such data");
            } else {
                dst = {{$entityName}}.CreateDefault();
                {{$c := try_get .columns "create_time"}}\
                {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
                dst.CreateTime = TypeConv.toLong(Times.unix());
                {{else}}\
                dst.CreateTime(DateTime.Now);{{end}}{{end}}
            }\
            {{range $i,$c := exclude .columns $pkName "create_time" "update_time"}}
            dst.set{{$c.Prop}}(e.get{{$c.Prop}}());{{end}}\
            {{$c := try_get .columns "update_time"}}
            {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
            dst.setUpdateTime(TypeConv.toLong(Times.unix()));
            {{else}}\
            dst.setUpdateTime(new java.util.Date());{{end}}{{end}}
            this.repo.save(dst);
        }catch(Exception ex){
        }
        return null;
    }


    /** 批量保存{{.table.Comment}} */
    public Iterable<{{$entityName}}> saveAll{{$shortTitle}}(Iterable<{{$entityName}}> entities){
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
}
