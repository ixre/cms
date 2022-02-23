# 数据查询组件

文件(JAVA版):`ReportComponent.java`

```java
import net.fze.common.Standard;
import net.fze.extras.report.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.sql.Connection;
import javax.sql.DataSource;

@Component
public class ReportComponent implements IDbProvider {
    private final HashMap<String, ExportHub> exportHubMap = new HashMap<>();

    private final DataSource ds;

    public ReportComponent(@Inject DataSource ds) {
        this.ds = ds;
    }

    @NotNull
    @Override
    public Connection getDB() {
        try {
            return this.ds.getConnection();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        throw new Error("can't get any database connection");
    }


    private void lazyInit() {
        String rootPath = "/query";
        exportHubMap.put("default",
                new ExportHub(
                        this,
                        rootPath + "/default@query", !Standard.dev()
                ));
    }


    private ExportHub getHub(String key) {
        if (this.exportHubMap.isEmpty()) {
            this.lazyInit();
        }
        if (key.isEmpty()) return exportHubMap.get("default");
        return exportHubMap.get(key);
    }

    private Params parseParams(String params) {
        return ReportUtils.parseParams(params);
    }

    public DataResult fetchData(String key, String portal, Params params, String page, String rows) {
        ExportHub hub = this.getHub(key);
        if (hub == null) throw new Error("datasource not exists");
        return hub.fetchData(portal, params, page, rows);
    }
}
```

文件(Kotlin版):`ReportComponent.kt`

```kotlin
import net.fze.common.Standard
import net.fze.extras.report.*
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.stereotype.Component
import java.sql.Connection
import javax.sql.DataSource

@Component
class ReportComponent : IDbProvider {
    private val exportHubMap: MutableMap<String, ExportHub> = mutableMapOf()
    private val rootPath = "/query"

    @Inject
    private var ds: DataSource? = null

    override fun getDB(): Connection {
        return try {
            ds!!.connection
        } catch (ex: Exception) {
            ex.printStackTrace()
            throw Error(ex.message)
        }
    }

    private fun lazyInit() {
        exportHubMap["default"] = ExportHub(
            this,
            "$rootPath/default@query", !Standard.dev()
        )
    }

    private fun getHub(key: String): ExportHub? {
        if (this.exportHubMap.isEmpty()) {
            this.lazyInit()
        }
        if (key.isEmpty()) return exportHubMap["default"]
        return exportHubMap[key]
    }

    fun parseParams(params: String): Params {
        return ReportUtils.parseParams(params)
    }

    fun fetchData(key: String, portal: String, params: Params, page: String, rows: String): DataResult {
        val hub = this.getHub(key) ?: throw Exception("datasource not exists")
        return hub.fetchData(portal, params, page, rows)
    }
}
```