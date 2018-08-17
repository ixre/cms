<!DOCTYPE html>
<html>
<head>
    <title>{{.T.Comment}}列表</title>
    <link rel="stylesheet" href="/widget/easyui/themes/gray/easyui.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/easyui/themes/icon.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/mui/base.css" type="text/css" />
    <link rel="stylesheet" href="/widget/mui/themes/default/page.css" type="text/css" />
    <link rel="stylesheet" href="/css/own/own_page.css" type="text/css" />
</head>
<body>

<div id="dg"></div>
<div class="gra-toolbar expo-toolbar toolBar">
    <div class="expo-param-fields clearfix">
        <ul>
            <li>
                <a class="gra-btn gra-btn-fn btn-add">
                    <i class="fa fa-plus" aria-hidden="true"></i>创建{{.T.Comment}}</a>
            </li>
            <li>
                <span class="title">关键字：</span>
                <input type="text" field="keyword"/>
            </li>
            <li>
                <span class="title">排序方式：</span>
                <select field="orderBy">
                    <option value="id DESC">默认排序</option>
                </select>
            </li>
            <li>
                <input type="hidden" field="where" value="$${Where}"/>
                <a class="gra-btn gra-btn-fn btn-search">
                    <i class="fa fa-search" aria-hidden="true"></i>搜索</a>
            </li>

        </ul>

        <div class="gra-toolbar-right">
            <div class="gra-btn btn-export">导出</div>
        </div>
    </div>
</div>

<template id="tpl_op">
    <a class="gra-btn gra-btn-a" href="javascript:void(0)" onclick="_edit('{id}')">编辑</a>
    <!--<a class="gra-btn gra-btn-a" href="javascript:void(0)" onclick="_list('{id}')">列表</a>-->
    <a class="gra-btn gra-btn-a" href="javascript:void(0)" onclick="_del('{id}')">删除</a>
</template>

<script type="text/javascript" src="/js/base.js"></script>
<script type="text/javascript">
    require(["/js/own/require_config.js"], function () {
        require(["base", "jquery.easyui.zh", "extra/export","extra/util"], pageLoad);
    });

    var flag = 1;
    function pageLoad(_) {
        expo.portal = "{{.T.Prefix}}/{{.T.Title}}List";
        var opHtml = $b.html("tpl_op");
        $("#dg").datagrid({
            toolbar: ".toolBar",
            singleSelect: !false,
            pagination: true,
            rownumbers: true,
            fitColumns: true,
            url: expo.getDataUrl(),
            columns: [
                [

        {{range $i,$c := .T.Columns}}
            {field: "{{$c.Name}}", title: "{{$c.Comment}}", align:"center", width: 100},
        {{end}}
                    {
                        field: "-", title:"操作", align:"center", width: 120, formatter: function (val, row) {
                        return $b.template(opHtml, row);
                    }
                    },
                ]
            ]
        });
        $b.$fn(".btn-search").click(_search);
        $b.$fn(".btn-add").click(_create);
        $b.$fn(".btn-export").click(function(){
            expo.showExportDialog();
        });
    }

    function _create() {
        var d = $b.dialog.create2("添加{{.T.Comment}}", true, true);
        d.open("create{{.T.Title}}", 600, 400);
        // $b.tab.open("创建{{.T.Comment}}", "create{{.T.Title}}", true);
    }
    
    function _search() {
        expo.search("dg");
    }

    function _edit(id) {
        var d = $b.dialog.create2("修改{{.T.Comment}}", true, true);
        d.open("edit{{.T.Title}}?id=" + id, 600, 400);
        // $b.tab.open("修改{{.T.Comment}}", "edit{{.T.Title}}?id="+id, true);
    }

    <!--
    function _list(id){
        $b.tab.open("{{.T.Comment}}", "List?group_id="+id, true);
    }
    -->

    function _del(id) {
        $b.dialog.confirm("删除后不可恢复，您确定要继续吗？", function (r) {
            if (!r) {return;}
            if(flag) {
                flag = 0;
                $b.xhr.jsonPost("del{{.T.Title}}", {id: id}, function (r) {
                    flag = 1;
                    if (!r.errCode) {
                        window.refresh()
                    } else {
                        $b.dialog.alert(r.message);
                    }
                });
            }
        });
    }
</script>

</body>
</html>