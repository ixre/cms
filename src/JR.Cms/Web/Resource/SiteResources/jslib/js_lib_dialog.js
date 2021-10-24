//
//文件：对话框插件
//版本: 1.0
//时间：2011-10-01
//

/************************Dialog****************************/
/*
 * name :Dialog
 * date :2010/11/29
 * update:2019/03/27
 */

//
// 模态窗口样式设置示例代码：
// .ui-dialog .dialog-mask{background:#333;opacity:0.2;filter:alpha(opacity=20);}
// .ui-dialog .box {padding-bottom:50px;min-width:200px;
//     -moz-box-shadow:0 0 5px 0 #bbb; /*firefox*/
//     -webkit-box-shadow:0 0 5px 0 #bbb; /*webkit*/
//     /*! box-shadow:0 0 5px 0px #bbb; */ /*opera或ie9*/
//     zoom: 1; filter: progid:DXImageTransform.Microsoft.DropShadow(OffX=5, OffY=0, Color=#bbb); /* ie6,ie7,ie8 */
// }
// .ui-dialog .box .title{font-size:1.2em;font-weight:bold;background:#F2F2F2;line-height:2em;padding:0 10px;}
// .ui-dialog .box .close{cursor:pointer;position:absolute;right:0;top:0;padding:0 12px;
//     background:url(tab_close.gif) center no-repeat;text-indent:-99em}
// .ui-dialog .box .con{background:#F2F2F2;}
// .ui-dialog .ui-alert-message{padding:1em 1.5em;color:#666;font-size:1.1em;}
// .ui-dialog .ui-alert-control{background:#F8F8F8;text-align:right;padding:5px;border-top:solid 1px #e3e3e3;}
// .ui-dialog .ui-alert-control input{border:solid 1px #FFF;border-color: #ADB1B8 #A2A6AC #8D9096;
//     border-radius:3px;display: inline-block;vertical-align:middle;padding:2px 10px;margin-left:8px;
//     background:transparent linear-gradient(to bottom, #F7F8FA, #E7E9EC);height:100%;
//     box-shadow:0px 1px 0px rgba(255, 255, 255, 0.6) inset,0 1px 0 rgba(255,255,255,0.1);}

//对话框的容器

//
// 调用示例：
// var d=new dialog({title:'对话框',drag:true,"style":'dialog default'});
// d.async('ajaxtest');
//


var DialogText = {
    ALERT_MSG_TPL: '<div class="ui-alert-message">' +
        '<span class="alert-icon alert-icon-{icon}"></span>{msg}</div>',
    ALERT_BTN_TPL: '<input type="button" tag="{tag}" value="{text}"/>',
    ALERT_ALL_TITLE: '提示',
    ALERT_BTN_OK_TEXT: '确定',
    ALERT_BTN_CANCEL_TEXT: '取消'
};

function simpleDialog(opt) {
    //来源窗口
    this.window = window;
    //参数
    this.opts = {
        initialized: false,
        id: parseInt(1000 + Math.random() * 8999 +
            (new Date()).valueOf() % 1000),  //模态框的编号
        title: opt.title||'窗口', // 标题
        drag: opt.drag != false, //是否使用拖拽
        className: opt.className || "gra-modal ui-dialog",  //样式
        setupFade: false,    //是否渐渐淡去
        canNotClose: false, //是否不允许关闭
        onclose: null,//关闭函数,如果返回false,则不关闭
        cross: opt.cross != false, //是否穿越框架
        textArr: DialogText, //文本数组
    };
    this.eles = {
        panel: null,
        con: null,
        box: null,
    };
    //窗口
    this.win = window;
    //文档
    this.doc = null;
    //是否穿越框架
    if (this.opts.cross != false) {
        while (this.win.parent != this.win) {
            this.win = this.win.parent;
        }
    }
    this.doc = this.win.document;
}

