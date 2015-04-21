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
//.ui-dialog .bglayer{background:#d0d0d0;opacity:0.2;filter:alpha(opacity=20);}
//.ui-dialog .box {
//    border: solid 1px #bbb;
//    background: #f3f3f3;
//    -moz-box-shadow:0 0 10px 2px #bbb; /*firefox*/
//    -webkit-box-shadow:0 0 10px 2px #bbb; /*webkit*/
//    box-shadow:0 0 10px 2px #bbb; /*opera或ie9*/
//    zoom: 1; filter: progid:DXImageTransform.Microsoft.DropShadow(OffX=10, OffY=2, Color=#bbb); /* ie6,ie7,ie8 */
//}
//.ui-dialog .box .title{padding-top:10px;font-size:14px;font-weight:bold;color:#006699;background:#F0F0F0;padding:0 10px;line-height:30px;
//    background:#f9f9f9 url(/framework/assets/sys_themes/default/title_bg.gif) 0 5px repeat-x;
//    /*border-bottom:solid 2px #FFF;*/}
//.ui-dialog .box .title .txt{padding-left:25px;background:url(/framework/assets/sys_themes/default/blocks.gif) left center no-repeat;}
//.ui-dialog .box .close{padding:0 10px;font-weight:normal;}
//.ui-dialog .box .boxcontent{margin:0 5px 5px 5px;padding:10px;background:#fff;border:solid 1px #d0d0d0}
//.ui-dialog .box .bottom{display:none;}
//

//对话框的容器

