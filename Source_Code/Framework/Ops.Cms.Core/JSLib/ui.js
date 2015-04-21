
/*
* 名称 ： UI库
* 创建时间：2012-09-22
*/


/********* Tipbox (2013-06-01) ************/

$JS.extend({
    tipbox: {
        id: 'ui-tipbox',
        size: { x: 0, y: 0, bx: 0, by: 0 },
        show: function (html, noStyle, topOffset, timeout, dir) {
            var div = document.getElementById(this.id);
            if (div) { document.body.removeChild(div); }
            div = document.createElement('DIV');
            div.setAttribute('id', this.id);
            div.className = this.id;
            div.style.cssText += 'position:fixed;width:auto;overflow:hidden;';
            div.innerHTML = '<div class="ui-tipbox-container">' + html + '</div>';
            document.body.appendChild(div);

            if (noStyle) {
                div.innerHTML += '<style>.ui-tipbox{border:solid 5px #006666;background:white;}.ui-tipbox .ui-tipbox-container{padding:30px 30px 20px 40px;}</style>';
            }

            //计算长,宽
            this.size.x = div.offsetWidth;
            this.size.y = div.offsetHeight;
            this.size.bx = document.documentElement.clientWidth;
            this.size.by = document.documentElement.clientHeight;


            //设置内置的div的宽度
            div.getElementsByTagName("DIV")[0].style.width = this.size.x + 'px';


            //设置上下左右
            div.style.width = '1px';
            div.style.height = '1px';

            var _x = 1, _y = 1, _opa = 0, _px = (this.size.x > this.size.y ? this.size.x : this.size.y) / 20 / 2;
            var _size = this.size;
            var _timer = setInterval(function () {

                ++_px;
                if (_x + _px > _size.x) {
                    _x = _size.x;
                } else {
                    _x += _px;
                }

                if (_y + _px > _size.y) {
                    _y = _size.y;
                } else {
                    _y += _px;
                }

                div.style.width = _x + 'px';
                div.style.height = _y + 'px';
                div.style.left = ((_size.bx - _x) / 2) + 'px';
                div.style.top = ((_size.by - _y) / 2 - topOffset) + 'px';

                _opa += 5;

                if (div.style.filter) {
                    div.style.filter = 'filter:alpha(opacity=' + _opa + ')';
                } else {
                    div.style.opacity = _opa / 100;
                }

                if (div == null || (_x == _size.x && _y == _size.y && _opa >= 100)) {

                    //清除定时器
                    clearInterval(_timer);

                }

            }, 10);

            //定时关闭
            if (timeout > 0) {
                var func = (function (t) {
                    return function () {
                        t.close();
                    };
                })(this);
                setTimeout(func, timeout);
            }
        },
        close: function (dir) {
            var div = document.getElementById(this.id);
            var _opa = 100;
            var _isUp = dir != 'left';
            var _t = _isUp ? div.offsetTop : div.offsetLeft;            //top和left的像素
            var _tt = -(_isUp ? this.size.y : this.size.x) - 20;       //要滚动到的最终点
            var _tpx = _t / 40;                                        //滚动的单位
            var _timer = setInterval(function () {
                ++_tpx;
                _t -= _tpx;

                if (div == null || _t < _tt) {
                    if (div) {
                        try {
                            document.body.removeChild(div);
                        } catch (exc) { }
                    }
                    clearInterval(_timer);
                } else {
                    if (_isUp) {
                        div.style.top = _t + 'px';
                    } else {
                        div.style.left = _t + 'px';
                    }

                    _opa -= 5;

                    if (div.style.filter) {
                        div.style.filter = 'filter:alpha(opacity=' + _opa + ')';
                    } else {
                        div.style.opacity = _opa / 100;
                    }
                }
            }, 10);
        }
    }
});

/******************** JS 分页 (2013-06-19) **********************/

