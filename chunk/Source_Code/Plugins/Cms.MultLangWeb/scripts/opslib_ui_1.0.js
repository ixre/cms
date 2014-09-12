
/*定时器 */
function timer(times, callback) { this.times = times; this.callback = callback; }
timer.prototype.start = function (func) {
    var t, i = 0, _t = this;
    t = setInterval(function () { i++; if (func) func(); if (i == _t.times) { clearInterval(t); if (_t.callback) _t.callback(); } }, 1);
};


/******************** DIV SCROLLER ************************/

//创建时间:2011/07/14
function scroller(elemID, params, interval) {

    /* 初始变量值 */
    this.$P = null;                                 //父元素
    this.$C = null;                                 //子元素
    this.$L = null;                                 //子元素列表
    this.pagePanel = null;                          //分条容器，如果设置params.pagerid,则自动启用分条容器

    this.index = 0;                                 //当前显示的图片
    this.offset = 0;                                //要滚动的偏移量
    this.scroll = 0;                                //已经滚动的偏移量
    this.scrollUnit = params.unit || 5;                //每次滚动的单位
    this.lock = false;                              //锁定滚动，用于下一张
    this.direction = params.direction || 'left';    //滚动方向
    this.interval = interval;                       //滚动间隔时间


    this.timer = null;                              //子元素计时器
    this.taskTimer = null;


    //-------------------- 获取元素 ----------------------//

    //父元素
    this.$P = document.getElementById(elemID);
    this.$P.style.cssText += 'overflow:hidden;position:relative;';

    //滚动列表的父元素(第一个UL)
    //UL里的li样式应用padding属性，而不是margin属性

    var _elist = this.$P.getElementsByTagName('UL');
    if (_elist.length == 0) {
        alert('无法找到元素ID为:' + this.$P.getAttribute('id') + '下的UL元素!'); return false;
    } else {
        this.$C = _elist[0];
        this.$L = this.$C.getElementsByTagName('LI');

        //给子元素的子元素添加索引值,用于定位
        for (var i = 0; i < this.$L.length; i++) {
            this.$L[i].setAttribute('scroll-index', i);
        }

    }

    //-------------------- 校验方向 ----------------------//

    if (this.direction == 'left') {
        //向左滚动则设置为一行
        var totalWidth = 0;
        for (var i = 0; i < this.$L.length; i++) {
            totalWidth += this.$L[i].offsetWidth;
        }
        this.$C.style.width = totalWidth + 'px';   //是否会两行
    }
    else if (this.direction == 'top') {
    } else {
        alert('仅支持方向：left和top'); return false;
    }


    //---------------- 初始化分条显示容器 ----------------//

    this.pagePanel = document.getElementById(params.pagerid);

    if (this.pagePanel) {
        for (var i = 0; i < this.$L.length; i++) {
            var e = document.createElement("A");
            if (i == 0) {
                e.className = 'current';
            }
            e.innerHTML = (i + 1).toString();
            e.href = 'javascript:;';
            e.onclick = (function (t, _i) {
                return function () {
                    t.setIndex(_i);
                };
            })(this, i);

            this.pagePanel.appendChild(e);
        }
    }



    //-------------------- 开始启动 ---------------------//
    //this._start();
    this._restart();

}

scroller.prototype.logger = function (text) {
    var _logger = document.getElementById('scroll-logger');
    if (_logger) {
        _logger.innerHTML = text;
    }
};

scroller.prototype._async = function () {
    //
    //BUG:索引有误，第一次跳转，第2张的索引为0
    //

    //获取图片索引
    this.index = parseInt(this.$L[0].getAttribute('scroll-index'));

    if (this.index == this.$L.length) {
        this.index = 0;
    }

    //显示条数
    this.logger((this.index + 1) + '/' + this.$L.length);

    if (this.pagePanel) {
        var es = this.pagePanel.getElementsByTagName('A');
        for (var i = 0; i < es.length; i++) {
            es[i].className = i == this.index ? 'current' : '';
        }
    }
};


//设置索引
scroller.prototype.setIndex = function (_index) {
    this.lock = true;
    for (var i = 0; i < this.$L.length; i++) {
        if (parseInt(this.$L[i].getAttribute('scroll-index')) == _index) {
            var node = this.$L[i];
            this.$C.removeChild(node);
            this.$C.insertBefore(node, this.$L[0]);
            break;
        }
    }
    this.$C.style.marginLeft = '0px';

    this.index = _index;
    this.lock = false;
    this._async();
    this._restart();
};

//上一张
scroller.prototype.prev = function () {
    //如果已经点击则点击无效
    if (this.lock || this.scroll > this.scrollUnit) {
        return false;
    }
    else {
        this.lock = true;
        var t = this;
        this._start(true, true, function () { t.lock = false; });
    }
};

