#!target:spring/src/main/java/{{.global.pkg}}/repo/{{.table.Prefix}}/{{.table.Title}}JpaRepository.java
package {{pkg "java" .global.pkg}}.repo.{{.table.Prefix}};

import {{pkg "java" .global.pkg}}.entity.{{.table.Title}}{{.global.entity_suffix}};
import org.springframework.data.jpa.repository.JpaRepository;
{{$pkType := pk_type "java" .table.PkType}}
/** {{.table.Comment}}仓储接口 */
public interface {{.table.Title}}JpaRepository extends JpaRepository<{{.table.Title}}{{.global.entity_suffix}}, {{$pkType}}>{

}
