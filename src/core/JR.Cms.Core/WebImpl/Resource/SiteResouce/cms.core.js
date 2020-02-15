if (!window.$b) var $b = $jr;



//设置工作路径
$jr.__WORKPATH__ = '/public/assets/js/';
window.jr = $jr;
window.j6 = $jr;
window.cms = $jr;


/****************  页面处理事件 **************/
var _scripts = document.getElementsByTagName('SCRIPT');
var _sloc = _scripts[_scripts.length - 1].src;                                  //Script Location
var _hp = {                                                                //Script Handle Params
    loadUI: jr.request('ui', _sloc) == '1',                                     //load ui lib
    hoverNavi: jr.request('hover', _sloc).indexOf('navi') != -1,            //Hover Navigator
    hoverCList: jr.request('hover', _sloc).indexOf('clist') != -1,           //Hover Category List
    hoverAList: jr.request('hover', _sloc).indexOf('alist') != -1,           //Hover Archive List
    plugins: jr.request('ld', _sloc)
};

var plugins = null;
if (_hp.loadUI) {
    plugins = new Array('ui', 'scrollbar', 'scroller', 'form');
} else if (_hp.plugins) {
    plugins = _hp.plugins.split(',');
}
if (plugins) {
    for (var i = 0; i < plugins.length; i++) {
        jr.ld(plugins[i]);
    }
}

/************** 自动设置导航 *********************/
var _auto_checked_navigator = false;
var _auto_navigator_ele;
var _auto_navigator_timer;
if (_hp.hoverNavi) {
    var navi = null;
    _auto_navigator_timer = setInterval(function () {
        navi = document.getElementsByClassName('page-navigator');
        if (navi && navi.length != 0) {
            navi = navi[0];
            var loc = window.location.pathname;
            var lis = [];
            var lis2 = navi.getElementsByTagName('UL')[0].childNodes;
            jr.each(lis2, function (i, e) {
                if (e.nodeName == "LI") {
                    lis.push(e);
                }
            });

            if (lis.length > 0) {
                var link;
                var isHovered = false;
                var setHover = function (_loc) {
                    for (var i = 0; i < lis.length; i++) {
                        link = lis[i].getElementsByTagName('A')[0];
                        if (link.href.indexOf(_loc) != -1) {
                            lis[i].className += lis[i].className.indexOf('current') == -1 ? ' current' : '';
                            isHovered = true;
                            break;
                        }
                    }
                };

                //全局匹配
                setHover(loc);

                if (!isHovered) {
                    var _loc = loc;
                    //模糊匹配
                    var i = 0;
                    while (!isHovered) {
                        var splitIndex = _loc.lastIndexOf('/');
                        if (splitIndex == _loc.length - 1) {
                            splitIndex = _loc.substring(0, _loc.length - 1).lastIndexOf('/');
                        }
                        _loc = _loc.substring(0, splitIndex + 1);
                        setHover(_loc);
                        if (++i > lis.length) {
                            isHovered = true;
                        }
                    }
                }
                setIE6Drop(lis);
            }
            clearInterval(_auto_navigator_timer);
        }
    }, 100);
}

function setIE6Drop(lis) {
    /****************** 设置二级菜单 *******************/
    //针对IE6不支持hover属性
    if (window.ActiveXObject) {
        var agent = window.navigator.userAgent;
        if (/MSIE\s*(6|7)\.0/.test(agent)) {

            for (var i = 0; i < lis.length; i++) {
                jr.event.add(lis[i], 'mouseover', (function (t) {
                    return function () {
                        t.className += ' drop';
                    };
                })(lis[i]));

                jr.event.add(lis[i], 'mouseout', (function (t) {
                    return function () {
                        t.className = t.className.replace(' drop', '');
                    };
                })(lis[i]));

            }
        }
    }
}