//下一张
scroller.prototype.next = function () {
    //如果已经点击则点击无效
    if (this.lock || this.scroll > this.scrollUnit) {
        return false;
    }
    else {
        this.lock = true;
        var t = this;
        this._start(!true, true, function () { t.lock = false; });
    }
};



//开始
scroller.prototype._start = function (asc, internal, call) {
    if (this.lock && !internal) { return false; }

    //下一张
    if (!asc) {
        this._sc_left(call);
    } else {
        this._sc_right(call);
    }

    //同步状态
    this._async();
};

//重新开始
scroller.prototype._restart = function () {
    var t = this;
    //重复滚动
    if (this.taskTimer) {
        clearTimeout(this.taskTimer);
    }
    this.taskTimer = setTimeout(function () { t._start(); }, this.interval);
};

//向左滚动
scroller.prototype._sc_left = function (call) {
    var ref = this;
    var _offset = ref.$L[0].offsetWidth;

    this.timer = setInterval(function () {

        if (ref.scroll == _offset) {
            clearInterval(ref.timer);
            ref.scroll = 0;

            //添加到后面
            var node = ref.$L[0];
            ref.$C.removeChild(node);
            ref.$C.appendChild(node);

            //执行回执函数
            if (call) { call(); }

            ref._restart();


            ref.$C.style.marginLeft = '0px';
            return false;
        }

        ref.scroll += ref.scrollUnit;

        if (ref.scroll > _offset) {
            ref.scroll = _offset;
        }
        ref.$C.style.marginLeft = (-ref.scroll) + 'px';

    }, 10);

};

//向由滚动
scroller.prototype._sc_right = function (call) {

    //上一张
    var ref = this;
    var _offset = ref.$L[ref.$L.length - 1].offsetWidth;

    //追加到前面
    var node = ref.$C.lastChild;
    ref.$C.removeChild(node);
    ref.$C.insertBefore(node, ref.$L[0]);
    //重设位置
    ref.scroll = -_offset;


    this.timer = setInterval(function () {

        if (ref.scroll == 0) {
            clearInterval(ref.timer);

            //执行回执函数
            if (call) { call(); }

            ref._restart();


            ref.$C.style.marginLeft = '0px';
            return false;
        }

        ref.scroll += ref.scrollUnit;

        if (ref.scroll > 0) {
            ref.scroll = 0;
        }
        ref.$C.style.marginLeft = (ref.scroll) + 'px';
    }, 10);
};



//
// DIV卷轴效果
// 创建时间：2011-10-26 14:51
// _setup控制每步卷的像素
//
function roller(arguments) {
    //参数
    this.elem = arguments.elem;
    this.direction = arguments.direction;
    this.pix = arguments.pix;
    this.elem.style.cssText += 'overflow:hidden;';
}
roller.prototype.start = function (_setup, callback) {

    var _elem = this.elem,
            _pix = this.pix,
            _setup = _setup | 1;

    //i和j为变量
    var i, j;

    //定时器
    var timer;

    var callbackFunc = function () { if (callback != null) callback(); };

    //
    // 向上和向左逻辑一样,向下和向右则反之
    //
    switch (this.direction) {
        case "up":
            i = _pix; j = 0;
            timer = setInterval(function () {
                i -= _setup;
                if (i < 0) {
                    i = 0;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.height = i.toString() + "px";
            }, 10);
            break;

        case "left":
            i = _pix; j = 0;
            timer = setInterval(function () {
                i -= _setup;
                if (i < 0) {
                    i = 0;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.width = i.toString() + "px";
            }, 10);
            break;

        case "down":
            i = 0; j = _pix;
            timer = setInterval(function () {
                i += _setup;
                if (i > j) {
                    i = j;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.height = i.toString() + "px";
            }, 10);
            break;

        case "right":
            i = 0; j = _pix;
            timer = setInterval(function () {
                i += _setup;
                if (i > j) {
                    i = j;
                    clearInterval(timer); callbackFunc();
                }
                _elem.style.width = i.toString() + "px";
            }, 10);
            break;

    }

};



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
//创建实例时候加入该容器，关闭时候全部从容器中移除
var dialogPanel = new Array();

//
// 调用示例：
// var d=new dialog({title:'对话框',usedrag:true,style:'dialog default'});
// d.async('ajaxtest');
//

function dialog(_dialog) {

    //模态框的编号
    this.id = new Date().getMilliseconds() + parseInt(Math.random() * 100);
    this.title = _dialog.title;                                                     //标题
    this.usedrag = _dialog.usedrag;                                                 //是否使用拖拽
    this.style = _dialog.style || 'dialog';                                         //对话框样式,默认dialog
    this.setupFade = !_dialog.setupFade ? _dialog.setupFade : true;                 //是否渐渐淡去
    this.onclose;                                                                   //关闭函数,如果返回false,则不关闭

    //画布大小（整个网页大小）
    this.canvas = {
        width: document.documentElement.clientWidth,
        clientHeight: document.documentElement.clientHeight,
        height: document.documentElement.clientHeight > document.body.clientHeight
                ? document.documentElement.clientHeight : document.body.clientHeight
    };


    //模态框的位置
    this.point = {
        x: parseInt((this.canvas.width) / 2) + document.documentElement.scrollLeft,
        y: parseInt((this.canvas.clientHeight) / 2) + document.documentElement.scrollTop
    };

    //添加到窗口容器中
    dialogPanel.push(this);


    //初始化
    var initialize = function (dialog) {

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
            titleElem.innerHTML = dialog.title + '<a href="javascript:closeDialog(\'' + dialog.id + '\');" class="close" style="position:absolute;right:5px;top:0;text-decoration:none;font-family:Verdana" title="关闭窗口">X</a>';
            elem.appendChild(titleElem);
        }

    };

    initialize(this);

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
                new drag(box.getElementsByTagName('div')[0]).start(box);              //使用拖拽对象
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
                        new drag(box.getElementsByTagName('div')[0]).start(box);              //使用拖拽对象
                    }

                }
            }, 1);
        }


    };
}

