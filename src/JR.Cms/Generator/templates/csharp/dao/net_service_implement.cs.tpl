#!target:csharp/Service/{{title .table.Prefix}}/{{.table.Title}}ServiceImpl.cs
namespace {{pkg "csharp" .global.pkg}}.Service
{

using System;
using {{pkg "csharp" .global.pkg}}.Entity;
using {{pkg "csharp" .global.pkg}}.Repository;
using System.Collections.Generic;
using JR.Stand.Core.Extensions;

{{$tableTitle := .table.Title}}\
{{$shortTitle := .table.ShortTitle}}\
{{$entityName := join .table.Title .global.entity_suffix}}\
{{$pkName := .table.Pk}}\
{{$pkProp :=  .table.PkProp}}\
{{$pkType := type "csharp" .table.PkType}}

/// <summary>
/// {{.table.Comment}}服务
/// </summary>
public class {{.table.Title}}ServiceImpl : I{{.table.Title}}Service {
    
    private readonly I{{.table.Title}}Repository _repo;

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

    /// <summary>
    /// 保存{{.table.Comment}}
    /// </summary>
    public Error Save{{$shortTitle}}({{$entityName}} e){
         try
         {
            {{$entityName}} dst;
            {{if equal_any .table.PkType 3 4 5}}\
            if (e.{{$pkProp}} > 0) {
            {{else}}
            if (e.{{$pkProp}} != "") {
            {{end}}
                dst = this._repo.FindById(e.{{$pkProp}});
                if(dst == null)throw new Exception("no such data");
            } else {
                dst = {{$entityName}}.CreateDefault();
                {{$c := try_get .columns "create_time"}}\
                {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
                dst.CreateTime = TypeConv.toLong(Times.unix());
                {{else}}\
                dst.CreateTime = DateTime.Now;{{end}}{{end}}
            }\
            {{range $i,$c := exclude .columns $pkName "create_time" "update_time"}}
            dst.{{$c.Prop}} = e.{{$c.Prop}};{{end}}\
            {{$c := try_get .columns "update_time"}}\
            {{if $c}}{{if equal_any $c.Type 3 4 5 }}\
            dst.UpdateTime = TimeUtils.Unix();
            {{else}}\
            dst.UpdateTime = DateTime.Now;{{end}}{{end}}
            this._repo.Save(dst);
        }catch(Exception ex){
            return new Error((ex.InnerException ?? ex).Message);
        }
        return null;
    }

    /// <summary>
    /// 删除{{.table.Comment}}
    /// </summary>
    public int Delete{{$shortTitle}}ById({{$pkType}} {{$pkName}}) {
        return this._repo.DeleteById({{$pkName}});
    }

}
}
