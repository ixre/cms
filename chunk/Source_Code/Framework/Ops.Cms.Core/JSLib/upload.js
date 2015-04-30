/*******************************************
* 文 件 名：sui.fileupload.js
* 文件说明：文件上传库
* 修改说明：
*	2013-10-27 19:56 newmin [!]:IE7下up.innerHTML报错
* 							[+]:调试模式下显示file input
********************************************/
function fileUpload(arg, infoFunc) {
    this.id = arg.id;                                                         //选择文件域的ID
    this.infopanel = arg.info ? document.getElementById(arg.info) : null;
    this.processID = Math.random() * 100 + 100;                               //进程编号，用于与服务端辨别
    this.debug = arg.debug || false;                                          //调试模式
    this.uploadurl = arg.url,                                                 //接受文件的地址
    this.processurl = arg.processurl,                                         //进度数据地址
    this.filename = null;                                                     //文件名称
    this.file = null;                                                         //文件选择域
    this.repeatSelect = arg.repeatSelect == undefined ? false : arg.repeatSelect;                                     //允许重复选择文件
    this.btnText = '';
    this.btnClicked = false;

    this.repeatSelect = false;
    //为URL附加参数
    /*
    this.processurl += this.processurl.indexOf('?') != -1 ? '&' : '?';
    this.processurl += 'processID=' + this.processID;
    */

    var __file_panel = document.getElementById(this.id);
    this.btnText = __file_panel.innerHTML || '选择文件';
    __file_panel.innerHTML = '';
    //__file_panel.style.position = 'relative';

    var __html_ifr,
        __html_form,
        __html_up_btn,
        __html_process,
        __html_msg_panel;

    //添加框架
    try {
        __html_ifr = document.createElement('<IFRAME name="' + (this.id + '_iframe') + '">');
    } catch (ex) {
        __html_ifr = document.createElement('IFRAME');
        __html_ifr.setAttribute('name', this.id + '_iframe');
    }
    __file_panel.appendChild(__html_ifr);
    if (!this.debug) {
        __html_ifr.style.display = 'none';
    }

    //添加事件
    var _loadCall = (function (t, ifr) {
        return function () {
            var _doc = ifr.contentWindow.document;
            //判断是否跨域
            var _headContent = '';
            try {
                var heads = _doc.getElementsByTagName('HEAD');
                if (heads.length != 0) {
                    _headContent = heads[0].innerHTML;
                }
            } catch (exc) {
                t.onUploadComplete.apply(t, [false, exc]);
                return;
            }


            //自动检测是否包含标题，如果包含标题则上传错误
            if (_headContent) {
                var title = /<title>(.+?)<\/title>/.exec(_headContent);
                if (title) {
                    title = title[1];
                    t.onUploadComplete.apply(t, [false, title, _doc]);
                    return;
                }
            }

            var content = _doc.body.innerHTML;
            //成功返回状态
            var sucessResult = content;
            if (content == '') return;
            content = /{[\s\S]*}/igm.exec(content);
            //返回的是Json字符
            if (content) {
                sucessResult = $JS.toJson(content);
            }
            t.onUploadComplete.apply(t, [true, sucessResult, _doc]);
        };
    })(this, __html_ifr);

    $JS.event.add(__html_ifr, 'load', _loadCall);

    //添加表单
    __html_form = document.createElement('FORM');
    __html_form.setAttribute('id', this.id + '_form');
    __html_form.method = 'POST';
    __html_form.style.cssText = 'display:inline';
    __html_form.className = 'ui-uploadform';
    __html_form.action = this.uploadurl;
    __html_form.enctype = 'multipart/form-data';          //IE外设置enctype
    __html_form.encoding = 'multipart/form-data';         //IE设置enctype

    __html_form.target = __html_ifr.getAttribute('name');
    __file_panel.appendChild(__html_form);

    __html_up_btn = document.createElement("span");
    __html_up_btn.className = 'upbtn';
    __html_up_btn.innerHTML = this.btnText;
    __html_form.appendChild(__html_up_btn);

    this.file = document.createElement('INPUT');
    this.file.setAttribute('type', 'file');
    this.file.setAttribute('name', this.id);
    this.file.setAttribute('tabindex', '9999');
    var __x = __html_up_btn.offsetTop;
    var opacity = this.debug ? 50 : 0;
    this.file.style.cssText = 'opacity:' + (opacity / 100) + ';filter:alpha(opacity=' + opacity + ');cursor:normal;position:absolute;top:' + __html_up_btn.offsetTop + 'px;left:' + __html_up_btn.offsetLeft + 'px;z-index:101;width:' + __html_up_btn.offsetWidth + 'px;height:' + __html_up_btn.offsetHeight + 'px;';

    //允许重复上传文件
    if (this.repeatSelect) this.file.onclick = function () { this.value = ''; };
    __html_form.appendChild(this.file);

    __html_process = document.createElement('INPUT');
    __html_process.setAttribute('name', 'upload_process');
    __html_process.setAttribute('type', 'hidden');
    __html_process.setAttribute('value', this.id + '|' + this.processID);
    __html_form.appendChild(__html_process);

    /*
    //上传目录
    if (arg.dir) {
    var Obj = document.createElement('INPUT');
    Obj.setAttribute('name', 'dir');
    Obj.setAttribute('type', 'hidden');
    Obj.setAttribute('value', arg.dir);
    __html_form.appendChild(Obj);
    }

    //点击上传文字便选择文件
    __html_up_btn.onclick = (function (t) {
    return function () {
    t.click();
    }
    })(this.file);
    */

    this.filename = this.file.value;
    setInterval((function (t) {
        return function () {
            if (t.file.value!= '' && t.filename != t.file.value) {
                t.filename = t.file.value;
                t.onFileChanged(t.file);
            }
        }
    })(this), 100);
}

