//
//文件：对话框插件
//版本: 1.0
//时间：2011-10-01
//



/************************Dialog****************************/
/*
* name :Dialog
* date :2010/11/29
* update:2011/11/07
*/

//
// 模态窗口样式设置示例代码：
//.dialog .bglayer{background:black;opacity:0.6;filter:alpha(opacity=60);}
//.dialog .box{border:solid 1px #0066cc;background:white;padding:5px;}
//.dialog .box .loader{width:80px;background:url(/style/admin/ajax-loader.gif) center center no-repeat;height:30px;}
//.dialog .box .title{font-size:14px;font-weight:bold;color:#006699;background:#f5f5f5;padding:0 10px;line-height:25px;border-bottom:solid 1px #e5e5e5;margin:-5px -5px 5px -5px;}
//.dialog .box .close{padding:0 5px;font-weight:normal;}
//

//对话框的容器

//
// 调用示例：
// var d=new dialog({title:'对话框',usedrag:true,style:'dialog default'});
// d.async('ajaxtest');
//

function dialog(_dialog) {
    this.win = window;                                                              //窗口
    this.doc = null;                                                                //文档
    //模态框的编号
    this.id = new Date().getMilliseconds() + parseInt(Math.random() * 100);
    this.title = _dialog.title;                                                     //标题
    this.usedrag = _dialog.usedrag;                                                 //是否使用拖拽
    this.style = _dialog.style || 'ui-dialog';                                         //对话框样式,默认dialog
    this.setupFade = !_dialog.setupFade ? _dialog.setupFade : true;                 //是否渐渐淡去
    this.onclose;                                                                   //关闭函数,如果返回false,则不关闭

    //是否穿越框架
    if (_dialog.cross != false) {
        while (this.win.parent != this.win) { this.win = this.win.parent; }
    }
    this.doc = this.win.document;


    this._inited = false;


    //画布大小（整个网页大小）
    this.canvas = {
        width: this.doc.documentElement.clientWidth,
        clientHeight: this.doc.documentElement.clientHeight,
        height: Math.max(this.doc.documentElement.clientHeight, this.doc.body.clientHeight)
    };


    //模态框的位置
    this.point = {
        x: parseInt((this.canvas.width) / 2) + this.doc.documentElement.scrollLeft,
        y: parseInt((this.canvas.clientHeight) / 2) + this.doc.documentElement.scrollTop
    };



    /****************************  相关函数 *******************************/

    //重设对话框的位置
    this.fixBoxPosition = function (relay) {

        var box = this.getPanel().getElementsByTagName('DIV')[1];


        //延时设置
        if (!relay) {

            this.point.x = (this.canvas.width - box.offsetWidth) / 2 + document.documentElement.scrollLeft;
            this.point.y = (this.canvas.clientHeight - box.offsetHeight) / 2 + document.documentElement.scrollTop;
            box.style.left = this.point.x + 'px';
            box.style.top = this.point.y + 'px';

            //如果使用拖动
            if (this.title && this.usedrag) {
                new drag(box.getElementsByTagName('div')[0], this.win).start(box);              //使用拖拽对象
            }

        } else {

            var dialog = this;
            var i = box.offsetWidth;
            //设置多次,IE下会无法正常获得offsetHeight;
            var timer = setInterval(function () {

                dialog.point.x = (dialog.canvas.width - box.offsetWidth) / 2 + document.documentElement.scrollLeft;
                dialog.point.y = (dialog.canvas.clientHeight - box.offsetHeight) / 2 + document.documentElement.scrollTop;

                box.style.left = dialog.point.x + 'px';
                box.style.top = dialog.point.y + 'px';

                if (i != box.offsetWidth) {

                    clearInterval(timer);

                    //如果使用拖动
                    if (dialog.title && dialog.usedrag) {
                        new drag(box.getElementsByTagName('div')[0], dialog.win).start(box);              //使用拖拽对象
                    }

                }
            }, 1);
        }
    };
}

dialog.prototype._initialize = function () {
    if (this._inited) { return false; }
    var dialog = this;
    var document = this.doc;

    //添加Panel
    var panel = document.createElement('div');
    panel.id = 'panel_' + dialog.id;
    panel.className = dialog.style;
    document.body.appendChild(panel);

    //添加遮盖层
    var elem = document.createElement('div');
    elem.className = 'bglayer';
    elem.style.cssText = 'z-index:99;position:absolute;top:0;left:0;width:' + dialog.canvas.width + 'px;height:' + dialog.canvas.height + 'px;';
    panel.appendChild(elem);

    //添加模态窗口
    elem = document.createElement("DIV");
    elem.className = 'box';
    //elem.id = 'box_' + dialog.id;
    elem.style.cssText = 'z-index:100;position:absolute;left:' + (dialog.point.x) + "px;top:" + (dialog.point.y) + 'px;';
    panel.appendChild(elem);

    //如果显示标题
    if (dialog.title) {
        var titleElem = document.createElement("div");
        titleElem.className = 'title';
        titleElem.innerHTML = dialog.title + '<span class="close" onclick="window[\'dialog_' + this.id + '\'].close()" style="position:absolute;right:5px;top:0;text-decoration:none;font-family:Verdana;cursor:point" title="关闭窗口"><span>X</span></span>';
        elem.appendChild(titleElem);
        this.win['dialog_' + this.id] = this;
    }

    this._inited = true;
};