//
// 调用示例：
// var d=new dialog({title:'对话框',usedrag:true,style:'dialog default'});
// d.async('ajaxtest');
//
function simpleDialog(dialogOptions) {
    this._simgpleDialog = true;
    this.window = window;
    this.win = window;                                                                //窗口
    this.doc = null;                                                                //文档
    //模态框的编号
    this.id = new Date().getMilliseconds() + parseInt(Math.random() * 100);
    this.title = dialogOptions.title;                                                     //标题
    this.usedrag = dialogOptions.usedrag;                                                 //是否使用拖拽
    this.style = dialogOptions.style || 'ui-dialog';                                      //对话框样式,默认dialog
    this.setupFade = !dialogOptions.setupFade ? dialogOptions.setupFade : true;           //是否渐渐淡去
    this.onclose = dialogOptions.onclose;                                                 //关闭函数,如果返回false,则不关闭

    //是否穿越框架
    if (dialogOptions.cross != false) {
        while (this.win.parent != this.win) { this.win = this.win.parent; }
    }
    this.doc = this.win.document;


    this._inited = false;


    //画布大小（整个网页大小）
    this.canvas = {
        width: this.doc.documentElement.clientWidth,
        height: Math.min(this.doc.documentElement.clientHeight, this.doc.body.clientHeight),
        maxHeight: Math.max(this.doc.documentElement.clientHeight, this.doc.body.clientHeight)
    };

    //模态框的位置
    this.point = {
        x: parseInt((this.canvas.width) / 2) + this.doc.documentElement.scrollLeft,
        y: parseInt((this.canvas.height) / 2) + this.doc.documentElement.scrollTop
    };



    /****************************  相关函数 *******************************/

    //重设对话框的位置
    this.fixBoxPosition = function (relay) {

        var box = this.getPanel().getElementsByTagName('DIV')[1];

        //延时设置
        if (!relay) {
            this.point.x = (this.canvas.width - box.offsetWidth) / 2 + document.documentElement.scrollLeft;
            this.point.y = (this.canvas.height - box.offsetHeight) / 2 + document.documentElement.scrollTop;

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
                dialog.point.y = (dialog.canvas.height - box.offsetHeight) / 2 + document.documentElement.scrollTop;

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

simpleDialog.prototype._initialize = function () {
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
    elem.style.cssText = 'z-index:999;position:absolute;top:0;left:0;width:'
        + dialog.canvas.width + 'px;height:'
        + dialog.canvas.maxHeight + 'px;';

    panel.appendChild(elem);

    //添加模态窗口
    elem = document.createElement("DIV");
    elem.className = 'box';
    //elem.id = 'box_' + dialog.id;
    elem.style.cssText = 'z-index:1000;position:absolute;left:'
        + (dialog.point.x) + "px;top:" + (dialog.point.y) + 'px;';
    panel.appendChild(elem);

    //如果显示标题
    if (dialog.title) {
        var titleElem = document.createElement("div");
        titleElem.className = 'title';
        titleElem.innerHTML = '<span class="left corner" style="position:absolute;left:0;top:0">&nbsp;</span><span class="txt">'
            + dialog.title + '<span class="close" onclick="window[\'dialog_'
            + this.id + '\'].close()" style="position:absolute;right:5px;top:0;text-decoration:none;font-family:Verdana;cursor:pointer" title="关闭窗口"><span>X</span></span></span><span class="right corner" style="position:absolute;right:0;top:0">&nbsp;</span>';
        elem.appendChild(titleElem);
        this.win['dialog_' + this.id] = this;
    }

    //创建内容区域
    var cm = document.createElement("DIV");
    cm.className = 'content boxcontent';
    cm.id = 'boxcontent_' + this.id;
    elem.appendChild(cm);

    //如果显示标题
    if (dialog.title) {
        var be = document.createElement("div");
        be.className = 'bottom';
        be.style.cssText = "position:relative;";
        be.innerHTML = '<span class="left corner" style="position:absolute;left:0;top:0">&nbsp;</span><span class="txt">&nbsp;</span><span class="right corner" style="position:absolute;right:0;top:0">&nbsp;</span>';
        elem.appendChild(be);
    }

    this._inited = true;
};


//对话框容器
simpleDialog.prototype.getPanel = function () { return this.doc.getElementById('panel_' + this.id); };

//异步读取
simpleDialog.prototype.async = function (uri, method, params, loadfunc, func) {

    //初始化
    this._initialize();

    var pl = this.doc.getElementById('boxcontent_' + this.id);
    if (loadfunc) { loadfunc(pl); }

    //异步读取数据
    var ajax = $JS.xhr;
    var dialog = this;

    if (!method || method.toLowerCase() == "get") {
        ajax.get(uri, function (x) {
            $JS.loadHTML(pl, x);
            dialog.fixBoxPosition(true);   //重设位置
            if (func) func(x);
        });
    } else {
        ajax.post(uri, params, function (x) {
            $JS.loadHTML(pl, x);
            dialog.fixBoxPosition(true);   //重设位置
            if (func) func(x);
        });

    }
};

//打开网址
simpleDialog.prototype.open = function (uri, width, height, scroll) {

    //初始化
    this._initialize();
    var pl = this.doc.getElementById('boxcontent_' + this.id);
    pl.innerHTML += "<iframe frameborder='0' scrolling='" + (scroll || 'no') + "' src='" + uri + "' width='" + (width || '100%') + "' style='padding:0' height='" + (height || '100%') + "'></iframe>";
    pl.style.width = Math.max(pl.scrollWidth, width) + 'px';
    pl.style.height = Math.max(pl.scrollHeight, height) + 'px';
    this.fixBoxPosition();
};

//写入内容
simpleDialog.prototype.write = function (html) {
    //初始化
    this._initialize();
    var pl = this.doc.getElementById('boxcontent_' + this.id);
    if (!this.title) {
        pl.innerHTML = html;
    } else {
        var divs = pl.getElementsByTagName('DIV');
        for (var i = 1; i < divs.length; i++) {
            pl.removeChild(divs[i]);
        }
        pl.innerHTML += html;
    }
    this.fixBoxPosition();
};

//获取框架里的Window对象
simpleDialog.prototype.getFrameWindow = function () {
    var iframes = this.getPanel().getElementsByTagName('IFRAME');
    if (iframes.length > 0) {
        return iframes[0].contentWindow;
    }
    return null;
};

//
// 关闭对话框
// callback为关闭时候发生的回执函数
//
simpleDialog.prototype.close = function (callback) {
    //onclose不为空，且返回值为false则不允许关闭
    if (this.onclose != null && this.onclose() == false) {
        return false;
    }

    var opacity = 0;    //透明度
    var panel = this.getPanel();

    //移除面板
    var closeDialog = (function (win) {
        return function () {
            win.document.body.removeChild(panel);
            if (callback) callback();
        }
    }(this.win));

    //删除该对象
    try {
        //IE6不能删除对象
        delete this.win['dialog_' + this.id];
    } catch (ex) { this.win['dialog_' + this.id] = null; }

    closeDialog();
    this._inited = false;

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

drag.prototype.regist = function (obj, cursor, moveHandler, stopHandler) {
    var o = this.elem;
    obj = obj ? obj : o;
    var sx, sy;

    var document = this.win == null ? document : this.win.document;

    o.style.cursor = cursor || "move";

    var move = moveHandler || function (e) {
        e = e || event;
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        if (e.preventDefault) e.preventDefault();                       //这两句便是解决firefox拖动问题的.
        with (obj.style) {
            position = "absolute";
            left = e.clientX - sx + "px";
            top = e.clientY - sy + "px";
        }

    };

    $JS.event.add(o, "mousedown", function (e) {
        e = e || event;
        if (e.button == 1 || e.button == 0) {
            sx = e.clientX - obj.offsetLeft; sy = e.clientY - obj.offsetTop;
            $JS.event.add(document, 'mousemove', move, false);
            $JS.event.add(document, 'mouseup', stopDrag, false);
        }
    }, false);

    var stopDrag = function () {
        $JS.event.remove(document, 'mousemove', move, false);
        $JS.event.remove(document, 'mouseup', stopDrag, false);

        if (stopHandler && stopHandler instanceof Function) stopHandler();
    };


};
drag.prototype.custom = function (obj, cursor, moveHandler, stopHandler) {
    return this.regist(obj, cursor, moveHandler, stopHandler);
};
drag.prototype.start = function (obj) {
    this.regist(obj, null, null, null);
};


var SimpleDialog = {

    //创建对话框
    create: function (dialogOpts) {
        return new simpleDialog(dialogOpts);
    },

    //创建
    create2: function (title, usedrag, cross, onclose, style) {
        return new simpleDialog({
            title: title,
            usedrag: usedrag || false,
            cross: cross || false,
            //setupFade: fade || false,
            style: style,
            onclose: onclose
        });
    },

    //获取对话框,targetWin为目标窗口，获取目标窗口的弹出
    getDialog: function () {
        var dialog = null;
        var reg = /^dialog_/i;
        var targetWin = window;

        var getFromWin = function (win) {
            var d = null;
            for (var i in win) {
                if (reg.test(i) && win[i] != null) {
                    d = win[i];
                    //todo:不能识别到是哪个dialog，
                    //可能会出现在dialog里打开dialog
                    //所以判断到第一个dialog
                    break;
                }
            }
            return d;
        };

        do {
            dialog = getFromWin(targetWin);
            if (dialog == null) {
                targetWin = targetWin.parent;
                dialog = getFromWin(targetWin);
            }
            if (dialog) { break; }
        } while (targetWin.parent != targetWin);




        //如果传递为空，则获取最顶层的窗口
        //if (!targetWin) {
        //    var targetWin = window;
        //    while (targetWin.parent != targetWin) {
        //        targetWin = targetWin.parent;
        //    }
        //}



        return dialog;
    },

    //关闭窗口
    close: function () {
        var d = this.getDialog();
        if (d) d.close();
    }
};




//插件
(function (j) {
    j.extend({
        dialog: window.SimpleDialog,
        drag: function (ele, win) {
            return new drag(ele, win);
        }
    });
}($JS));