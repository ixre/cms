# 模板引擎文档 V1

## 调用方法
语法: `$[方法名]([参数])`, 调用示例如下:
```
$archives('cms/release','2')
```

```
#begin each_category(product,100,path,name,url)
    <div class="mod-prod-cats hidden-xs">
            <span class="mod-prod-cats-circle">
                <h3><a href="${url}" title="More ${name} product">${name}</a></h3>
            </span>
    </div>
#end
```

## 包含页面

```
${include:"include/header.html"}
```