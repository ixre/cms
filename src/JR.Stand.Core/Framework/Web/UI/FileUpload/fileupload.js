function fileupload(arg) {
    this.id = arg.id; //选择文件域的ID
    this.buttonText = arg.text; //上传按钮的文字
    this.infopanel = document.getElementById(arg.infopanel); //信息显示框
    this.processID = Math.floor(Math.random() * 100 + 100); //进程编号，用于与服务端辨别
    this.debug = arg.debug || false; //调试模式
    this.uploadurl = arg.url, //接受文件的地址
        this.processurl = arg.processurl, //进度数据地址
        this.filename = null; //文件名称
    this.file = null; //文件选择域

    //为URL附加参数
    this.processurl += this.processurl.indexOf('?') != -1 ? '&' : '?';
    this.processurl += 'processID=' + this.processID;

    //包装表单及文件选择域
    var __file_panel = document.getElementById(this.id);
    if (__file_panel.nodeName != 'SPAN') {
        alert('错误：请使用SPAN元素包装！');
    } else {
        __file_panel.style.position = 'relative';
    }

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

    //添加表单
    __html_form = document.createElement('FORM');
    __html_form.setAttribute('id', this.id + '_form');
    __html_form.method = 'POST';
    __html_form.action = this.uploadurl;
    __html_form.enctype = 'multipart/form-data'; //IE外设置enctype
    __html_form.encoding = 'multipart/form-data'; //IE设置encoding

    __html_form.target = __html_ifr.getAttribute('name');
    __file_panel.appendChild(__html_form);

    __html_up_btn = document.createElement("A");
    __html_up_btn.setAttribute('href', 'javascript:;');
    __html_up_btn.setAttribute('class', 'upload_btn');
    __html_up_btn.innerHTML = this.buttonText || '选择文件';
    __html_up_btn.style.cssText += 'outline:none;';
    __html_form.appendChild(__html_up_btn);

    var file_width = __html_up_btn.offsetWidth,
        file_height = __html_up_btn.offsetHeight || __html_up_btn.scrollHeight;
    this.file = document.createElement('INPUT');
    this.file.setAttribute('type', 'file');
    this.file.setAttribute('name', this.id);
    this.file.style.cssText = 'opacity:0;filter:alpha(opacity=0);position:absolute;top:0;left:0;width:' + file_width + 'px;height:' + file_height + 'px;z-index:101';
    __html_form.appendChild(this.file);

    __html_process = document.createElement('INPUT');
    __html_process.setAttribute('name', 'upload_process');
    __html_process.setAttribute('type', 'hidden');
    __html_process.setAttribute('value', this.id + '|' + this.processID);
    __html_form.appendChild(__html_process);


    if (!this.debug) {
        __html_ifr.style.display = 'none';
    } else {
        this.infopanel.innerHTML = '初始化完成!';
    }

    __html_up_btn.onclick = (function(t) {
        return function() {
            t.click();
        }
    })(this.file);


    this.filename = this.file.value;
    setInterval((function(t) {
        return function() {
            if (t.filename != t.file.value) {
                t.filename = t.file.value;
                t.onFileChanged(t.file);
            }
        }
    })(this), 100);


};

//当文件重新选择后发生
fileupload.prototype.onFileChanged = function(obj) {};

//上传时候发生
fileupload.prototype.onUploading = function(json) {
    if (!this._uploaded) {
        this._uploaded = 0;
        this._isMb = json.length >= 1024 * 1024;
    }
    this._tips = this._isMb ? (json.uploaded / 1024 / 1024).toFixed(2) + 'M/' + (json.length / 1024 / 1024).toFixed(2) + 'M' :
        parseInt(json.uploaded / 1024) + 'K/' + parseInt(json.length / 1024) + 'K';
    this._bit = Math.floor((json.uploaded - this._uploaded) * 2 / 1024);
    this._uploaded = json.uploaded;
    this.printf('上传进度:' + this._tips + ' &nbsp;&nbsp;速度：' + this._bit + 'kb/s');
};

//处理进度Json字符串
fileupload.prototype._handleProcessJson = function(jsonstr, ajax_timer) {
    //Json格式如：[{length:102400,received:1506,path:'/upload/12345.gif'}]";
    if (this.onUploading) {
        var process = eval(jsonstr)[0];
        this.onUploading(process);

        if (process.uploaded >= process.length) {
            clearInterval(ajax_timer);
            if (this.onUploadComplete) {
                this.onUploadComplete(process);
            }
        }
    }

};

fileupload.prototype.onUploadComplete = function(url) {};

//输出上传信息
fileupload.prototype.printf = function(html) {
    this.infopanel.innerHTML = html;
};

//调用上传文件
fileupload.prototype.upload = function() {
    var form = document.forms[this.id + '_form'];
    if (form) {
        //提交表单
        form.submit();

        //显示进度
        if (this.processurl) {
            var timer;

            var asyncRequest = (function(t) {
                return function() {
                    new js().ajax.post(t.processurl, '', function(json) {
                        //处理数据
                        t._handleProcessJson(json, timer);
                    }, function() {
                        clearInterval(timer);
                        t.printf('<span style="color:red">上传出错!</span>');
                    });
                };
            })(this);

            timer = setInterval(asyncRequest, 500);

        }
    }
};

//检查文件类型
fileupload.prototype.checkFileTypes = function(extstr) {
    var fileName = this.file.value;
    var ext = fileName.substring(fileName.lastIndexOf('.'));
    return extstr.indexOf('*' + ext.toLowerCase()) != -1;
};