#!target:csharp/Repository/{{title .table.Prefix}}/I{{.table.Title}}Repository.cs
using {{pkg "csharp" .global.pkg}}.Entity.{{.table.Title}}{{.global.entity_suffix}};
using System.Collections.Generic;
{{$entityName := join .table.Title .global.entity_suffix}}
namespace {{pkg "csharp" .global.pkg}}.Repository{
    {{$pkType := pk_type "csharp" .table.PkType}}
    /** {{.table.Comment}}仓储接口 */
    public interface I{{.table.Title}}Repository{

        /// <summary>
        /// 保存{{.table.Comment}}
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        {{$pkType}} Save({{$entityName}} e);
        
         /// <summary>
         /// 根据ID获取{{.table.Comment}}
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         {{$entityName}} FindById({{$pkType}} {{.table.Pk}});
          
         /// <summary>
         /// 根据条件查找{{.table.Comment}}
         /// </summary>
         /// <param name="where"></param>
         /// <returns></returns>
         {{$entityName}} FindBy(string where);   
         
         /// <summary>
         /// 获取所有{{.table.Comment}}
         /// </summary>
         /// <returns></returns>
         IList<{{$entityName}}> FindAll(); 
                      
         /// <summary>
         /// 删除{{.table.Comment}}
         /// </summary>
         int DeleteById({{$pkType}} {{.table.Pk}});
    }
}