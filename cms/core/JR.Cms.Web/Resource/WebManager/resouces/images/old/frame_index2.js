jr.xhr.max_request = 4;

if (!window._path) {
    window._path = 'admin';
}
window.md = null;
window.menuTree = null;
window.sites = [];
window.username = null;
window.groupname = null;


jr.xhr.get(window._path + '?module=ajax&action=appinit', function (x) {
    var ip, address;
    eval(x);
});


/********************************** RIBBON JS *********************************/
var RIBBON = {

    ele: {
        source: null,       //Ribbon元素
        page: null,         //Pages元素
        header: null
    },
    tabPages: null,
    tabPageHeaders: null,

    //迭代TabPages
    eachPage: function (call) {
        var tabHeaders = this.tabPageHeaders;
        jr.each(this.tabPages, function (i, e) {
            if (call) call(i, e, tabHeaders[i]);
        });
    },

    //设置显示第几页
    setIndex: function (i) {
        if (this.tabPages[i].onclick) {
            this.tabPages[i].onclick();
        }
    },

    //添加页
    addPage: function (pageText, headerHtml) {
        var li = document.createElement('LI');
        li.innerHTML = pageText + '<span></span>';
        this.ele.page.getElementsByTagName('UL')[0].appendChild(li);

        var group = document.createElement('DIV');
        group.className = 'ribbon-page-header hidden';
        group.innerHTML = headerHtml;

        this.ele.header.appendChild(group);

        this.init(this.ele.source);
    },

    //初始化
    init: function (e) {
        var supprtClassSelector = !!e.getElementsByClassName;

        this.ele.source = e;
        this.ele.page = (supprtClassSelector ? e.getElementsByClassName('ribbon-pages') : document.getElementsByClassName('ribbon-pages', e))[0];
        this.ele.header = (supprtClassSelector ? e.getElementsByClassName('ribbon-header') : document.getElementsByClassName('ribbon-header', e))[0];

        this.tabPages = this.ele.page.getElementsByTagName('LI');
        this.tabPageHeaders = (supprtClassSelector ? this.ele.header.getElementsByClassName('ribbon-page-header') : document.getElementsByClassName('ribbon-page-header', this.ele.header));

        var R = this;
        jr.each(this.tabPages, function (i, e) {
            e.onclick = (function (_i) {
                return function () {
                    jr.each(R.tabPages, function (j, je) {
                        je.className = j != _i ? '' : 'selected';
                        R.tabPageHeaders[j].className = j != _i ? 'ribbon-page-header hidden' : 'ribbon-page-header';
                    });

                };

            })(i);
        });
    },

    pageLoad: function () { alert('x'); }
};

function $(id) { return jr.$(id); }



function menu_init() {
    var title, html, linktext, url;
    for (var i1 = 0; i1 < md.length; i1++) {
        title = md[i1].text;
        html = '';
        for (var i2 = 0; i2 < md[i1].childs.length; i2++) {
            if (md[i1].childs[i2].childs.length > 0) {
                html += '<div class="group"><h2>' + md[i1].childs[i2].text + '</h2><div class="fns" id="fns_' + i2 + '">';
                for (var i3 = 0; i3 < md[i1].childs[i2].childs.length; i3++) {
                    linktext = md[i1].childs[i2].childs[i3].text;
                    url = md[i1].childs[i2].childs[i3].uri;
                    html += (i3 != 0 && i3 % 4 == 0 ? '<div class="clearfix"></div>' : '') +
                  '<a class="fn" style="cursor:pointer;" url="' + url + '"' +
                  (md[i1].childs[i2].childs.length == 1 ? ' style="margin:0 ' + ((100 - linktext.length * 14) / 2) + 'px"' : '') +
                  '><span class="icon icon_' + i1 + '_' + i2 + '_' + i3 + '"></span>' + linktext + '</a>';
                }
                html += '</div></div>';
            }
        }

        RIBBON.addPage(title, html);
    }

    RIBBON.setIndex(0);

    jr.each(jr.dom.getsByClass(RIBBON.ele.source, 'fn'), function (i, e) {
        e.onclick = function () {
            tab.show(this.innerHTML, this.getAttribute('url'));
        };
    });

}