$b.event.add(window, 'load', function () {
    if (_hp.hoverNavi && _auto_navigator_ele) {
        clearInterval(_auto_navigator_timer);
    }

    var loc = window.location.pathname;


    /****************** 设置分类菜单 *******************/

    //根据className设置Hover状态
    var setHoverByClassName = function (e) {
        var lis = e.childNodes;
        var link;
        var isHovered = false;

        var setHover = function (_loc) {
            for (var i = 0; i < lis.length; i++) {
                if (lis[i].nodeName[0] == '#') continue;
                link = lis[i].getElementsByTagName('A')[0];
                if (link.href.indexOf(_loc) != -1) {
                    lis[i].className += lis[i].className.indexOf('current') == -1 ? ' current' : '';
                    isHovered = true;
                    break;
                }
            }
        };

        //全局匹配
        setHover(loc);

        //模糊匹配
        if (!isHovered) {
            var splitIndex = loc.lastIndexOf('/');
            if (splitIndex == loc.length - 1) {
                splitIndex = loc.substring(0, loc.length - 1).lastIndexOf('/');
            }
            setHover(loc.substring(0, splitIndex + 1));
        }
    };

    // 设置CList选中效果 (2012-11-03) **
    var _e_clist = document.getElementsByClassName('clist');
    if (_hp.hoverCList && _e_clist.length != 0) {
        setHoverByClassName(_e_clist[0]);
    }

    // 设置AList选中效果 (2012-11-03) **
    _e_clist = document.getElementsByClassName('alist');
    if (_hp.hoverAList && _e_clist.length != 0) {
        setHoverByClassName(_e_clist[0]);
    }

    // 选项卡
   
    $b.$fn(".tab").each(function (i, e) {
        var tabFN = function () {
            var t = this;
            var parent = this.parent();
            var active_i = -1;
            var active = function (e, b) {
                if (b) e.addClass("actived");
                else e.removeClass("actived");
            };
            parent.find(".tab").each(function (i, e) {
                var same = e.raw() == t.raw();
                if (same) active_i = i;
                active(e, same);
            });
            while (parent && parent.find(".frame").len() == 0) {
                parent = parent.parent();
            }
            parent.find(".frame").each(function (i, e) {
                active(e, active_i == i);
            });
        };
        if (e.hasClass("tab-hover") && document.documentElement.offsetWidth > 991) {
            e.mouseover(tabFN);
        } else {
            e.click(tabFN);
        }
    });

    // 滚动到目标
    var scrollLock = 0;
    $b.$fn(".scroll-to").click(function () {
        if (scrollLock == 1) return;
        scrollLock = 1;
        var target = this.attr("target");
        if (!target) throw ".scoll-top missing attribute \"target\"";
        var ele = this;
        var target = $b.$fn(target);
        var offset = parseInt(this.attr("offset") || 0);
        var y = target.attr("offsetTop") + offset;
        var doc = document.documentElement;
        var timer = setInterval(function () {
            var setup = (y - doc.scrollTop) / 5;
            setup = setup > 0 ? Math.floor(setup) : Math.ceil(setup);
            if (Math.abs(setup) == 0) {
                doc.scrollTop = y;
                clearInterval(timer);
                scrollLock = 0;
            } else {
                doc.scrollTop += setup;
            }
        },10);
    });

    // 将元素绝对定位
    $b.event.add(document, "scroll", function () {
        var fixedArr = $b.$fn(".scroll-link");
        var scrollTop = document.documentElement.scrollTop;
        fixedArr.each(function (i, e) {
            var top = e.attr("offsetTop") + e.parent().attr("offsetTop");
            if (scrollTop > top) {
                if (e.css().position != "fixed") {
                    var left = e.attr("offsetLeft");
                    var width = e.attr("offsetWidth");
                    e.css({ position: "fixed", top: "0", left: left + "px", width: width + "px" });
                    e.addClass("scroll-linked");
                }
            } else {
                e.css({ position: "inherit", top: "inherit", left: "inherit", width: "inherit" });
                e.removeClass("scroll-linked");
            }
        });
    });

    // Exchange
    $b.$fn(".ui-exchange").each(function (i, e) {
        e = e.raw();
        var v = null; var d = null; var f = null;
        var g = 'exchange';
        switch (e.nodeName) { case 'IMG': f = 'src'; break; default: f = 'innerHTML'; break }
        if (f == null) return; v = e[f]; d = e.getAttribute(g); if (d) {
            jr.event.add(e, 'mouseover', (function (a, b, c) { return function () { a[b] = c } })(e, f, d));
            jr.event.add(e, 'mouseout', (function (a, b, c) { return function () { a[b] = c } })(e, f, v));
        }
    });
    // 初始化wow.js
    if (window.WOW && !window.wowInit) {
        new WOW().init();
        window.wowInit = true;
    }
});

/***********************  设置自动时间  ***********************/
/*
var ele_dts = document.getElementsByClassName('autotime');
var weeks = new Array('日', '一', '二', '三', '四', '五', '六');
var cmath = function (v) {
    if (v < 10) return '0' + v;
    return v;
};
var setDate = function () {
    var dt = new Date();
    var str = cmath(dt.getFullYear()) + '年' + cmath(dt.getMonth() + 1) + '月' + cmath(dt.getDate()) + '日&nbsp;/&nbsp;周' +
          weeks[dt.getDay()] + '&nbsp;' + cmath(dt.getHours()) + ':' + cmath(dt.getMinutes()) + ':' + cmath(dt.getSeconds());

    for (var i = 0; i < ele_dts.length; i++) {
        ele_dts[i].innerHTML = str;
    }
};
if (ele_dts.length != 0) {
    setDate();
    setInterval(setDate, 1000);
}
*/




