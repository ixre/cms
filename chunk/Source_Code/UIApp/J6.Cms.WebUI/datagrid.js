if (!window._path) {
    window._path = 'admin';
}
window.sites = [];
window.groupname = null;

if (window.menuData == undefined) {
    j6.xhr.get(window._path + '?module=ajax&action=appinit', function (x) {
        var ip, address, md, username;
        eval(x);
        window.menuData = md;
        j6.json.bind(document, { userName: username });
    });
    //window.menuData = [];
}

if (window.menuHandler == undefined) {
    window.menuHandler = null;
}


var FwMenu = {
    ele: null,
    menuTitles: [],
    getByCls: function (className) {
        return this.ele.getElementsByClassName ? this.ele.getElementsByClassName(className) : document.getElementsByClassName(className, this.ele);
    },
    init: function (data, menuHandler) {
        //获取菜单元素
        this.ele = document.getElementsByClassName('page-left-menu')[0];
        //第一次加载
        var md = data;

        //处理菜单数据
        if (menuHandler && menuHandler instanceof Function) {
            var hdata = menuHandler(data);
            if (hdata != undefined && hdata != null) {
                md = hdata;
            }
        }

        var menuEle = this.ele;

        menuEle.innerHTML = '';
        var title, html, linktext, url;
        for (var i1 = 0; i1 < md.length; i1++) {
            title = md[i1].text;
            html = '';
            for (var i2 = 0; i2 < md[i1].childs.length; i2++) {
                if (md[i1].childs[i2].childs.length > 0) {
                    html += '<div class="group-title" group="' + md[i1].id + '" style="cursor:pointer" title="点击展开操作菜单"><span>' + md[i1].childs[i2].text + '</span></div>';
                    html += '<div class="panel hidden"><ul id="fns_' + i2 + '">';
                    for (var i3 = 0; i3 < md[i1].childs[i2].childs.length; i3++) {
                        linktext = md[i1].childs[i2].childs[i3].text;
                        url = md[i1].childs[i2].childs[i3].uri;
                        // html += (i3 != 0 && i3 % 4 == 0 ? '<div class="clearfix"></div>' : '') +
                        html += '<li' + (i2 == 0 && i3 == 0 ? ' class="current"' : '') + '><a class="fn" style="cursor:pointer;" url="' + url + '"' +
                       //(md[i1].childs[i2].childs.length == 1 ? ' style="margin:0 ' + ((100 - linktext.length * 14) / 2) + 'px"' : '') +
                       '><span class="icon icon_' + i1 + '_' + i2 + '_' + i3 + '"></span>' + linktext + '</a></li>';
                    }
                    html += '</ul></div>';
                }
            }
            menuEle.innerHTML += html;
        }

        //获取所有的标题菜单
        this.menuTitles = this.getByCls('group-title');
        var t = this;
        j6.each(this.menuTitles, function (i, e) {
            var groupName = e.getAttribute('group');
            j6.event.add(e, 'click', (function (_t, _e) {
                return function () {
                    _t.show(_e);
                };
            })(t, e));

            j6.each(e.nextSibling.getElementsByTagName('LI'), function (i2, e2) {
                j6.event.add(e2, 'click', (function (_this, _t, g) {
                    return function () {
                        _t.set(groupName, _this);
                        var a = _this.childNodes[0];
                        if (a.url != '') {
                            FwTab.show(a.innerHTML, a.getAttribute('url'));
                        }
                    };
                })(e2, t, groupName));
            });
        });

    },
    //设置第几组显示
    change: function (id) {
        var menuTitles = this.menuTitles;
        var groupName = id;
        if (!groupName) {
            if (menuTitles.length == 0) {
                return;
            } else {
                groupName = menuTitles[0].getAttribute('group');
            }
        }
        var isFirst = true;
        var selectedLi = null;  //已经选择的功能菜单
        var firstPanel = null;
        var titleGroups = [];
        var _lis;

        j6.each(menuTitles, function (i, e) {
            if (e.getAttribute('group') != groupName) {
                e.className = 'group-title hidden';
                e.nextSibling.className = 'panel hidden';
            } else {
                titleGroups.push(e);
                e.className = 'group-title';

                //第一个panel
                if (firstPanel == null) {
                    firstPanel = e.nextSibling;
                }
            }
        });

        for (var i = 0; i < titleGroups.length; i++) {
            _lis = titleGroups[i].nextSibling.getElementsByTagName('LI');
            for (var j = 0; j < _lis.length; j++) {
                if (_lis[j].className == 'current') {
                    selectedLi = _lis[j];
                    i = titleGroups.length + 1; //使其跳出循环
                    break;
                }
            }
        }

        if (selectedLi != null) {
            selectedLi.parentNode.parentNode.className = 'panel';
        } else if (firstPanel != null) {
            firstPanel.className = 'panel';
        }

    },
    //查看菜单
    show: function (titleDiv) {
        var groupName = titleDiv.getAttribute('group');
        j6.each(this.menuTitles, function (i, e) {
            if (e.getAttribute('group') == groupName) {
                if (e != titleDiv) {
                    e.nextSibling.className = 'panel hidden';
                } else {
                    e.nextSibling.className = 'panel';
                }
            }
        });
    },
    set: function (groupName, ele) {
        j6.each(this.menuTitles, function (i, e) {
            if (e.getAttribute('group') == groupName) {
                j6.each(e.nextSibling.getElementsByTagName('LI'), function (i, e2) {
                    e2.className = ele == e2 ? 'current' : '';
                });
            }
        });
    }
};







//=========================== 辅助方法 ===============================//
function _loadCategoryTree() {

    var treeTitle = '站点栏目';
    var treeGroupName = 'content';
    var menuTitles = FwMenu.menuTitles;
    var e = null;
    for (var i = 0; i < menuTitles.length; i++) {
        e = menuTitles[i];
        var groupName = e.getAttribute('group');
        if (groupName == treeGroupName) {
            if (e.innerHTML.replace(/<[^>]+>/g, '') == treeTitle) {
                j6.load(e.nextSibling, window._path + '?module=category&action=tree&for=archives&siteid=&ajax=1&rd=' + Math.random() + '#noload', function (result) { });
                break;
            }
        }
    }
}