simpleDialog.prototype = {
    newElement: function (node, opt) {
        var e = document.createElement(node);
        for (i in opt) {
            e.setAttribute(i, opt[i]);
        }
        return e;
    },
    // 初始化窗口
    _initialize: function () {
        with (this.opts) {
            if (initialized) {
                return false;
            }
            var doc = this.doc;
            var absCss = 'position:fixed;top:0;left:0;bottom:0;right:0;margin:auto';
            //添加Panel
            this.eles.panel = this.newElement('DIV', {
                "id": 'panel_' + id,
                "class": className
            });
            doc.body.appendChild(this.eles.panel);
            //添加遮盖层,多个模态框只显示一个遮盖层。
            // var maskLayer = doc.getElementById("glob-dialog-mask");
            // if (!maskLayer) {
            this.eles.panel.appendChild(this.newElement('DIV', {
                // "id":"glob-dialog-mask",
                "class": 'dialog-mask mask',
                "style": 'z-index:99;' + absCss
            }));
            //添加模态窗口
            var css = 'z-index:100;' + absCss;
            this.eles.box = this.newElement('DIV', {"class": 'modal box', "style": css});
            this.eles.panel.appendChild(this.eles.box);

            //添加标题
            if (title) {
                var titHtml = '<span class="left corner" style="position:absolute;left:0;top:0">&nbsp;</span><span class="txt">' + title + '</span>';
                if (!canNotClose) {
                    titHtml += '<span class="close" style="position:absolute;right:5px;top:0;">' +
                        '<i class="fa fa-times" aria-hidden="true"></i></span>';
                }
                titHtml += '<span class="right corner" style="position:absolute;right:0;top:0">&nbsp;</span>';
                var elem = this.newElement('DIV', {
                    "class": 'title',
                    "style": 'user-select:none;-ms-user-select:none;-moz-user-select:none;-webkit-user-select:none;'
                });
                elem.innerHTML = titHtml;
                this.eles.box.appendChild(elem);
                elem.getElementsByTagName('SPAN')[2].onclick = (function (d) {
                    return function () {
                        d.close();
                    };
                })(this);
                this.win['dialog_' + this.id] = this; //todo:??

                //使用拖拽对象
                if (drag) {
                    new dragObject(elem, this.win).start(this.eles.box);
                }
            }

            //创建内容区域
            this.eles.con = this.newElement('DIV', {"class": 'content con'});
            this.eles.box.appendChild(this.eles.con);

            //如果显示标题
            if (title) { //todo: ??
                var be = doc.createElement("div");
                be.className = 'bottom';
                be.style.cssText = "position:relative;";
                be.innerHTML = '<span class="left corner" style="position:absolute;left:0;">&nbsp;</span><span class="txt">&nbsp;</span><span class="right corner" style="position:absolute;right:0;">&nbsp;</span>';
                this.eles.box.appendChild(be);
            }
            initialized = true;
        }
    },
    //获取对话框容器
    getPanel: function () {
        return this.eles.panel;
    },
    //隐藏模态框
    hiddenBox: function () {
        with (this.eles.box) {
            style.visibility = 'hidden';
            style.left = 'inherit';
            style.bottom = 'inherit';
        }
    },
    //显示模态框
    showBox: function () {
        with (this.eles.box) {
            style.visibility = '';
            style.width = offsetWidth + 'px';
            style.height = scrollHeight + 'px';
            style.left = '0';
            style.bottom = '0';
        }
    },
    //获取框架里的Window对象
    getFrameWindow: function () {
        var frames = this.eles.panel.getElementsByTagName('IFRAME');
        if (frames.length > 0) {
            return frames[0].contentWindow;
        }
        return null;
    },
    //获取当前目标窗口
    getTargetWindow: function () {
        return this.win;
    },
    // 获取来源窗口
    getWindow: function () {
        return this.window;
    },
    // 回调
    callback: function (func, args) {
        var f = this.getWindow()[func];
        if (f && f instanceof Function) {
            f(args);
        }
    },
    //打开网址
    open: function (url, width, height, scroll) {
        this._initialize();
        this.hiddenBox();
        // 修正URL,如当前页为：/Home/Main;
        // 传入："Index"，自动设置为"/Home/Index"
        if (url.length > 0 && url[0] != '/' &&
            url.indexOf('//') != 0 &&
            url.indexOf('https://') == -1 &&
            url.indexOf('http://') == -1) {
            var path = this.getWindow().location.pathname;
            url = path.substring(0, path.lastIndexOf('/') + 1) + url;
        }
        this.eles.con.innerHTML += "<iframe frameborder='0' scrolling='" +
            (scroll || 'auto') + "' src='" + url + "' width='" +
            (width || '100%') + "' style='padding:0' height='" +
            (height || '100%') + "'></iframe>";
        this.showBox();
    },
    //写入内容
    write: function (html) {
        this._initialize();
        this.hiddenBox();
        var pl = this.eles.con;
        if (!this.opts.title) {
            pl.innerHTML = html;
        } else {
            // var divs = pl.getElementsByTagName('DIV');
            // for (var i = 1; i < divs.length; i++) {
            //     pl.removeChild(divs[i]);
            // }
            pl.innerHTML += html;
        }
        this.showBox();
    },
    // 保持兼容
    async: function (uri, method, params, func) {
        return this.load(uri, method, params, func);
    },
    //异步加载
    load: function (uri, method, params, func) {
        this._initialize();
        this.hiddenBox();
        //异步读取数据
        var ajax = $jr.xhr;
        var call = (function (d) {
            return function (x) {
                $jr.loadHTML(d.eles.con, x);
                d.showBox();
                if (func) func(x);
            };
        })(this);
        if (!method || method.toLowerCase() == "get") {
            ajax.get(uri, call);
        } else {
            ajax.post(uri, params, call);
        }
    },
    //自定义弹出内容
    custom: function (p) {
        this._initialize();
        this.hiddenBox();
        var d = this;
        d.write($jr.template(this.opts.textArr.ALERT_MSG_TPL, {
            msg: p.message, icon: p.icon
        }));
        var buttonsHtml = '';
        for (var i = 0; i < p.buttons.length; i++) {
            var e = p.buttons[i];
            if (e && e.tag) { //兼容IE7
                buttonsHtml += $jr.template(this.opts.textArr.ALERT_BTN_TPL,
                    {tag: e.tag, text: e.text});
            }
        }
        var rd = Math.floor(Math.random() * 10000);
        d.write('<div class="ui-alert-control" id="ui-alert-control-' + rd + '">' + buttonsHtml + '</div>');
        this.showBox();
        var pl = d.getTargetWindow().document.getElementById('ui-alert-control-' + rd);
        var fnBtnList = pl.getElementsByTagName('INPUT'); //查找点击事件的元素
        for (var i = 0; i < fnBtnList.length; i++) {
            if (i === 0) {
                fnBtnList[i].focus();
            }
            fnBtnList[i].onclick = (function (d, p) {
                return function () {
                    var tag = this.getAttribute('tag');
                    if (p.click && p.click instanceof Function) {
                        if (p.click(tag, d)) { //如果点击了指定的按钮，且处理返回true,则关闭提示框
                            d.close();
                        }
                    } else {
                        d.close();
                    }
                };
            })(d, p);
        }
        return d;
    },
    closeDialog: function () {
        this.win.document.body.removeChild(this.eles.panel);
    },
    //关闭对话框,callback为关闭时候发生的回执函数
    close: function (callback) {
        //onclose不为空，且返回值为false则不允许关闭
        if (this.opts.onclose != null && this.opts.onclose() == false) {
            return false;
        }

        //移除面板
        this.closeDialog();
        if (callback) callback();

        //删除该对象
        try {
            //IE6不能删除对象
            delete this.win['dialog_' + this.id];
        } catch (ex) {
            this.win['dialog_' + this.id] = null;
        }


        this.opts.initialized = false;

        /*
         var opacity = 0;    //透明度
         //是否渐渐的淡去
         if (!this.opts.setupFade) {
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
    }
};


//
// 拖拽代码
// 2011-11-08
//
function dragObject(elem, window) {
    this.elem = elem;
    this.win = window;
}

dragObject.prototype.regist = function (target, cursor, moveHandler, stopHandler) {
    var o = this.elem;
    target = target ? target : o;
    var sx, sy;

    var document = this.win == null ? document : this.win.document;

    o.style.cursor = cursor || "move";

    var move = moveHandler || function (e) {
        e = e || event;
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        if (e.preventDefault) e.preventDefault();   //这两句便是解决firefox拖动问题的.
        with (target.style) {
            position = "absolute";
            margin = 'inherit';
            left = e.clientX - sx + "px";
            top = e.clientY - sy + "px";
        }

    };

    $jr.event.add(o, "mousedown", function (e) {
        e = e || event;
        if (e.button == 1 || e.button == 0) {
            sx = e.clientX - target.offsetLeft;
            sy = e.clientY - target.offsetTop;
            $jr.event.add(document, 'mousemove', move, false);
            $jr.event.add(document, 'mouseup', stopDrag, false);
        }
    }, false);

    var stopDrag = function () {
        $jr.event.remove(document, 'mousemove', move, false);
        $jr.event.remove(document, 'mouseup', stopDrag, false);
        if (stopHandler && stopHandler instanceof Function) stopHandler();
    };
};
dragObject.prototype.custom = function (obj, cursor, moveHandler, stopHandler) {
    return this.regist(obj, cursor, moveHandler, stopHandler);
};
dragObject.prototype.start = function (obj) {
    this.regist(obj, null, null, null);
};


var SimpleDialog = {
    //创建对话框
    create: function (dialogOpts) {
        return new simpleDialog(dialogOpts);
    },

    //创建
    create2: function (title, usedrag, cross, onclose, className) {
        return new simpleDialog({
            title: title,
            drag: usedrag || false,
            cross: cross || false,
            //setupFade: fade || false,
            "className": className,
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
            if (dialog) {
                break;
            }
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
    },
    customAlert: function (p) {
        var title = DialogText.ALERT_ALL_TITLE + '-' + (p.title || '');
        title = title.replace(/^-|-$/g, '');
        var d = this.create({
            title: title,
            className: "ui-dialog custom-dialog",
            drag: p.drag || true,
            cross: p.cross || true,
            canClose: false
        });
        return d.custom(p);
    },
    alert: function (msg, func, icon, text) {
        return this.customAlert({
            icon: icon,
            message: msg,
            buttons: [
                {tag: 'ok', text: text || DialogText.ALERT_BTN_OK_TEXT}
            ],
            click: function () {
                if (func instanceof Function) func();
                return true;
            }
        });
    },
    confirm: function (msg, func, textArr) {
        var okTxt = DialogText.ALERT_BTN_OK_TEXT;
        var cancelTxt = DialogText.ALERT_BTN_CANCEL_TEXT;
        if (textArr instanceof Array && textArr.length === 2) {
            okTxt = textArr[0];
            cancelTxt = textArr[1];
        }
        return this.customAlert({
            icon: 'confirm',
            message: msg,
            buttons: [
                {tag: 'ok', text: okTxt},
                {tag: 'cancel', text: cancelTxt}
            ],
            click: function (tag) {
                if (func instanceof Function) {
                    func(tag != 'cancel');
                }
                return true;
            }
        });
    }
};

//插件
(function (r) {
    if (r) {
        var obj = {
            dialog: window.SimpleDialog,
            drag: function (ele, win) {
                return new dragObject(ele, win);
            }
        };
        r.extend(obj);
    }
})(window.$jr);