RIBBON.resize = function () {
    var ribbonHeadHeight = (this.ele || RIBBON.ele).source.offsetHeight;
    var height = document.documentElement.clientHeight;
    var width = document.documentElement.clientWidth;
    var sib = (this.ele || RIBBON.ele).source.nextSibling;
    while (sib && sib.nodeName != 'DIV') {
        sib = sib.nextSibling;
    }
    if (sib != null) {
        var offset = 20;
        sib.style.height = (height - ribbonHeadHeight - offset) + 'px';
        var pls = (sib.getElementsByClassName ? sib.getElementsByClassName('ribbon-main-right') : document.getElementsByClassName('ribbon-main-right', sib))[0].getElementsByTagName('DIV');
        pls[2].style.height = (sib.offsetHeight - pls[0].offsetHeight) + 'px';
        jr.$('ribbon-main-right').style.width = (width - jr.$('ribbon-main-left').offsetWidth) + 'px';
    }
};



/* Tab管理 */
var tab = {
    //框架集
    frames: null,
    tabs: null,
    initialize: function () {
        var framebox = jr.$('pageframes');
        this.frames = framebox.getElementsByTagName('DIV')[2];
        this.tabs = jr.$('pagetabs').childNodes[0];
    },
    pageBeforeLoad: function () {
        var frames = this.frames;
        var _frames = [this.frames.previousSibling.previousSibling, this.frames.previousSibling];
        jr.each(_frames, function (i, e) {
            if (e.nodeName == '#text') e = e.previousSibling;
            if (i == 0 && e.offsetWidth == 0) {
                e.style.width = frames.offsetWidth + 'px';
                e.style.height = frames.offsetHeight + 'px';
            }
            e.className = e.className.replace(' hidden', '');
        });
    },
    pageLoad: function () {
        var _frames = [this.frames.previousSibling.previousSibling, this.frames.previousSibling];
        jr.each(_frames, function (i, e) {
            if (e.nodeName == '#text') e = e.previousSibling;
            setTimeout(function () { e.className += ' hidden'; }, 200);
        });
    },
    show: function (text, url) {
        var _tabs = this.tabs.getElementsByTagName('LI');
        var _indent;
        var _exits = false;
        var _cur_indents = url;
        var _li = null;

        jr.each(_tabs, function (i, obj) {
            _indent = obj.getAttribute('indent');
            if (_indent == _cur_indents) {
                _exits = true;
                obj.className = 'current';
                _li = obj;
            }
        });
        if (!_exits) {
            this.pageBeforeLoad();
            //添加框架
            var frameDiv = document.createElement('DIV');
            var frame = document.createElement('IFRAME');
            frame.src = url;
            this.frames.appendChild(frameDiv);

            var _loadCall = (function (t) {
                return function () {
                    t.pageLoad.apply(t);
                };
            })(this);

            frame.frameBorder = '0';
            frame.setAttribute('frameBorder', '0', 0);
            frame.setAttribute('border', '0');
            frame.setAttribute('indent', _cur_indents);
            frame.setAttribute('id', 'ifr_' + _cur_indents);
            jr.event.add(frame, 'load', _loadCall);

            frameDiv.appendChild(frame);


            //添加选项卡
            _li = document.createElement('LI');
            _li.onmouseout = (function (t) {
                return function () {
                    if (t.className != 'current') t.className = '';
                };
            })(_li);
            _li.onmouseover = (function (t) {
                return function () {
                    if (t.className != 'current') t.className = 'hover';
                };
            })(_li);
            _li.setAttribute('indent', _cur_indents);
            _li.innerHTML = '<span class="txt"><span class="link" onclick="tab.set(this)">' + text
                + '</span><span class="closebtn" title="关闭选项卡" onclick="tab.close(this);">x</span>'
                + '</span><span class="rgt"></span>';

            this.tabs.appendChild(_li);
        }

        //触发事件,切换IFRAME
        this.set(_li);
    },
    set: function (t) {
        var li = t.nodeName != 'LI' ? t.parentNode.parentNode : t;
        var _frames = this.frames.getElementsByTagName('DIV');
        var _lis = this.tabs.getElementsByTagName('LI');
        var offsetHeight = this.frames.parentNode.offsetHeight;
        jr.each(_lis, function (i, obj) {

            if (obj == li) {
                obj.className = 'current';
                _frames[i].className = 'current';
                _frames[i].style.height = offsetHeight + 'px';

            } else {
                obj.className = '';
                _frames[i].className = '';
                _frames[i].style.height = '0px';
            }
        });

    },
    close: function (t) {
        var closeIndex = -1;
        var isActived = false;
        var offsetHeight = this.frames.parentNode.offsetHeight;

        jr.each(jr.dom.getsByClass(this.tabs, 'closebtn'), function (i, obj) {
            if (obj == t) {
                closeIndex = i + 1;
                isActived = obj.parentNode.parentNode.className == 'current';
            }
        });

        if (closeIndex > 0) {
            var _lis = this.tabs.getElementsByTagName('LI');
            var _ifrs = this.frames.getElementsByTagName('DIV');

            var ifr = _ifrs[closeIndex].childNodes[0];
            if (ifr.nodeName == 'IFRAME') {
                ifr.src = '';
                ifr = null;
            }

            this.tabs.removeChild(_lis[closeIndex]);
            this.frames.removeChild(_ifrs[closeIndex]);

            //如果关闭当前激活的tab,则显示其他的tab和iframe
            if (isActived) {
                if (closeIndex >= _lis.length) {
                    closeIndex = _lis.length - 1;
                }
                _lis[closeIndex].className = 'current';
                if (_ifrs[closeIndex]) {
                    _ifrs[closeIndex].className = 'current';
                    _ifrs[closeIndex].style.height = offsetHeight + 'px';
                }

            }
        }
    }

};

