<!DOCTYPE html>
<html>
<head>
    <title>{{.table.Comment}}详情</title>
    #!target:spring/src/main/java/{{.global.pkg}}/resources/templates/admin/{{.table.Prefix}}/{{.table.Name}}_details.ftl
    <link rel="stylesheet" href="/assets/css/base.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/mui/base.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/mui/themes/default/page.css" type="text/css"/>
    <link rel="stylesheet" href="/css/own/own_page.css" type="text/css"/>
</head>
<body>

<div id="form1" class="margin30">
    <input type="hidden" field="{{.table.Pk}}" value="{{default "go" .table.PkType}}"/>
    <div class="container">
        {{.table.Comment}}详情
        <div class="gra-hr"></div>
    </div>
    <div class="container form">
        {{range $i,$c := .columns}}{{if not $c.IsPk}}
        <div class="gra-form-field col-md-6">
            <div class="gra-form-label">{{$c.Comment}}：</div>
            <div class="gra-form-col">
                <span field="{{$c.Name}}"></span>
            </div>
        </div>
        {{end}}{{end}}
    </div>
    <div class="gra-form-field col-md-6">
        <div class="gra-form-label">&nbsp;</div>
        <div class="gra-form-col">
            <div class="gra-btn gra-btn-inline btn-submit">确认订单</div>
        </div>
    </div>
</div>

<script type="text/javascript" src="/assets/js/base.js"></script>
<script type="text/javascript">
    var entity = $${EntityJSON};
    var flag = 1;
    var id = entity["{{.table.Pk}}"];
    require(["/assets/js/super/require_config.js"], function () {
        require(['base',"extra/util"], pageLoad);
    });

    function pageLoad(_) {
        {{range $i,$c := .columns}}
        {{if ends_with $c.Name "_time"}}
        entity["{{$c.Name}}"] = utils.unix2str(entity["{{$c.Name}}"]);
        {{end}}
        {{if ends_with $c.Name "_amount"}}
        entity["{{$c.Name}}"] = utils.formatMoney(entity["{{$c.Name}}"]);
        {{end}}
        {{end}}
        $b.json.bind('form1', entity);

        $b.$fn(".btn-submit").click(function(){
            if($b.dialog.confirm("操作确认消息..",function(r){
                if(r) {
                    if (!flag) return false;
                    flag = 0;
                    $b.xhr.jsonPost("save{{.table.Title}}", {"{{.table.Pk}}":id}, function (r) {
                        flag = 1;
                        if (!r["errCode"]) {
                            $b.dialog.alert('操作成功', function () {
                                $b.dialog.close();
                            });
                        } else {
                            $b.dialog.alert(r["errMsg"]);
                        }
                    });
                }
            }));
        });
    }

</script>
</body>
</html>