//输出上传信息
fileUpload.prototype.printf = function (html) { if (this.infopanel) this.infopanel.innerHTML = html; };

//检查文件类型
fileUpload.prototype.checkFileTypes = function (extstr) {
    var fileName = this.file.value;
    var ext = fileName.substring(fileName.lastIndexOf('.'));
    return new RegExp('\\*' + ext + ';*', 'i').test(extstr);
};

//当文件重新选择后发生
fileUpload.prototype.onFileChanged = function (obj) { };

//上传时候发生
fileUpload.prototype.onUploading = function (json) { };

//上传完成时发生,数据如：{result:true,url:'',message:''}
fileUpload.prototype.onUploadComplete = function (result, data, source) { };

/*
//处理进度Json字符串
fileupload.prototype._handleProcessJson = function (jsonstr, ajax_timer) {
    //Json格式如：[{state:1,length:102400,received:1506,path:'/upload/12345.gif'}]";
    var process = eval(jsonstr)[0];
    if (process.state != 1) {
        if (this.onUploading) {
            if (process.length != 0 && process.uploaded < process.length || process.path == '') {
                this.onUploading(process);
            }
        }
    }
    else {
        //state 为1表示上传完成
        clearInterval(ajax_timer);
        if (this.onUploadComplete) {
            this.onUploadComplete(process);
        }
    }
};
*/


//调用上传文件
fileUpload.prototype.upload = function () {
    var form = document.forms[this.id + '_form'];
    if (form) {
        //提交表单
        form.submit();
        if (this.onUploading) this.onUploading();

        /*
        //显示进度
        if (this.processurl) {
        var _timer;

        var asyncRequest = (function (t) {
        return function () {
        try {
        J.xhr.request(
        { uri: t.processurl, method: "POST", data: 'json' },
        {
        success: function (json) {
        //处理数据
        t._handleProcessJson(json, _timer);
        },
        error: function () {
        clearInterval(_timer);
        t.printf('<span style="color:red">上传出错!</span>');
        } 
        });
        } catch (ex) {
        clearInterval(_timer);
        throw ex;
        }
        };
        })(this);
        _timer = setInterval(asyncRequest, 1000);
        }*/
    }
};


//上传文件
function fileUploadObject(arg, func) {
    var _upload = new fileUpload(arg);
    var up = document.getElementById(_upload.id).getElementsByTagName('SPAN')[0];
    /*
   var _lastuploaded = 0;
   var _upload_speed = 0;

   
   _upload.onUploading = function (json) {
       _upload_speed = parseInt((json.uploaded - _lastuploaded) / 1024 * 2);
       _lastuploaded = json.uploaded;
       this.printf('上传进度:' + parseInt(json.uploaded / 1024) + 'KB/' + parseInt(json.length / 1024) + 'KB&nbsp;&nbsp;上传速度：' + _upload_speed + 'KB/s');
   };
   */


    _upload.onUploading = function (json) {
        _upload.file.setAttribute('disabled', 'disabled');
        try { up.innerHTML = '上传中'; } catch (ex) { }
    };


    _upload.onUploadComplete = function (result, data, doc) {
        _upload.file.removeAttribute('disabled');
        try { up.innerHTML = _upload.btnText; } catch (ex) { }

        if (func) {
            func(result, data, doc);
        }
    };

    _upload.onFileChanged = function (file) {
        if (arg.exts && !_upload.checkFileTypes(arg.exts)) {
            alert('文件类型不允许上传,仅允许上传文件类型为：' + arg.exts);
        } else {
            _upload.upload();
        }
    };

    return _upload;
}

$JS.extend({
    upload: function (arg, func) { return fileUploadObject(arg, func); }
});

