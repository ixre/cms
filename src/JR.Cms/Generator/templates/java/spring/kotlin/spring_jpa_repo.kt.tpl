#!target:spring/src/main/kotlin/{{.global.pkg}}/repo/{{.table.Prefix}}/{{.table.Title}}JpaRepository.kt
package {{pkg "java" .global.pkg}}.repo.{{.table.Prefix}}

import {{pkg "kotlin" .global.pkg}}.entity.{{.table.Title}}{{.global.entity_suffix}}
import org.springframework.data.jpa.repository.JpaRepository
{{$pkType := pk_type "kotlin" .table.PkType}}
/** {{.table.Comment}}仓储接口 */
interface {{.table.Title}}JpaRepository : JpaRepository<{{.table.Title}}{{.global.entity_suffix}}, {{$pkType}}> {

}