//对话框容器
dialog.prototype.getPanel = function () { return this.doc.getElementById('panel_' + this.id); };

//异步读取
dialog.prototype.async = function (uri, method, params, loadfunc, func) {

    //初始化
    this._initialize();

    var pl = this.getPanel().getElementsByTagName('DIV')[1]; //容器第2个层
    if (loadfunc) { loadfunc(pl); }

    //异步读取数据
    var ajax = J.xhr;

    if (!method || method.toLowerCase() == "get") {
        ajax.get(uri, function (x) {
            pl.innerHTML += x;
            if (func) func(x);
        });
    } else {
        ajax.post(uri, params, function (x) {
            pl.innerHTML += x;
            if (func) func(x);
        });

    }
    this.fixBoxPosition(true);   //重设位置
};

//打开网址
dialog.prototype.open = function (uri, width, height, scroll) {
    //初始化
    this._initialize();
    var pl = this.getPanel().getElementsByTagName('DIV')[1]; //容器第2个层
    pl.innerHTML += "<iframe frameborder='0' scrolling='" + (scroll || 'no') + "' src='" + uri + "' width='" + (width || '100%') + "' style='padding:0' height='" + (height || '100%') + "'></iframe>";
    pl.style.width = pl.scrollWidth + 'px';
    pl.style.height = pl.scrollHeight + 'px';
    this.fixBoxPosition();
};

//写入内容
dialog.prototype.write = function (html) {
    //初始化
    this._initialize();
    var boxElem = this.getPanel().getElementsByTagName('DIV')[1]; //容器第2个层
    if (!this.title) {
        boxElem.innerHTML = html;
    } else {
        var divs = boxElem.getElementsByTagName('DIV');
        for (var i = 1; i < divs.length; i++) {
            boxElem.removeChild(divs[i]);
        }
        boxElem.innerHTML += html;
    }
    this.fixBoxPosition();
};

//
// 关闭对话框
// callback为关闭时候发生的回执函数
//
dialog.prototype.close = function (callback) {
    if (this.onclose != null && !this.onclose()) {
        return false;
    }

    var opacity;    //透明度
    var panel = this.getPanel();

    //移除面板
    var closeDialog = (function (doc) {
    	return function(){
        	doc.body.removeChild(panel); if (callback) callback();
        }
    }(this.doc));

    closeDialog();
    this._inited=false;
    
    /*
    //是否渐渐的淡去
    if (!this.setupFade) {
    closeDialog(); return false;
    }
    
    //获取透明度
    var allowFilter = document.body.filters != undefined;
    opacity = 60;

    //定时设置遮盖的透明度
    var timer = setInterval(function () {
    if (allowFilter) {
    opacity -= opacity < 10 ? 1 : 10;           //IE较其他浏览器执行较慢(1:22)
    bgLayer.filters('alpha').opacity = opacity;
    }
    else {
    bgLayer.style.opacity = (--opacity) / 100;
    }

    //当透明度低于或等于0,则移除panel并执行回执函数
    if (opacity <= 0) {
    //清除定时器
    clearInterval(timer);
    closeDialog();
    }
    }, 1);
    */
};

//
// 拖拽代码
// 2011-11-08
//

function drag(elem, window) {
    this.elem = elem;
    this.win = window;
}
drag.prototype.start = function (obj) {
    var o = this.elem;
    var obj = obj ? obj : o;
    var sx, sy;
    var j = window.J;   //依赖于oplib

    var document = this.win == null ? document : this.win.document;

    o.style.cursor = "move";
    j.event.add(o, "mousedown", function (e) {
        e || event;
        if (e.button == 1 || e.button == 0) {
            sx = e.clientX - obj.offsetLeft; sy = e.clientY - obj.offsetTop;
            document.addEventListener('mousemove', move, false);
            document.addEventListener('mouseup', stopDrag, false);
        }
    }, false);

    var stopDrag = function () {
        document.removeEventListener('mousemove', move, false);
        document.removeEventListener('mouseup', stopDrag, false);
    };

    var move = function (e) {
        e || event;
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        if (e.preventDefault) e.preventDefault();                       //这两句便是解决firefox拖动问题的.
        with (obj.style) {
            position = "absolute";
            left = e.clientX - sx + "px";
            top = e.clientY - sy + "px";
        }
    };
};



//插件
(function (j) {
    j.plugin({ dialog: function (title, usedrag, fade, onclose, style) {
        return new dialog({
            title: title,
            usedrag: usedrag || false,
            setupFade: fade || false,
            style: style,
            onclose: onclose
        });
    }
    });
} ($JS));