//对话框容器
dialog.prototype.getPanel = function () { return document.getElementById('panel_' + this.id); };

//异步读取
dialog.prototype.async = function (uri, method, params, func) {

    var boxElem = this.getPanel().getElementsByTagName('DIV')[1]; //容器第2个层

    //创建加载提示
    var loader = document.createElement('DIV');
    loader.className = 'loader';
    boxElem.appendChild(loader);
    //如果样式表未设置则显示图片
    if (loader.offsetWidth == 0) {
        loader.innerHTML = '<div style="line-height:25px;text-align:center;color:#006699;width:100px">loading...</div>';
    }


    var removeLoader = function () {
        boxElem.removeChild(loader);
    };
    //异步读取数据

    var ajax = new ajaxObj();
    if (!method || method.toLowerCase() == "get") {
        ajax.get(uri, function (x) {
            removeLoader();
            boxElem.innerHTML += x;
            if (func) func(x);
        });
    } else {
        ajax.post(uri, params, function (x) {
            removeLoader();
            boxElem.innerHTML += x;
            if (func) func(x);
        }, function (x) {
            removeLoader();
        });

    }
    this.fixBoxPosition(true);   //重设位置
};

//打开网址
dialog.prototype.open = function (uri, width, height, scroll) {
    var boxElem = this.getPanel().getElementsByTagName('DIV')[1]; //容器第2个层
    boxElem.innerHTML += "<iframe frameborder='0' scrolling='" + scroll + "' src='" + uri + "' width='" + width + "' style='padding:0' height='" + height + "'></iframe>";
    this.fixBoxPosition();
};

//写入内容
dialog.prototype.write = function (html) {
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

    //移除模态窗口
    panel.removeChild(panel.getElementsByTagName('div')[1]);

    //遮盖层
    var bgLayer = panel.getElementsByTagName('div')[0];

    //移除面板
    var closeDialog = function () {
        document.body.removeChild(panel); if (callback) callback();
    };

    //是否渐渐的淡去
    if (!this.setupFade) {
        closeDialog(); return false;
    }

    //获取透明度
    var allowFilter = document.body.filters != undefined;
    //    if (allowFilter) {
    //        opacity = bgLayer.filters('alpha').opacity ||60;
    //    } else {
    //        opacity = bgLayer.style.opacity||60;
    //    }

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
};


//关闭窗口
function closeDialog(id) {
    for (var i in dialogPanel) {
        if (dialogPanel[i].id == id) dialogPanel[i].close();
    }
}


//
// 拖拽代码
// 2011-11-08
//

function drag(elem) {
    this.elem = elem;
}
drag.prototype.start = function (obj) {
    var o = this.elem;
    var obj = obj ? obj : o;
    var sx, sy;
    o.style.cursor = "move";
    addListener(o, "mousedown", function (e) {
        e || event;
        if (e.button == 1 || e.button == 0) {
            sx = e.clientX - obj.offsetLeft; sy = e.clientY - obj.offsetTop;
            addListener(document, "mousemove", move, false);
            addListener(document, "mouseup", stopDrag, false);
        }
    }, false);

    var stopDrag = function () {
        removeListener(document, "mousemove", move, false);
        removeListener(document, "mouseup", stopDrag, false);
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
