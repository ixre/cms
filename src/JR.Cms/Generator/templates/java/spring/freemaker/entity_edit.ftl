<!DOCTYPE html>
#!target:spring/src/main/java/{{.global.pkg}}/resources/templates/admin/{{.table.Prefix}}/edit_{{.table.Name}}.ftl
<html>
<head>
    <title>{{.table.Comment}}</title>
    <link rel="stylesheet" href="/assets/css/base.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/mui/base.css" type="text/css"/>
    <link rel="stylesheet" href="/widget/mui/themes/default/page.css" type="text/css"/>
    <link rel="stylesheet" href="/css/own/own_page.css" type="text/css"/>
</head>
<body>

<form action="" method="post" class="gra-form" id="form1">
    {{range $i,$c := .T.Columns}}{{if $c.IsPk}}
    <input type="hidden" field="{{$c.Name}}" name="{{$c.Name}}" value="0"/>{{else}}
    <div class="gra-form-field">{{if $c.NotNull}}
        <div class="gra-form-label"><span class="red">*&nbsp;</span>{{$c.Comment}}：</div>
        <div class="gra-form-col">
            <input type="text" field="{{$c.Name}}" name="{{$c.Name}}" class="ui-validate"/>
        </div>
        {{else}}
        <div class="gra-form-label">{{$c.Comment}}：</div>
        <div class="gra-form-col">
            <input type="text" field="{{$c.Name}}" name="{{$c.Name}}" class="ui-validate"/>
        </div>
        {{end}}
    </div>
    {{end}}
    {{end}}


    <div class="gra-form-field">
        <div class="gra-form-label">&nbsp;</div>
        <div class="gra-form-col">
            <div class="gra-btn gra-btn-inline btn-submit">提交</div>
            <div class="gra-btn gra-btn-inline btn-reset">重置</div>
        </div>
    </div>

</form>

<script type="text/javascript" src="/assets/js/base.js"></script>
<script type="text/javascript">
    var entity = $${EntityJSON};
    var form_id = "form1";
    require(["/assets/js/super/require_config.js"], function () {
        require(["base"], pageLoad);
    });

    var $d;
    var flag = 1;

    function pageLoad(_) {
        $d = $b.dialog.getDialog();
        //$b.json.lowerFields("#" + formId);
        $b.json.bind(formId, entity);

        /*
        //上传缩略图
        $b.upload({
            id: 'upload-btn',
            debug: !true,
            url: '../upload.cgi?type=category-icon',
            exts: '*.gif;*.jpg;*.png;*.bmp'
        }, function (result, data) {
            if (result) {
                $b.$('upload_path').value = data.url;
                $b.$('upload_img').setAttribute('url',
                    'images/' + data.url);
            } else {
                $b.dialog.alert("上传失败：" + data);
            }
        });
        */

        $b.$fn(".btn-submit").click(function () {
            if ($b.validator.validate(formId)) {
                var data = $b.json.toObject(formId);
                if (flag) {
                    flag = 0;
                    $b.xhr.jsonPost("save{{.table.Title}}",data, function (r) {
                        flag = 1;
                        if (!r["errCode"]) {
                            $b.dialog.alert('保存成功', function () {
                                if ($d) {
                                    $d.callback("refresh");
                                    $d.close();
                                }
                            });
                        } else {
                            $b.dialog.alert(r["errMsg"]);
                        }
                    });
                }
            }
        });

        $b.$fn(".btn-cancel").click(function () {
            document.forms[0].reset();
        });
    }
</script>
</body>
</html>