$JS.extend({
    toPager: function (id, size) {
        this.size = size;
        this.pageIndex = 1;
        this.pages = 0;
        this.pager = null;
        this.list = null;

        var container = document.getElementById(id);

        //获取分页列表
        this.list = container.getElementsByClassName ? document.getElementsByClassName('list', container) : container.getElementsByClassName('list');

        //计算页码
        this.pages = parseInt(this.list.length / this.size);
        if (this.list.length % this.size != 0) this.pages++;

        //获取分页容器
        if (this.pager == null) {
            var pagerArea = container.getElementsByClassName ? document.getElementsByClassName('pager', container) : container.getElementsByClassName('pager');
            if (pagerArea.length == 0) {
                var div = document.createElement('DIV');
                div.className = 'pager';
                container.appendChild(div);
                this.pager = div;
            } else {
                this.pager = pagerArea[0];
            }
        }


        var t = this;
        var links;

        this.showPage = function (index) {
            t.pageIndex = index;
            for (var i = 0; i < t.list.length; i++) {
                t.list[i].style.display = i >= (t.pageIndex - 1) * t.size && i < t.pageIndex * t.size ? 'block' : 'none';
            }
            t.pager.innerHTML = '页码:';

            for (var j = 0; j < t.pages; j++) {
                t.pager.innerHTML += '&nbsp;' + (t.pageIndex == j + 1 ? j + 1 : '<a href="javascript:;" page="' + (j + 1) + '">' + (j + 1) + '</a>');
            }

            links = t.pager.getElementsByTagName('A');

            for (var k = 0; k < links.length; k++) {
                links[k].onclick = (function (_p) {
                    return function () {
                        t.showPage(_p);
                    };
                })(links[k].getAttribute('page'));
            }
        };

        this.showPage(1);
    }
});


//=======================  扩展 =====================//
$JS.extend({
    contextmenu: {
        ele: null,
        currEle: null,
        inst: null,
        offset: 5,
        srcs: null,      //触发源
        show: function () {
            this.ele.style.display = 'block';
        },
        close: function () {
            this.ele.style.display = 'none';
        },
        bind: function (e, contextHTML, handler) {
            var j = $JS;
            //初始化
            if (!this.ele) {
                this.srcs = new Array();

                //添加菜单到body
                this.ele = document.createElement('DIV');
                this.ele.className = 'ui-contextmenu';
                this.ele.style.cssText = 'position:absolute;';
                document.body.appendChild(this.ele);

                //将menu添加到原元素里
                this.srcs.push(this.ele);

                //初始化事件 
                j.event.add(document.body, 'click', (function (e) { return function () { e.close.apply(e); }; }(this))); //关闭菜单

                this.ele.oncontextmenu = function () { return false; };

                // 屏蔽元素右键菜单 
                document.oncontextmenu = (function (ele, srcs, e) {
                    return function (event) {
                        var src = j.event.src(event);
                        var parent = src;
                        while (parent) {
                            for (var i = 0; i < srcs.length; i++) {
                                if (srcs[i] == parent) {
                                    return false;
                                }
                            }
                            parent = parent.parentNode;
                        }
                        return true;
                    };
                }(this.ele, this.srcs, e));
            }

            //添加到源中
            this.srcs.push(e);

            //显示菜单事件
            j.event.add(e, 'mouseup', (function (_this) {
                return function (_e) {
                    var event = _e ? _e : window.event; // 兼容IE,Firefox,Chrome  

                    if (event.button != 2) {
                        return false;
                    }

                    var mn = _this.ele;

                    var dom_TOP = Math.max(document.documentElement.scrollTop, document.body.scrollTop),
                        dom_LEFT = Math.max(document.documentElement.scrollLeft, document.body.scrollLeft),
                        event_X = event.clientX,
                        event_Y = event.clientY,
                        menu_WIDTH = mn.offsetWidth,
                        menu_HEIGHT = mn.offsetHeight;

                    // 获取鼠标离窗口右下角的X和Y轴距离(与滚动无关)  
                    var redge = Math.max(document.documentElement.clientWidth, document.body.clientWidth) - event_X;
                    var bedge = Math.max(document.documentElement.clientHeight, document.body.clientHeight) - event_Y;

                    //输出菜单结果
                    if (_this.currEle == null || _this.currEle != e) {
                        _this.ele.innerHTML = contextHTML;
                        if (handler) {
                            handler(_this.ele);
                        }
                    }


                    // 如果当前点击点到窗口右侧的距离小于菜单宽度，则将菜单向左弹出，否则向右弹出  
                    if (redge < menu_WIDTH) {
                        mn.style.left = (dom_LEFT + event_X - menu_WIDTH) + 'px';
                    } else {
                        mn.style.left = (dom_LEFT + event_X) + 'px';
                        _this.show();
                    }

                    // 如果当前点击点到窗口下侧的距离小于菜单高度，则将菜单向上弹出，否则向下弹出  
                    if (bedge < menu_HEIGHT) {
                        mn.style.top = (dom_TOP + event_Y - menu_HEIGHT) + 'px';
                    } else {
                        mn.style.top = (dom_TOP + event_Y) + 'px';
                        _this.show();
                    }

                    // 阻断事件冒泡传递，这一步必须  
                    j.event.stopBubble(event);
                };
            })(this));
        }
    }
});