tab.initialize();

RIBBON.init(document.getElementById('ribbon'));

//添加
//RIBBON.addPage('Tab3', '<div class="group"><h2>分组信息3</h2> <p>这是一个Tab</p></div>');


//初始化站点
var timer = setInterval(function () {
    if (window.md != null) {
        clearInterval(timer);

        window.M.loadCatTree();
        menu_init();


        jr.$('username').innerHTML = username;
        jr.$('groupname').innerHTML = groupname;

        var ifr = jr.$('ifr_first');
        ifr.src = ifr.getAttribute('ref');

        RIBBON.resize();
        loadApps();
        initSites();
    }
}, 1000);

//加载app
function loadApps() {
    var ele;
    jr.each(document.getElementsByTagName('H2'), function (i, e) {
        if (e.innerHTML == 'APPS') {
            ele = e.parentNode.getElementsByTagName('DIV')[0];
        }
    });
    if (ele) {
        ele.id = 'ribbon-apps';
        jr.load(ele, window._path + '?module=plugin&action=miniapps&ajax=1');
    }
}

//初始化站点
function initSites() {

    var siteEle = jr.$('sites');
    if (window.sites.length == 0) {
        siteEle.style.display = 'none';
    } else {
        var html = '<ul>';
        for (var i in window.sites) {
            html += '<li><a href="javascript:;" siteid="' + window.sites[i].id + '">' + window.sites[i].name + '</a>';
        }
        html += '</ul>';

        siteEle.getElementsByTagName('DIV')[0].innerHTML = html;
        jr.each(siteEle.getElementsByTagName('DIV')[0].getElementsByTagName('LI'), function (i, e) {
            e.getElementsByTagName('A')[0].onclick = function () {
                setSite(this.getAttribute('siteid'));
            };
        });

    }
}

//加载用户状态
jr.$('custom_menu').innerHTML = jr.$('headerOper').innerHTML;
//jr.hover(document.getElementsByClassName('ribbon-menu')[0]);

