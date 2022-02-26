#!target:csharp/Service/{{title .table.Prefix}}/I{{.table.Title}}Service.cs
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
public interface I{{.table.Title}}Service {
    
    /// <summary>
    /// 查找{{.table.Comment}}
    /// </summary>
    /// <returns></returns>
    {{$entityName}} Find{{$shortTitle}}ById({{$pkType}} {{$pkName}});

    /// <summary>
    /// 查找全部{{.table.Comment}}
    /// </summary>
    /// <returns></returns>
    IList<{{$entityName}}> FindAll{{$shortTitle}}();

    /// <summary>
    /// 保存{{.table.Comment}}
    /// </summary>
    Error Save{{$shortTitle}}({{$entityName}} e);

    /// <summary>
    /// 删除{{.table.Comment}}
    /// </summary>
    int Delete{{$shortTitle}}ById({{$pkType}} {{$pkName}});

}
}
