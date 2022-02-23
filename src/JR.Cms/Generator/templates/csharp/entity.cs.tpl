#!target:csharp/Entity/{{.table.Title}}{{.global.entity_suffix}}.cs

using System;
using System.Collections.Generic;

namespace {{pkg "csharp" .global.pkg}}.Entity
{
/// <summary>
/// {{.table.Comment}}({{.table.Name}})
/// </summary>
public class {{.table.Title}}{{.global.entity_suffix}} 
{
    {{range $i,$c := .columns}}{{$type := type "csharp" $c.Type}}
    /// <summary>
    /// {{$c.Comment}}
    /// </summary>
    public {{$type}} {{$c.Prop}}{get;set;}
    {{end}}
    /// <summary>
    /// 创建深拷贝
    /// </summary>
    /// <returns></returns>
    public {{.table.Title}}{{.global.entity_suffix}} Copy()
    {
        return new {{.table.Title}}{{.global.entity_suffix}}
        {
            {{range $i,$c := .columns}}
            {{$c.Prop}} = this.{{$c.Prop}},{{end}}
        };
    }

    /// <summary>
    /// 转换为MAP
    /// </summary>
    /// <returns></returns>
    public IDictionary<string,object> ToMap()
    {
        return new Dictionary<string,object>
        {
        {{range $i,$c := .columns}}
            {"{{$c.Prop}}",this.{{$c.Prop}}},{{end}}
        };
    }

    /// <summary>
    /// 使用默认值创建实例 
    /// </summary>
    /// <returns></returns>
    public static {{.table.Title}}{{.global.entity_suffix}} CreateDefault(){
        return new {{.table.Title}}{{.global.entity_suffix}}\
        {
            {{range $i,$c := .columns}}
            {{$c.Prop}} = {{default "csharp" $c.Type}},{{end}}
        };
    }
}
}