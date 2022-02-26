#!target:csharp/Repository/{{title .table.Prefix}}/{{.table.Title}}RepositoryImpl.cs
using {{pkg "csharp" .global.pkg}}.Entity.{{.table.Title}}{{.global.entity_suffix}};
using Dapper;
using System.Collections.Generic;
using JR.Stand.Core.Data.Provider;
using System.Data;

{{$entityName := join .table.Title .global.entity_suffix}}
{{$pkName := .table.Pk}}
{{$pkProp := .table.PkProp}}
{{$tableName := .table.Name}}
{{$columns := exclude .columns $pkName}}
{{$allColumns := .columns}}

namespace {{pkg "csharp" .global.pkg}}.Repository{
    {{$pkType := pk_type "csharp" .table.PkType}}
    /** {{.table.Comment}}仓储接口 */
    public class {{.table.Title}}RepositoryImpl : I{{.table.Title}}Repository{
    
        private readonly IDbProvider _provider;
        private readonly String _fieldAliases = @"
            {{range $i,$c := $allColumns}}{{$c.Name}} as {{$c.Prop}}{{if not (is_last $i $allColumns) }},{{end}}
            {{end}}";
    
        /// <summary>
        /// 创建仓储对象
        /// </summary>
        public {{.table.Title}}RepositoryImpl(IDbProvider provider)
        {
            this._provider = provider;
        }
             
        /// <summary>
        /// 保存{{.table.Comment}}
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public {{$pkType}} Save({{$entityName}} e)
        {
            using (IDbConnection db = _provider.GetConnection())
            {
                if (e.Id == 0)
                {
                    int i = db.Execute(_provider.FormatQuery(
                        @"INSERT INTO {{$tableName}}(
                           {{range $i,$c := $columns}}{{$c.Name}}{{if not (is_last $i $columns) }},{{end}}
                           {{end}}\
                        ) VALUES(
                          {{range $i,$c := $columns}}@{{$c.Prop}}{{if not (is_last $i $columns) }},{{end}}
                          {{end}}\
                        )"),
                    e);
                    return e.{{.table.PkProp}};
                }

                db.Execute(
                    _provider.FormatQuery(
                    @"UPDATE {{.table.Name}} SET 
                     {{range $i,$c := $columns}}{{$c.Name}} = @{{$c.Prop}}{{if not (is_last $i $columns) }},{{end}}
                     {{end}}\
                     WHERE {{$pkName}}=@{{$pkProp}}"),
                    e);
                return e.{{.table.PkProp}};
            }
        }

         /// <summary>
         /// 根据ID获取{{.table.Comment}}
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         public {{$entityName}} FindById({{$pkType}} {{.table.Pk}})
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                return db.QueryFirstOrDefault<{{$entityName}}>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM {{$tableName}} WHERE {{$pkName}} = @{{$pkProp}}"),
                    new {{$entityName}}{
                      {{$pkProp}} = {{$pkName}}, 
                    });
            }  
         }
         
         /// <summary>
         /// 根据条件查找{{.table.Comment}}
         /// </summary>
         /// <param name="where"></param>
         /// <returns></returns>
         public {{$entityName}} FindBy(string where)
         {
             using (IDbConnection db = _provider.GetConnection())
             {
                 return db.QueryFirstOrDefault<{{$entityName}}>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM {{$tableName}} WHERE ${where}"));
             }  
         }         
       
        /// <summary>
        /// 获取所有{{.table.Comment}}
        /// </summary>
        /// <returns></returns>
        public IList<{{$entityName}}> FindAll()
        {
           using (IDbConnection db = _provider.GetConnection())
           {
               return db.Query<{{$entityName}}>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM {{.table.Name}}")).AsList();
           } 
        }
              
         /// <summary>
         /// 删除{{.table.Comment}}
         /// </summary>
         public int DeleteById({{$pkType}} {{.table.Pk}})
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                 return db.Execute(
                     _provider.FormatQuery(
                         "DELETE FROM {{$tableName}} WHERE {{$pkName}} = @{{$pkProp}}"),
                     new {{$entityName}}{
                       {{$pkProp}} = {{$pkName}}, 
                     });
            }
         }
    }
}