window.M = {
    dialog: function (id, title, url, isAjax, width, height, closeCall) {
        var d = jr.dialog.create2(title, true, true);
        d.onclose = closeCall;
        if (isAjax) {
            d.async(url);
        } else {
            d.open(url, width, height);
        }
        return d;
    },
    alert: function (html, func) {
        jr.tipbox.show(html, false, 100, 2000, 'up');
        if (func) {
            setTimeout(func, 1000);
        }
    },
    msgtip: function (arg, func) {
        jr.tipbox.show(arg.html, false, 100, arg.autoClose ? 2000 : -1, 'up');
        if (func) {
            setTimeout(func, 1000);
        }
    },
    tip: function (msg, func) {
        this.msgtip({ html: msg, autoClose: true }, func);
    },
    loadCatTree: function () {
        var mt = jr.$('menutree');
        jr.load(jr.$('menutree'), window._path + '?module=category&action=tree&for=archives&siteid=&ajax=1&rd=' + Math.random() + '#noload', function (result) {
            //var left = mt.getElementsByTagName('B')[0].innerHTML.replace(/<[^>]+>/, '').length * 14 + 30;
            //jr.$('sites').style.left = left + 'px';
        });
    },
    clearCache: function (t) {
        window.M.msgtip({ html: '清除中....' });
        jr.xhr.post(window._path, 'module=ajax&action=clearcache', function (x) {
            window.M.msgtip({ html: '缓存清除完成!', autoClose: true });
            jr.xhr.get('/');
        }, function (x) { });
    },
    addFavorite: function () {
        var url = location.href;
        var title = document.title;
        try {
            window.external.addFavorite(url, title);
        }
        catch (e) {
            try {
                window.sidebar.addPanel(title, url, "");
            }
            catch (e) {
                alert("浏览器不支持,请手动添加！");
            }
        }
    },
    setFullScreen: function (event) {
        //var leftWidth = $(e_SD).offsetWidth;
        //if (leftWidth >= window.M.epix.leftWidth) {
        if (!$(e_SD).parentNode.style || $(e_SD).parentNode.style.display != 'none') {
            //全屏
            $(e_HD).style.height = '0px';
            $(e_SD).style.width = '0px';
            $(e_FT).style.height = '0px';
            $(e_HD).style.overflow = 'hidden';
            $(e_SD).parentNode.style.cssText += 'display:none';
        } else {
            //取消全屏
            $(e_HD).style.overflow = '';
            $(e_HD).style.height = (window.M.epix.topHeight - 5) + 'px';
            $(e_SD).style.width = (window.M.epix.leftWidth - 1) + 'px';
            $(e_FT).style.height = (window.M.epix.footHeight - 1) + 'px';
            $(e_SD).parentNode.style.display = '';
        }
        window.onresize();
    }
};


function setSite(id) {
    jr.xhr.request({ uri: window._path + '?module=system&action=selectsite&json=1&siteid=' + id, data: 'json' },
        {
            success: function (html) {
                window.M.loadCatTree();
                jr.xhr.get(window._path + '?module=ajax&action=appinit&onlysite=true', function (x) {
                    var ip, address;
                    eval(x);
                    initSites();
                });
            }
        }
    );
}

/* 重设窗口的大小 */
window.onresize = RIBBON.resize;

//设置按键
window.onload = function () {
    document.onkeydown = function (event) {
        var e = window.event || event;
        //按键ALT+F11,启用全屏
        if (e.altKey && e.keyCode == 122) {
            window.M.setFullScreen();
            e.returnvalue = false;
            return false;
        } else if (e.keyCode == 122) {
            window.M.setFullScreen();
            e.returnvalue = false;
            return false;
        } else if (!e.ctrlKey && e.keyCode == 116) {
            var ifr = null;
            var ifrs = document.getElementsByTagName('IFRAME');
            for (var i = 0; i < ifrs.length; i++) {
                if (ifrs[i].className == 'current') {
                    ifr = ifrs[i];
                    break;
                }
            }
            if (ifr != null) {
                var src = ifr.src;
                ifr.src = '';
                ifr.src = src;
            }
            e.returnvalue = false;
            return false;
        }
    };
};


